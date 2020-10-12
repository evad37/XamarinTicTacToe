using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    /// <summary>
    /// Model of Tic-Tac-Toe player
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Number of players created
        /// </summary>
        protected static int count = 0;
        /// <summary>
        /// Unique id for the player
        /// </summary>
        public readonly int id = count++;
        /// <summary>
        /// Number of points (wins)
        /// </summary>
        public int score { get; set; } = 0;
        /// <summary>
        /// Mark to use on board
        /// </summary>
        public string mark { get; protected set; }

        public Player(string mark)
        {
            this.mark = mark;
        }

        /// <summary>
        /// Adds on a point to the score for winning.
        /// </summary>
        public void AddWinPoint()
        {
            score++;
        }

        /// <summary>
        /// Test if another Player object is the same as this Player object. 
        /// </summary>
        /// <param name="player">Other player</param>
        /// <returns>True if there are the same, false otherwise</returns>
        public bool Equals(Player player)
        {
            return player.id == this.id;
        }
    }
}
