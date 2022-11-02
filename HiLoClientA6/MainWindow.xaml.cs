/*
 * File:		MainWindow.cxaml.cs
 * Project:		HiLoClient
 * By:			Quade Nielsen
 * Date:		November 12, 2021
 * Description:	This file contains the code-behind for the MainWindow of A-05 HiLoGame client application.
 *              The code contributes to the functionality of a UI for a client which plays a HiLo game with a server.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Net.Sockets;
using System.Windows.Shapes;
using HiLoObjects;

namespace A5HiLoGame
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Client client;


		/*
		 * Function     :   MainWindow
		 * Description  :   The constructor for MainWindow of the A-05 HiLoGame client application.
		 *                  Instantiates a client object and enables the first part of the UI.
		 * 
		 * Parameters   :   None
		 * Returns      :   None
		 */
		public MainWindow()
		{
			InitializeComponent();
			client = new Client();
			toggleConnectionScreen();
		}



		/*
		 * Function     :   btnConnectToServer_Click
		 * Description  :   The event handler for the WPF control which launches the attempt to connect to the server
		 *                  and initiate a Hi Lo game.
		 * 
		 * Parameters   :   object sender       :   the object that invoked the event handler
		 *                  RoutedEventArgs e   :   data associated with the event
		 * Returns      :   None
		 */
		private void btnConnectToServer_Click(object sender, RoutedEventArgs e)
		{
			tblConnectionError.Text = "";
			//validate input
			if (CheckConnectionInput())
			{
				//configure the client using the user input
				client.ConfigureClient(txbIPAddress.Text, txbPortNumber.Text, txbName.Text);

				//attempt to connect to the server
				try
				{
					client.ConnectClient();

					//if successful, move on to the next part of the UI
					toggleConnectionScreen();
					toggleGameScreen();
					lblGuessPrompt.Text = "Hi, " + client.PlayerName + "! Guess a number between " + client.Min + " and " + client.Max + "!";
				}
				catch
				{
				tblConnectionError.Text = "Could not connect to server!";
				}
			}
		}



		/*
		 * Function     :   btnSubmitGuess_Click
		 * Description  :   The event handler for the WPF control which submits a guess to the HiLo server.
		 * 
		 * Parameters   :   object sender       :   the object that invoked the event handler
		 *                  RoutedEventArgs e   :   data associated with the event
		 * Returns      :   None
		 */
		private void btnSubmitGuess_Click(object sender, RoutedEventArgs e)
		{   
			if (CheckGuessInput())
			{
				if (!int.TryParse(txbGuess.Text, out int guess))
				{
					tblGuessError.Text = "Invalid input!";
				}
				else
				{
					//send the user input to the server
					client.Guess = guess;
					try
					{
						client.ConnectClient();
					}
					catch
					{
						tblGuessError.Text = "Could not connect to server!";
					}

					//update the prompt on the UI
					lblGuessPrompt.Text = "Hi, " + client.PlayerName + "! Guess a number between " + client.Min + " and " + client.Max + "!";
					lblGuessTaunt.Text = "";

					//check the game state to see whether the UI needs to be updated
					if (client.GameState == (int)gameStates.WIN)
					{
						lblPlayAgain.Text = "Congratulations, " + client.PlayerName + "!\nWould you like to play again?";
						toggleGameScreen();
						toggleWinScreen();
					}
					else if (client.GameState == (int)gameStates.ABOVE_MAX)
					{
						tblGuessError.Text = "You must enter a number below the maximum!";
					}
					else if (client.GameState == (int)gameStates.BELOW_MIN)
					{
						tblGuessError.Text = "You must enter a number above the minimum!";
					}
					else
					{
						lblGuessTaunt.Text = "Close! But not quite!";
						tblGuessError.Text = "";
					}
				}
			}
			//lblGuessPrompt.Text = "Hi, " + client.PlayerName + "! Guess a number between " + client.Min + " and " + client.Max + "!";
		}



		/*
		 * Function     :   btnPlayAgain_Click
		 * Description  :   The event handler for the WPF control which submits a request to play again to the server.
		 * 
		 * Parameters   :   object sender       :   the object that invoked the event handler
		 *                  RoutedEventArgs e   :   data associated with the event
		 * Returns      :   None
		 */
		private void btnPlayAgain_Click(object sender, RoutedEventArgs e)
		{
			//attempt to call server
			try
			{
				client.ConnectClient();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unexpected exception : " + ex.ToString());
			}

			//update UI
			lblGuessPrompt.Text = "Hi, " + client.PlayerName + "! Guess a number between " + client.Min + " and " + client.Max + "!";
			tblGuessError.Text = "";
			lblGuessTaunt.Text = "";
			toggleWinScreen();
			toggleGameScreen();
		}



		/*
		 * Function     :   btnReturnToConnectScreen_Click
		 * Description  :   The event handler for the WPF control which returns the user to the connection screen.
		 * 
		 * Parameters   :   object sender       :   the object that invoked the event handler
		 *                  RoutedEventArgs e   :   data associated with the event
		 * Returns      :   None
		 */
		private void btnReturnToConnectScreen_Click(object sender, RoutedEventArgs e)
		{
			//reset client and update UI
			client.ResetClient();
			toggleWinScreen();
			toggleConnectionScreen();
		}



		/*
		 * Function     :   CheckConnectionInput
		 * Description  :   Checks the input for the connection screen and provides basic input validation.
		 * 
		 * Parameters   :   None
		 * Returns      :   bool: false if there is invalid input
		 */
		private bool CheckConnectionInput()
		{
			bool errorExists = false;
			tblAddressError.Text = "";
			tblPortError.Text = "";
			tblNameError.Text = "";
			if (txbIPAddress.Text == "")
			{
				tblAddressError.Text = "IP Address must not be blank!";
				errorExists = true;
			}
			if (txbPortNumber.Text == "")
			{
				tblPortError.Text = "Port Number must not be blank!";
				errorExists = true;
			}
			if (txbName.Text == "")
			{
				tblNameError.Text = "Name must not be blank!";
				errorExists = true;
			}
			if (errorExists) return false;
			return true;
		}



		/*
		 * Function     :   CheckGuessInput
		 * Description  :   Checks the input for the game screen and provides basic input validation.
		 * 
		 * Parameters   :   None
		 * Returns      :   bool: false if there is invalid input
		 */
		private bool CheckGuessInput()
		{
			bool errorExists = false;
			tblGuessError.Text = "";
			
			if (txbGuess.Text == "")
			{
				tblGuessError.Text = "Guess must not be blank!";
				errorExists = true;
			}
			
			if (errorExists) return false;
			return true;
		}



		/*
		 * Function     :   toggleConnectionScreen
		 * Description  :   Toggles the visibility and interactibility of the connection screen.
		 * 
		 * Parameters   :   None
		 * Returns      :   None
		 */
		private void toggleConnectionScreen()
		{
			if (ConnectionScreen.IsVisible == true)
			{
				ConnectionScreen.Visibility = Visibility.Hidden;
				ConnectionScreen.IsEnabled = false;
			}
			else
			{
				ConnectionScreen.Visibility = Visibility.Visible;
				ConnectionScreen.IsEnabled = true;
			}
		}



		/*
		 * Function     :   toggleConnectionScreen
		 * Description  :   Toggles the visibility and interactibility of the game screen.
		 * 
		 * Parameters   :   None
		 * Returns      :   None
		 */
		private void toggleGameScreen()
		{
			if (GameScreen.IsVisible == true)
			{
				GameScreen.Visibility = Visibility.Hidden;
				GameScreen.IsEnabled = false;
			}
			else
			{
				GameScreen.Visibility = Visibility.Visible;
				GameScreen.IsEnabled = true;
			}
		}



		/*
		 * Function     :   toggleConnectionScreen
		 * Description  :   Toggles the visibility and interactibility of the win screen.
		 * 
		 * Parameters   :   None
		 * Returns      :   None
		 */
		private void toggleWinScreen()
		{
			if (WinScreen.IsVisible == true)
			{
				WinScreen.Visibility = Visibility.Hidden;
				WinScreen.IsEnabled = false;
			}
			else
			{
				WinScreen.Visibility = Visibility.Visible;
				WinScreen.IsEnabled = true;
			}
		}




		/*
		 * Function     :   Window_Closing
		 * Description  :   Event handler for when the user decides to close the application and quit playing.
		 *                  Closing the application sends a message to the server, which checks whether the user
		 *                  wants to quit before removing them from the server.
		 * 
		 * Parameters   :   object sender       :   the object that invoked the event handler
		 *                  RoutedEventArgs e   :   data associated with the event
		 * Returns      :   None
		 */
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (client.ClientIsReset == true)
			{
				//set gamestate and notify the server that the user would like to quit
				client.GameState = (int)gameStates.PLAYER_QUIT;
				try
				{
					client.ConnectClient();
				}
				catch (Exception ex)
				{
					MessageBox.Show("Unexpected exception : " + ex.ToString());
				}

				//display message from the server
				MessageBoxResult result = MessageBox.Show(client.ServerReply, "HiLo Game", MessageBoxButton.YesNo);

				//decide what action to take
				switch (result)
				{
					case MessageBoxResult.Yes:
						//notify the server that the player will quit
						client.GameState = (int)gameStates.PLAYER_QUIT;
						try
						{
							client.ConnectClient();
						}
						catch (Exception ex)
						{
							MessageBox.Show("Unexpected exception : " + ex.ToString());
						}
						break;
					case MessageBoxResult.No:
						//continue playing
						client.GameState = (int)gameStates.NO_WIN;
						client.ConnectClient();
						e.Cancel = true;
						break;
				}
			}
		}
	}
}

