using System;
using System.Threading.Tasks;
using TicTacToe.Models;
using Xamarin.Forms;

namespace TicTacToe
{
    /// <summary>
    /// Controller for the app
    /// </summary>
    public partial class MainPage : ContentPage
    {
        public string dataFolderName { get; private set; } = "tictactoe";
        public string dataFileName { get; private set; } = "data.bin";

        /// <summary>
        /// 2D array to hold the board buttons. This allows event handlers to find
        /// the sender's position within the board.
        /// </summary>
        private readonly Button[,] boardButtons;

        /// <summary>
        /// Game instance
        /// </summary>
        public Game game { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            int boardSize = 3;
            game = new Game(boardSize);
            boardButtons = new Button[,]
            {
                {btn0_0, btn0_1, btn0_2},
                {btn1_0, btn1_1, btn1_2},
                {btn2_0, btn2_1, btn2_2}
            };
            // Initially disable all button while data is loading
            foreach (Button button in boardButtons)
            {
                button.IsVisible = false;
            }
            LoadData();
        }

        /// <summary>
        /// Load previous game data, if any, and then update the UI.
        /// </summary>
        private async void LoadData()
        {
            // Define a minimal delay so that the load activity indicator is always displayed to user
            Task minDelay = Task.Delay(2000);
            // Wait for at least minimal delay, and until the game data has been loaded 
            await Task.WhenAll(game.Load(dataFolderName, dataFileName), minDelay);
            loadActivityIndicator.IsRunning = false;
            loadActivityIndicator.IsVisible = false;
            UpdateUI();

        }

        /// <summary>
        /// Updates the UI based on the current game state.
        /// </summary>
        private void UpdateUI()
        {
            gameNumberLabel.Text = "Game #" + game.count.ToString();
            bool isComputersTurn = game.nextPlayer.Equals(game.computerPlayer);
            // Update each button based on the corresponding game board square
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    string square = game.board.squares[i, j];
                    Button button = boardButtons[i, j];
                    bool isEnabled;
                    if (isComputersTurn)
                    {
                        isEnabled = false;
                    }
                    else if (string.IsNullOrWhiteSpace(square))
                    {
                        isEnabled = true;
                    }
                    else
                    {
                        isEnabled = false;
                    }
                    button.IsEnabled = isEnabled;
                    button.IsVisible = true;
                    button.Text = string.IsNullOrWhiteSpace(square) ? " " : square;
                }
            }
            // Updating bolding of labels to indicate whose turn it is
            humanPlayerLabel.FontAttributes = isComputersTurn ? FontAttributes.None : FontAttributes.Bold;
            computerPlayerLabel.FontAttributes = isComputersTurn ? FontAttributes.Bold : FontAttributes.None;
            // Update points
            humanPointsNumberLabel.Text = game.humanPlayer.score.ToString();
            humanPointsTextLabel.Text = game.humanPlayer.score == 1 ? "Point" : "Points";
            computerPointsNumberLabel.Text = game.computerPlayer.score.ToString();
            computerPointsTextLabel.Text = game.computerPlayer.score == 1 ? "Point" : "Points";
        }

        /// <summary>
        /// Gets the row and column index of a game board button.
        /// </summary>
        /// <param name="btn">Game board button</param>
        /// <returns>Tuple of row index and column index</returns>
        private (int, int) GetButtonPosition(Button btn)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (btn.Equals(boardButtons[i, j]))
                    {
                        return (i, j);
                    }
                }
            }
            throw new Exception("Button not found");
        }

        /// <summary>
        /// Handles game board button clicks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoardButton_Clicked(object sender, EventArgs e)
        {
            // Handle asynchronosly so delays can be used
            PlayGameAsync((Button)sender);
        }

        /// <summary>
        /// Plays the game based on the button that was clicked.
        /// </summary>
        /// <param name="buttonClicked"></param>
        private async void PlayGameAsync(Button buttonClicked)
        {
            // Play the human's move
            (bool isOver, Player winner) = game.Play(game.humanPlayer, GetButtonPosition(buttonClicked));
            UpdateUI();
            if (!isOver)
            {
                // Play the computer's move
                (isOver, winner) = await PlayComputersMove();
            }
            if (isOver)
            {
                // After a short delay, show the winner or a draw message, and reset for next game 
                await Task.Delay(200);
                await ShowWinnerAndReset(winner);
                //UpdateUI();
            }
        }

        /// <summary>
        /// Play the computer's move, and update the UI after a delay that simulates "thinking time".
        /// </summary>
        /// <returns>Tuple with boolean indicating if game is over, and the winning player (if there is one)</returns>
        public async Task<(bool, Player)> PlayComputersMove()
        {
            (bool, Player) result = game.Play(game.computerPlayer, game.computerPlayer.MakeMove(game.board));
            int simulatedThinkingTime = (new Random()).Next(900, 1300);
            await Task.Delay(simulatedThinkingTime);
            UpdateUI();
            return result;
        }

        /// <summary>
        /// Shows an alert indicating who won, or if it was a draw, and resets for the next game.
        /// </summary>
        /// <param name="winner">Player that won</param>
        /// <returns></returns>
        private async Task ShowWinnerAndReset(Player winner)
        {
            string message;
            if (winner == null)
            {
                message = "It's a draw.";
            }
            else if (game.GetPlayerByMark(winner.mark) == game.humanPlayer)
            {
                message = "You win!";
            }
            else
            {
                message = "You lose :(";
            }
            await DisplayAlert("Game over", message, "OK");
            game.board.Clear();
            UpdateUI();
            // If the next player is the computer, start the next game by playing the computer's move
            if (game.nextPlayer.Equals(game.computerPlayer))
            {
                await PlayComputersMove();
            }
        }
    }
}
