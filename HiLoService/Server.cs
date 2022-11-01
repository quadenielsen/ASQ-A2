/*
 * File:		Server.cs
 * Project:		HiLoServer
 * By:			Quade Nielsen
 * Date:		November 12, 2021
 * Description:	This file contains the Server class for the Hi Lo Game.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using HiLoObjects;

namespace HiLoService
{
	/* 
	 * Name         :   Server
	 * Description  :   This class models the Client in the Hi Lo Game application.
	 *                  This class is responsible for handling Client-side logic.
	 */
	public class Server
	{
		private static int playerIDsDistributed = 0; //the number of players have have joined the server
		private static Mutex mutex = new Mutex(); //mutex which will be used to control access to resources
		private static volatile bool acceptingConnections = false;   //bool 


		private IPEndPoint localEndPoint; //the IPEndPoint for the listener socket of the server
		private Socket listener; //the socket that will listen for incoming connections to the server
		private Thread listenerThread;  //a thread for the listener socket

		private enum querySubstrings //the indices for the array of strings which the server will parse out of the client's messages
		{
			PLAYER_ID, PLAYER_NAME, PLAYER_GUESS, PLAYER_GAME_STATE
		}


		/*
		 * Function     :   Server
		 * Description  :   Constructor for the Server object. Creates a local IPEndpoint, a listener socket and thread using a given IP Address and Pot Number.
		 * 
		 * Parameters   :   string ip   :   the IP Address of the server
		 *                  int port    :   the port number of the server
		 * Returns      :   None
		 */
		public Server(string ip, int port)
		{
			try
			{
				IPAddress ipAddress = IPAddress.Parse(ip);
				localEndPoint = new IPEndPoint(ipAddress, port);

				listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				listenerThread = new Thread(Listen);
			}
			catch (Exception ex)
			{
				Logger.Log(ex.Message);
			}
			
		}


		/*
		 * Function     :   StartServer
		 * Description  :   Starts the server so that it begins listening for players who want to connect and play the HiLo game.
		 * 
		 * Parameters   :   None
		 * Returns      :   None
		 */
		public void StartServer()
		{
			try
			{
				//read from a config file to determine the range of numbers from which the server can choose a min, a max, and a number to guess
				GameEngine.configureRange(int.Parse(ConfigurationManager.AppSettings["HiLoMin"]), 
					int.Parse(ConfigurationManager.AppSettings["HiLoMax"]));

				//create a list of players that will be active in the server
				List<Player> players = new List<Player>();

				//begin listening 
				try
				{
					listener.Bind(localEndPoint);
					listener.Listen(10);
					acceptingConnections = true;
					listenerThread.Start(players);
				}
				catch (Exception ex)
				{
					Logger.Log(ex.Message);
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex.Message);
			}
		}


		/*
		 * Function     :   ServiceConnection
		 * Description  :   Handles requests from clients.
		 * 
		 * Parameters   :   obect o :   objects passed to the thread performing the method.
		 * Returns      :   None
		 */
		public static void ServiceConnection(object o)
		{

			//take the connection handler and the list of players out of the array
			object[] array = (object[])o;
			Socket worker = (Socket)array[0];
			List<Player> players = (List<Player>)array[1];
			EventWaitHandle handle = (EventWaitHandle)array[2];

			//prepare to store a string from the client
			string data = null;
			//prepare a buffer to recieve data 
			byte[] bytes = new Byte[1024];

			try
            {
				while (true)
				{
					int bytesRec = worker.Receive(bytes);
					data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
					if (data.IndexOf("<EOF>") > -1)
					{
						break;
					}
				}
			}
            catch (Exception ex)
            {
				Logger.Log(ex.Message);
            }
			//get data from client
			

			//parse out the data
			string[] query = data.Split('?');

			//===============
			//  NEW PLAYER
			//===============

			//if the player has just connected to the server, instantiate a new player
			if (query[(int)querySubstrings.PLAYER_ID] == "0")
			{
				//create a new player
				Player player = new Player(query[(int)querySubstrings.PLAYER_NAME]);
				//increment the number of players that have joined the server
				playerIDsDistributed++;
				//give the new player an ID
				player.PlayerID = playerIDsDistributed;

				//add the new player to the list of players on the server
				players.Add(player);

				//start a new hilo game for the player
				GameEngine.startNewGame(player);

				//send data back to client
				byte[] msg = Encoding.ASCII.GetBytes(player.PlayerID + "?" + player.Name + "?" + player.Min + "?" + player.Max + "?" + player.GameState + "?" + "<EOF>");
				worker.Send(msg);
			}

			//===============
			//  PLAYER WON
			//===============

			//if the player has won and would like to play again, start a new game for them
			else if (int.Parse(query[(int)querySubstrings.PLAYER_GAME_STATE]) == (int)gameStates.WIN)
			{
				Player player = players.Find(search => search.PlayerID == int.Parse(query[(int)querySubstrings.PLAYER_ID]));
				GameEngine.startNewGame(player);

				byte[] msg = Encoding.ASCII.GetBytes(player.PlayerID + "?" + player.Name + "?" + player.Min + "?" + player.Max + "?" + player.GameState + "?" + "<EOF>");
				worker.Send(msg);
			}

			//===============
			//  PLAYER QUIT
			//===============

			//if the player has indicated that they would like to quit, ask them if they're sure
			else if (int.Parse(query[(int)querySubstrings.PLAYER_GAME_STATE]) == (int)gameStates.PLAYER_QUIT)
			{
				//send the client a message asking whether they will quit
				byte[] msg = Encoding.ASCII.GetBytes("Are you sure you want to quit?");
				worker.Send(msg);


				try
                {
					//wait for a response from the client
					while (true)
					{
						int bytesRec = worker.Receive(bytes);
						data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
						if (data.IndexOf("<EOF>") > -1)
						{
							break;
						}
					}
				}
				catch (Exception ex)
                {
					Logger.Log(ex.Message);
                }
				

				//parse the data
				query = data.Split('?');

				//if the player would still like to quit, remove them from the list of players
				if (int.Parse(query[(int)querySubstrings.PLAYER_GAME_STATE]) == (int)gameStates.PLAYER_QUIT)
				{
					Player player = players.Find(search => search.PlayerID == int.Parse(query[(int)querySubstrings.PLAYER_ID]));
					players.Remove(player);
				}
			}

			//===============
			//  ACTIVE GAME
			//===============

			//if the player already exists, evaluate their guess
			else
			{
				//look up the player
				Player player = players.Find(search => search.PlayerID == int.Parse(query[(int)querySubstrings.PLAYER_ID]));

				//store the player's guess
				int playerGuess = int.Parse(query[(int)querySubstrings.PLAYER_GUESS]);

				//evaluate
				GameEngine.evaluateGuess(player, playerGuess);

				//send a response back to the player
				byte[] msg = Encoding.ASCII.GetBytes(player.PlayerID + "?" + player.Name + "?" + player.Min + "?" + player.Max + "?" + player.GameState + "?" + "<EOF>");
				worker.Send(msg);
			}

			worker.Shutdown(SocketShutdown.Both);
			worker.Close();
			handle.Set();
		}



		/*
		 * Function     :   Listen
		 * Description  :   Continually waits for connections to be made to the server.
		 *                  When a connection is made by a client, passes the socket over to another thread to provide services to the client.
		 *                  When the bool acceptingConnections is set to false, the thread will stop taking connections and will wait for
		 *                  any remaining connections to close.
		 * 
		 * Parameters   :   obect o :   objects passed to the thread performing the method.
		 * Returns      :   None
		 */
		private void Listen(object o)
		{
			List<Player> players = (List<Player>)o;
			List<WaitHandle> threadHandles = new List<WaitHandle>();
			try
			{
				//continually create sockets which will be passed over to threads when a connection is found
				while (acceptingConnections)
				{
					Socket worker = listener.Accept();
					if (acceptingConnections == true)
                    {
						EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.ManualReset);
						threadHandles.Add(handle);
						//when a connection is found, create a new thread to service it
						Thread thread = new Thread(ServiceConnection);
						//pass the socket and the list of players to the thread
						object[] array = { worker, players, handle };
						thread.Start(array);
					}
				}
				
			}
			catch (Exception ex)
			{
				Logger.Log(ex.Message);
			}
			
		}



		/*
		 * Function     :   ShutDown
		 * Description  :   Shuts down the server.
		 *					Stops the listener thread from taking connections and interrupts the Accept() call
		 *					made by the listener socket if it is currently waiting for a connection.
		 *					Waits for the listener thread to finish execution.
		 * 
		 * Parameters   :   None
		 * Returns      :   None
		 */
		public void ShutDown()
		{
			acceptingConnections = false;
			listener.Dispose();
			listenerThread.Join();
		}

	}
}


