/*
 * File:		GameEngine.cs
 * Project:		HiLoServer
 * By:			Quade Nielsen
 * Date:		November 12, 2021
 * Description:	This file contains the GameEngine class for the Hi Lo Game.  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiLoObjects
{
   /* 
    * Name         :   GameEngine
    * Description  :   This class handles game logic for the Hi-Lo game. The user configures the
    *                  range of guessable numbers and then the engine can be used to start new games
    *                  for players and evaluate their guesses.
    */
    class GameEngine
    {
        private static int hiloMin; //the minimum value in the range of guessable numbers
        private static int hiloMax; //the maximum value in the range of guessable numbers



        /*
         * Function     :   configureRange
         * Description  :   Sets the range of guessable numbers for the High-Low game.
         * 
         * Parameters   :   int min :   the minimum
         *                  int max :   the maximum
         * Returns      :   None
         */
        public static void configureRange(int min, int max)
        {
            hiloMin = min;
            hiloMax = max;
        }



        /*
         * Function     :   startNewGame
         * Description  :   Starts a new HiLo game for a player.
         * 
         * Parameters   :   Player player :   the player that will have a new game started for them
         * Returns      :   None
         */
        public static void startNewGame(Player player)
        {
            player.GameState = (int)gameStates.NO_WIN;
            player.Min = hiloMin;
            player.Max = hiloMax;
            Random random = new Random();
            player.Number = random.Next(hiloMin, hiloMax);
        }



        /*
         * Function     :   evaluateGuess
         * Description  :   Evaluates the guess given by a player.
         * 
         * Parameters   :   Player player :   the player whose guess will be evaluated
         * Returns      :   None
         */
        public static void evaluateGuess(Player player, int playerGuess)
        {
            if (playerGuess == player.Number)
            {
                player.GameState = (int)gameStates.WIN;
            }
            else if (playerGuess < player.Number && playerGuess > player.Min)
            {
                player.Min = playerGuess;
                player.GameState = (int)gameStates.NO_WIN;
            }
            else if (playerGuess > player.Number && playerGuess < player.Max)
            {
                player.Max = playerGuess;
                player.GameState = (int)gameStates.NO_WIN;
            }
            else if (playerGuess < player.Min)
            {
                player.GameState = (int)gameStates.BELOW_MIN;
            }
            else if (playerGuess > player.Max)
            {
                player.GameState = (int)gameStates.ABOVE_MAX;
            }
            else
            {
                player.GameState = (int)gameStates.NO_WIN;
            }
        }

    }
}
