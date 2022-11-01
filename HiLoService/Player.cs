/*
 * File:		Player.cs
 * Project:		HiLoServer
 * By:			Quade Nielsen
 * Date:		November 12, 2021
 * Description:	This file contains the Player class for the Hi Lo Game. It also contains the enum gameStates
 *              which is accessed through the HiLoObjects namespace.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiLoObjects
{
    public enum gameStates //game states for the Hi Lo Game
    {
        NO_WIN, BELOW_MIN, ABOVE_MAX, WIN, PLAYER_QUIT
    }

   /* 
    * Name         :   Player
    * Description  :   This class models the player in the Hi-Lo Game. The server uses this class to keep track
    *                  of game variables and states for the user.
    */
    class Player
    {
        private int playerID; //the unique ID of the player
        private string name; //the player's name
        private int min; //the current min of the player's guessable range
        private int max; //the current max of the player's guessable range
        private int number; //the number chosen by the server that the player is trying to guess
        private int gameState; //the current state of the player's game


        /*
         * Function     :   Player
         * Description  :   The constructor for Player class. The player requires a name. Then the object
         *                  must be passed to the GameEngine in order to start a new game.
         * 
         * Parameters   :   string name : the player's new name
         * Returns      :   None
         */
        public Player(string name)
        {
            playerID = 0;
            this.name = name;
            min = 1;
            max = 1;
            number = 1;
            gameState = 0;
        }

        public int PlayerID { get => playerID; set => playerID = value; }
        public string Name { get => name; set => name = value; }
        public int Min { get => min; set => min = value; }
        public int Max { get => max; set => max = value; }
        public int Number { get => number; set => number = value; }
        public int GameState { get => gameState; set => gameState = value; }
    }
}
