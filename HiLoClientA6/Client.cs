/*
 * File:		Client.cs
 * Project:		HiLoServer
 * By:			Quade Nielsen
 * Date:		November 12, 2021
 * Description:	This file contains the Client class for the Hi Lo Game.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using HiLoObjects;
using HiLoService;

namespace A5HiLoGame
{
    /* 
    * Name         :   Client
    * Description  :   This class models the Client in the Hi Lo Game application.
    *                  This class is responsible for handling Client-side logic.
    */
    public class Client
    {
        private enum querySubstrings //the indices for the array of strings which the client will parse out of the server's responses
        {
            PLAYER_ID, PLAYER_NAME, PLAYER_MIN, PLAYER_MAX, PLAYER_GAME_STATE
        }

        private string ipAddressString; //the IP Address that the client will attempt to communicate with
        private string portNumberString; //the port number for the endpoint that the client will attempt to communicate with
        private string playerName; //the name of player playing the game on the client
        private int playerID; //the player's ID
        private int min; //the min value of the hi lo game received by the client from the server
        private int max; //the max value of the hi lo game received by the client from the server
        private int guess; //the client player's current guess to be sent to the server
        private int gameState; //the state of the hi lo game
        private string serverReply; //the string sent by the server in response to the client's requests



        /*
         * Function     :   Client
         * Description  :   The constructor for the Client class. Needs to be instantiated
         *                  with the window and then have properties set by the user.
         * 
         * Parameters   :   None
         * Returns      :   None
         */
        public Client()
        {
        }


        public string IPAddressString { get => ipAddressString; set => ipAddressString = value; }
        public string PortNumberString { get => portNumberString; set => portNumberString = value; }
        public string PlayerName { get => playerName; set => playerName = value; }
        public int PlayerID { get => playerID;}
        public int Min { get => min;}
        public int Max { get => max;}
        public int Guess { get => guess; set => guess = value; }
        public int GameState { get => gameState; set => gameState = value; }
        public string ServerReply { get => serverReply; }

        public bool ClientIsReset { get; set; }



        /*
         * Function     :   ConfigureClient
         * Description  :   Updates the information needed for the client to communicate with a server.
         * 
         * Parameters   :   string ipa  :   the ip address that the client will attempt to communicate with
         *                  string port :   the port number for the endpoint that the client will attempt to communicate with 
         *                  string name :   the name of the client player
         * Returns      :   None
         */
        public void ConfigureClient(string ipa, string port, string name)
        {
            ipAddressString = ipa;
            portNumberString = port;
            playerName = name;
            ClientIsReset = true;
        }



        /*
         * Function     :   ResetClient
         * Description  :   Resets the client once the player is finished playing.
         * 
         * Parameters   :   None
         * Returns      :   None
         */
        public void ResetClient()
        {
            playerName = "";
            playerID = 0;
            guess = 0;
            gameState = 0;
            ClientIsReset = false;
    }


        /*
         * Function     :   ConnectClient
         * Description  :   Attempts to connect the client to the server.
         * 
         * Parameters   :   None
         * Returns      :   None
         */
        public void ConnectClient()
        {
            //prepare a buffer for the server's response 
            byte[] bytes = new byte[1024];

            //prepare to connect to the server
            try
            {
                //set up the IP address entered by the user
                IPAddress ipAddress = IPAddress.Parse(IPAddressString);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, int.Parse(PortNumberString));

                //create a socket with which to communicate with the server
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    

                //attempt to connect to the server
                try
                {
                    sender.Connect(remoteEP);

                    //create a message and send it to the server
                    byte[] msg = Encoding.ASCII.GetBytes(playerID + "?" + playerName + "?" + guess + "?" + gameState + "?" + "<EOF>");
                    int bytesSent = sender.Send(msg);

                    //wait to recieve a response from the server
                    int bytesRec = sender.Receive(bytes);
                    //store the server's reply
                    serverReply = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    //unless player wants to quit, parse the string sent by the server
                    if (gameState != (int)gameStates.PLAYER_QUIT)
                    {
                        string[] query = serverReply.Split('?');

                        //store the server's response into client variables
                        playerID = int.Parse(query[(int)querySubstrings.PLAYER_ID]);
                        playerName = query[(int)querySubstrings.PLAYER_NAME];
                        min = int.Parse(query[(int)querySubstrings.PLAYER_MIN]);
                        max = int.Parse(query[(int)querySubstrings.PLAYER_MAX]);
                        gameState = int.Parse(query[(int)querySubstrings.PLAYER_GAME_STATE]);
                    }

                    //end communication
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (ArgumentNullException ane)
                {
                    Logger.Log(ane.Message);
                }
                catch (SocketException se)
                {
                    Logger.Log(se.Message);
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message);
                }

            }
            catch (ArgumentNullException ane)
            {
                throw ane;
            }
            catch (SocketException se)
            {
                throw se;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
