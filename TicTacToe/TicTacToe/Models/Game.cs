using System;
using System.Threading.Tasks;
using PCLStorage;
using System.IO;
using System.Diagnostics;

namespace TicTacToe.Models
{
    /// <summary>
    /// Model of Tic-Tac-Toe game
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Game board
        /// </summary>
        public Board board { get; private set; }
        /// <summary>
        /// Array of players
        /// </summary>
        private Player[] players = { new Player("O"), new ComputerPlayer("X") };
        /// <summary>
        /// The human player
        /// </summary>
        public Player humanPlayer { get => players[0]; }
        /// <summary>
        /// The computer player
        /// </summary>
        public ComputerPlayer computerPlayer { get => (ComputerPlayer)players[1]; }
        /// <summary>
        /// Index number of the next player (in the players array) 
        /// </summary>
        private int nextPlayerIndex = 0;
        /// <summary>
        /// The player who has the next turn
        /// </summary>
        public Player nextPlayer { get => players[nextPlayerIndex]; }
        /// <summary>
        /// Count of games played (including any game currently being played)
        /// </summary>
        public int count { get; private set; } = 1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="boardSize">Size of board (number of rows and columns)</param>
        public Game(int boardSize)
        {
            board = new Board(boardSize);
        }

        /// <summary>
        /// Saves the current state to a binary file.
        /// </summary>
        /// <param name="folderName">Name of folder containing the binary file</param>
        /// <param name="fileName">Name of the binary file</param>
        public async void Save(string folderName, string fileName)
        {
            IFolder folder = FileSystem.Current.LocalStorage;
            folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            using (BinaryWriter writer = new BinaryWriter(await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite)))
            {
                // Save board size (as int32)
                writer.Write(board.size);
                // Save board state (as string)
                writer.Write(board.ToString());
                // Save scores (as int32s)
                writer.Write(humanPlayer.score);
                writer.Write(computerPlayer.score);
                // Save next player (as int32)
                writer.Write(nextPlayerIndex);
                // Save game count (as int32)
                writer.Write(count);
            }
        }

        /// <summary>
        /// Loads and sets the state from a binary file.
        /// </summary>
        /// <param name="folderName">Name of folder containing the binary file</param>
        /// <param name="fileName">Name of the binary file</param>
        /// <returns></returns>
        public async Task Load(string folderName, string fileName)
        {
            try
            {
                IFolder folder = FileSystem.Current.LocalStorage;
                folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
                IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                using (BinaryReader reader = new BinaryReader(await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite)))
                {
                    // Read values
                    int boardSize = reader.ReadInt32();
                    string boardState = reader.ReadString();
                    int humanScore = reader.ReadInt32();
                    int computerScore = reader.ReadInt32();
                    int nextGamePlayerIndex = reader.ReadInt32();
                    int gameCount = reader.ReadInt32();

                    // Log those values for debugging purposes
                    Debug.WriteLine($"boardSize: {boardSize}");
                    Debug.WriteLine($"boardState: {boardState}");
                    Debug.WriteLine($"humanScore: {humanScore}");
                    Debug.WriteLine($"computerScore: {computerScore}");
                    Debug.WriteLine($"nextGamePlayerIndex: {nextGamePlayerIndex}");
                    Debug.WriteLine($"gameCount: {gameCount}");

                    // Update state from those values
                    board = new Board(boardSize, boardState);
                    humanPlayer.score = humanScore;
                    computerPlayer.score = computerScore;
                    nextPlayerIndex = nextGamePlayerIndex;
                    count = gameCount;
                }
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"[Load] IOException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Load] Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the next player
        /// </summary>
        public void UpdateNextPlayer()
        {
            // By adding 1 then taking the remainder, this code will still work for any number of players
            nextPlayerIndex = (nextPlayerIndex + 1) % players.Length;
        }

        /// <summary>
        /// Get a player from their mark.
        /// </summary>
        /// <param name="mark">Player's mark</param>
        /// <returns>Player for that mark</returns>
        public Player GetPlayerByMark(string mark)
        {
            foreach (Player player in players)
            {
                if (player.mark == mark)
                {
                    return player;
                }
            }
            throw new Exception($"Could not find a player with mark '{mark}'");
        }

        /// <summary>
        /// Checks if a line of the board is a winning set (all aquares in the
        /// line are marked with the same player's mark).
        /// </summary>
        /// <param name="boardLine">Line of the board (squares making up a row,
        /// column, or diagonal)</param>
        /// <returns>True if the line is a winning set, false otherwise</returns>
        public bool IsWinningSet(string[] boardLine)
        {
            if (string.IsNullOrWhiteSpace(boardLine[0]))
            {
                return false;
            }
            string mark = boardLine[0];
            foreach (string boardSqaure in boardLine)
            {
                if (mark != boardSqaure)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if game is over, and if either player has won.
        /// </summary>
        /// <returns>Tuple with boolean indicating if game is over, and the
        /// winning player (if there is one)</returns>
        public (bool, Player) IsEndOfGame()
        {
            // Check if any lines are winning sets
            foreach (string[] line in board.GetLines())
            {
                if (IsWinningSet(line))
                {
                    return (true, GetPlayerByMark(line[0]));
                }
            }
            // Otherwise there is no winning player. The game can continue if
            // there are any blank sqaures left.
            return (!board.HasBlankSqaures(), null);
        }

        /// <summary>
        /// Plays a move by a player.
        /// </summary>
        /// <param name="player">Player making the move</param>
        /// <param name="square">Row and column of chosen square</param>
        /// <returns>Tuple with boolean indicating if game is over, and the
        /// winning player (if there is one)</returns>
        public (bool, Player) Play(Player player, (int, int) square)
        {
            // The next player will have the next turn
            UpdateNextPlayer();
            // Mark the chosen square with the player's mark
            board.MarkSquare(square, player.mark);
            // Check for the end of game
            (bool isEnd, Player winner) = IsEndOfGame();
            if (isEnd)
            {
                count++;
                if (winner != null) // not a draw
                {
                    winner.AddWinPoint();
                }
                // Note that the board is not reset yet. That's a job for the
                // controller, once it has finished displaying the final board
                // configuration and the end of game message (won, lost, draw).
            }
            return (isEnd, winner);
        }
    }
}
