using Newtonsoft.Json;

namespace TicTacToeWPF
{
    public class GameState
    {
        public string[,] Board { get; set; } = new string[3, 3];
        public string PlayerSymbol { get; set; } = "X"; // Новое свойство
        public string CurrentPlayer { get; set; }
        public bool IsGameOver { get; set; }

        // Вычисляемый символ компьютера
        [JsonIgnore]
        public string ComputerSymbol => PlayerSymbol == "X" ? "O" : "X";
    }
}