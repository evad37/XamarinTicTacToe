using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe.Models
{
    /// <summary>
    /// Represents the Tic-Tac-Toe board
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Size of the board (number of rows and columns)
        /// </summary>
        public int size { get; private set; }
        /// <summary>
        /// The squares of the board. If a square has been marked by a player,
        /// it will be set to that player's mark. Otherwise, it will be empty.
        /// </summary>
        public string[,] squares { get; private set; }

        /// <summary>
        /// Constructor for balnk board
        /// </summary>
        /// <param name="size">Size of the board (number of rows and columns)</param>
        public Board(int size)
        {
            this.size = size;
            this.squares = new string[size, size];
        }

        /// <summary>
        /// Constructor that restores a previous state
        /// </summary>
        /// <param name="size">Size of the board (number of rows and columns)</param>
        /// <param name="state">Prior state of the board (from the <see cref="ToString"/> method)</param>
        public Board(int size, string state) : this(size)
        {
            //this.size = size;
            //this.squares = new string[size, size];

            for (int i = 0; i < state.Length; i++)
            {
                int row = i / size;
                int col = i % size;
                string mark = state.Substring(i, 1);
                if (!String.IsNullOrWhiteSpace(mark))
                {
                    squares[row, col] = mark;
                }
            }
        }

        /// <summary>
        /// Gets a row of the board as a 1-D array 
        /// </summary>
        /// <param name="rowIndex">Index of row</param>
        /// <returns>Row</returns>
        public string[] GetRow(int rowIndex)
        {
            string[] row = new string[size];
            for (int i = 0; i < size; i++)
            {
                row[i] = squares[rowIndex, i];
            }
            return row;
        }
        /// <summary>
        /// Gets a column of the board as a 1-D array 
        /// </summary>
        /// <param name="colIndex">Index of column</param>
        /// <returns>Column</returns>
        public string[] GetCol(int colIndex)
        {
            string[] col = new string[size];
            for (int i = 0; i < size; i++)
            {
                col[i] = squares[i, colIndex];
            }
            return col;
        }

        /// <summary>
        /// Gets a diagonal of the board as a 1-D array 
        /// </summary>
        /// <param name="isReverseDiagnal">Set to true to get the 'bottom-left to top-right' diagonal, or false to get the 'top-left to bottom-right' diagonal</param>
        /// <returns>Diagonal</returns>
        public string[] GetDiagonal(bool isReverseDiagnal)
        {
            string[] diag = new string[size];
            for (int i = 0; i < size; i++)
            {
                int row = isReverseDiagnal ? size - 1 - i : i;
                diag[i] = squares[row, i];
            }
            return diag;
        }

        /// <summary>
        /// Gets a list of all the lines (rows, columns, and complete diagonals) on the board.
        /// </summary>
        /// <returns>Lines on the board</returns>
        public List<string[]> GetLines()
        {
            List<string[]> lines = new List<string[]>();
            // Loop through each row and column
            for (int i = 0; i < size; i++)
            {
                lines.Add(GetRow(i));
                lines.Add(GetCol(i));
            }
            // Add the two diagonals
            lines.Add(GetDiagonal(true));
            lines.Add(GetDiagonal(false));
            return lines;
        }

        /// <summary>
        /// Checks if there are any blank squares on the board.
        /// </summary>
        /// <returns>True if the are blank squares, false if all squares are occupied</returns>
        public bool HasBlankSqaures()
        {
            for (int i=0; i<size; i++)
            {
                for (int j=0; j<size; j++)
                {
                    if (string.IsNullOrWhiteSpace(squares[i,j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Clears all marks from the board.
        /// </summary>
        public void Clear()
        {
            squares = new string[size, size];
        }

        /// <summary>
        /// Checks if a square can be marke (is not already occupied)
        /// </summary>
        /// <param name="row">Row index of square to check</param>
        /// <param name="col">Column index of square to check</param>
        /// <returns>True if it can be marked, false if already occupied</returns>
        public bool CanMarkSquare(int row, int col)
        {
            return string.IsNullOrWhiteSpace(squares[row, col]);
        }

        /// <summary>
        /// Marks a sqaure. Throws an exception if the square is already occupied.
        /// </summary>
        /// <param name="sqaure">Row and column index of selected square</param>
        /// <param name="mark">Mark to place on square</param>
        public void MarkSquare((int,int) sqaure, string mark)
        {
            if (!CanMarkSquare(sqaure.Item1, sqaure.Item2))
            {
                throw new Exception($"The square at {sqaure.Item1},{sqaure.Item2} is already occupied");
            }
            squares[sqaure.Item1, sqaure.Item2] = mark;
        }

        /// <summary>
        /// Converts the current board state to a string.
        /// </summary>
        /// <returns>String representation of the current board state</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    sb.Append(string.IsNullOrWhiteSpace(squares[i, j]) ? " " : squares[i, j]);
                }
            }
            return sb.ToString();
        }

    }
}