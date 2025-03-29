namespace TicTacToeWPF
{
    // Класс для хранения состояния игры
    public class GameState
    {
        // Игровое поле 3x3, где хранятся значения ячеек ("X", "O" или пусто)
        public string[,] Board { get; set; } = new string[3, 3];
        // Текущий игрок, начинаем с "X"
        public string CurrentPlayer { get; set; } = "X";
        // Флаг завершения игры
        public bool IsGameOver { get; set; }
    }
}
