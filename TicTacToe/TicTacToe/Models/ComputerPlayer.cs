using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToe.Models;
using Xamarin.Forms.Internals;

namespace TicTacToe
{
    /// <summary>
    /// Model of an automated (computer) Tic-Tac-Toe player 
    /// </summary>
    public class ComputerPlayer : Player
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ComputerPlayer(string mark) : base(mark) { }

        /// <summary>
        /// Checks if two marks are both the computer player's mark
        /// </summary>
        /// <param name="first">First mark</param>
        /// <param name="second">Second mark</param>
        /// <returns>True if both marks are the computer player's mark, false otherwise</returns>
        private bool BothComputerPlayerMarks(string first, string second)
        {
            return first == mark && second == mark;
        }

        /// <summary>
        ///  Checks if two marks are both another player's mark
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>True if both marks are the another player's mark, false otherwise</returns>
        private bool BothOtherPlayerMarks(string first, string second)
        {
            return first == second && first != mark && !string.IsNullOrEmpty(first);
        }

        /// <summary>
        /// Checks if any squares in a set have the computer player's mark
        /// </summary>
        /// <param name="squares">Set of sqaures</param>
        /// <returns>True if any have the computer player's mark, false otherwise</returns>
        private bool AnyComputerMarks(string[] squares)
        {
            return squares.Any(squares => squares == mark);
        }

        /// <summary>
        /// Gets the next index number, looping back to 0 if the limit is
        /// reached.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private int NextIndex(int current, int limit)
        {
            return (current + 1) % limit;
        }

        /// <summary>
        /// Gets the previous index number, looping back to (limit - 1) if the
        /// -1 is reached.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private int PrevIndex(int current, int limit)
        {
            return (current + limit - 1) % limit;
        }

        /// <summary>
        /// Returns the index of the winning space within the line, if there is one 
        /// </summary>
        /// <param name="line">Line of squares in a row, column, or diagonal</param>
        /// <returns>Index of the winning square</returns>
        private int? WinIndex(string[] line)
        {
            int numberOfComputerPlayerMarks = line.Where(square => square == mark).Count();
            int numberOfEmptySquares = line.Where(square => string.IsNullOrEmpty(square)).Count();
            if (numberOfComputerPlayerMarks == line.Length - 1 && numberOfEmptySquares == 1)
            {
                return line.IndexOf(sqaure => string.IsNullOrEmpty(sqaure));
            }
            return null;
        }

        /// <summary>
        /// Returns the index of the blocking space within the line, if there is one 
        /// </summary>
        /// <param name="line">Line of squares in a row, column, or diagonal</param>
        /// <returns>Index of the winning square</returns>
        private int? BlockIndex(string[] line)
        {
            IEnumerable<string> otherPlayerMarks = line.Where(square => !string.IsNullOrEmpty(square) && square != mark);
            int numberOfOtherPlayerMarks = otherPlayerMarks.Count();
            bool allSameMark = otherPlayerMarks.All(square => square == otherPlayerMarks.ToArray()[0]);
            int numberOfEmptySquares = line.Where(square => string.IsNullOrEmpty(square)).Count();

            if (numberOfOtherPlayerMarks == line.Length - 1 && allSameMark && numberOfEmptySquares == 1)
            {
                return line.IndexOf(sqaure => string.IsNullOrEmpty(sqaure));
            }
            return null;
        }


        /// <summary>
        /// Gets the square that gives the winning move, if there is one.
        /// </summary>
        /// <param name="board">Current game board</param>
        /// <returns>row index and column index of the chosen square</returns>
        private (int, int)? GetWinningMove(Board board)
        {
            // Check each row and column
            for (int i = 0; i < board.size; i++)
            {
                // Check row
                int? winningSquareColumn = WinIndex(board.GetRow(i));
                if (winningSquareColumn.HasValue)
                {
                    return (i, winningSquareColumn.Value);
                }
                // Check column
                int? winningSquareRow = WinIndex(board.GetCol(i));
                if (winningSquareRow.HasValue)
                {
                    return (winningSquareRow.Value, i);
                }
            }
            // Check top-left to bottom-right diagonal
            int? winningSqaureIndex = WinIndex(board.GetDiagonal(false));
            if (winningSqaureIndex.HasValue)
            {
                return (winningSqaureIndex.Value, winningSqaureIndex.Value);
            }
            // Check bottom-left to top-right diagonal
            winningSqaureIndex = WinIndex(board.GetDiagonal(true));
            if (winningSqaureIndex.HasValue)
            {
                int row = board.size - 1 - winningSqaureIndex.Value;
                int col = winningSqaureIndex.Value;
                return (row, col);
            }
            return null;
        }


        /// <summary>
        /// Gets the square that blocks another player from winning, if there is one.
        /// </summary>
        /// <param name="board">Current game board</param>
        /// <returns>row index and column index of the chosen square</returns>
        private (int, int)? GetBlockingMove(Board board)
        {
            // Check each row and column
            for (int i = 0; i < board.size; i++)
            {
                // Check row
                int? blockingSquareColumn = BlockIndex(board.GetRow(i));
                if (blockingSquareColumn.HasValue)
                {
                    return (i, blockingSquareColumn.Value);
                }
                // Check column
                int? blockingSquareRow = BlockIndex(board.GetCol(i));
                if (blockingSquareRow.HasValue)
                {
                    return (blockingSquareRow.Value, i);
                }
            }
            // Check top-left to bottom-right diagonal
            int? blockingSqaureIndex = BlockIndex(board.GetDiagonal(false));
            if (blockingSqaureIndex.HasValue)
            {
                return (blockingSqaureIndex.Value, blockingSqaureIndex.Value);
            }
            // Check bottom-left to top-right diagonal
            blockingSqaureIndex = BlockIndex(board.GetDiagonal(true));
            if (blockingSqaureIndex.HasValue)
            {
                int row = board.size - 1 - blockingSqaureIndex.Value;
                int col = blockingSqaureIndex.Value;
                return (row, col);
            }
            return null;
        }

        /// <summary>
        /// Gets the square that is adjacent to another one of this computer player's marks, if there is one.
        /// </summary>
        /// <param name="board">Current game board</param>
        /// <returns>row index and column index of the chosen square</returns>
        private (int, int)? GetAdjacentMove(Board board)
        {
            for (int i = 0; i < board.size; i++)
            {
                for (int j = 0; j < board.size; j++)
                {
                    if (board.CanMarkSquare(i, j))
                    {
                        // Get all potential adjacent positions, including
                        // invalid positions (row/column is -1 or board.size)
                        int nextRow = i + 1;
                        int prevRow = i - 1;
                        int nextCol = j + 1;
                        int prevCol = j - 1;
                        (int, int)[] potentialAdjacentPositions = {
                            (prevRow, prevCol), (prevRow, j), (prevRow, nextCol),
                            (i, prevCol), (i, nextCol),
                            (nextRow, prevCol), (nextRow, j), (nextRow, nextCol)
                        };
                        // Filter out invalid positions to get the actual adjacent positions
                        List<(int, int)> adjacentPositions = new List<(int, int)>(potentialAdjacentPositions.Where(position =>
                             0 <= position.Item1 && position.Item1 < board.size &&
                             0 <= position.Item2 && position.Item2 < board.size
                        ));
                        // Get the board squares for those positions
                        string[] adjacentSquares = adjacentPositions.Select(position => board.squares[position.Item1, position.Item2]).ToArray();
                        // Check if any neighbour has this computer player's mark
                        if (AnyComputerMarks(adjacentSquares))
                        {
                            return (i, j);
                        }
                    }
                }

            }
            return null;
        }

        /// <summary>
        /// Gets a random square that is not yet occupied.
        /// </summary>
        /// <param name="board">Current game board</param>
        /// <returns>row index and column index of the chosen square</returns>
        private (int, int) GetRandomMove(Board board)
        {
            Random random = new Random();
            int row;
            int col;
            do
            {
                row = random.Next(board.size);
                col = random.Next(board.size);
            } while (!board.CanMarkSquare(row, col));
            return (row, col);
        }

        /// <summary>
        /// Chooses the computer player's next square, based on the current
        /// state of the board.
        /// </summary>
        /// <param name="board">Current game board</param>
        /// <returns>Tuple of the row index and column index of the chosen square</returns>
        public (int, int) MakeMove(Board board)
        {
            // Make a winning move, if possible
            (int, int)? winningMove = GetWinningMove(board);
            if (winningMove.HasValue)
            {
                return winningMove.Value;
            }

            // Block another player from winning, if possible
            (int, int)? blockingMove = GetBlockingMove(board);
            if (blockingMove.HasValue)
            {
                return blockingMove.Value;
            }

            // If the centre is clear, take that space.
            int center = board.size / 2;
            if (board.CanMarkSquare(center, center))
            {
                return (center, center);
            }

            // If there is a space next to another mark of this computer player, take that space.
            (int, int)? adjacentMove = GetAdjacentMove(board);
            if (adjacentMove.HasValue)
            {
                return adjacentMove.Value;
            }

            // Otherwise, it will take a random space that is not yet occupied.
            return GetRandomMove(board);
        }
    }
}
