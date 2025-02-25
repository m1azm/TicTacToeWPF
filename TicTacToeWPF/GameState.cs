namespace TicTacToeWPF
{
    public class GameState
    {
        public string[,] Board { get; set; } = new string[3, 3];
        public string CurrentPlayer { get; set; } = "X";
        public bool IsGameOver { get; set; }
    }
}