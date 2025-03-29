using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace TicTacToeWPF
{
    public partial class MainWindow : Window
    {
        // Состояние игры: игровое поле, текущий игрок, флаг завершения игры
        private GameState _gameState = new();
        private const string SaveFileName = "savegame.json";
        // Флаг режима игры: против компьютера или против человека
        private bool isComputerMode = false;
        private Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            InitializeGameBoard(); // Создание кнопок для игрового поля
            UpdateBoard();         // Отображение состояния игрового поля
            ChangeTheme("Light");  // Установка темы по умолчанию
        }

        // Создание кнопок для ячеек игрового поля (3x3)
        private void InitializeGameBoard()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    var button = new Button
                    {
                        Tag = $"{row},{col}",
                        FontSize = 32,
                        Margin = new Thickness(2)
                    };
                    button.Click += Cell_Click;
                    gameBoard.Children.Add(button);
                }
            }
        }

        // Обработка клика по ячейке
        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (_gameState.IsGameOver) return;

            var button = (Button)sender;
            var pos = button.Tag.ToString().Split(',');
            int row = int.Parse(pos[0]);
            int col = int.Parse(pos[1]);

            if (!string.IsNullOrEmpty(_gameState.Board[row, col]))
                return;

            _gameState.Board[row, col] = _gameState.CurrentPlayer;
            button.Content = _gameState.CurrentPlayer;

            if (HasWon(_gameState.CurrentPlayer))
            {
                MessageBox.Show($"Игрок {_gameState.CurrentPlayer} победил!");
                _gameState.IsGameOver = true;
            }
            else if (IsDraw())
            {
                MessageBox.Show("Ничья!");
                _gameState.IsGameOver = true;
            }
            else
            {
                _gameState.CurrentPlayer = _gameState.CurrentPlayer == "X" ? "O" : "X";
            }

            UpdateBoard();

            // Ход компьютера, если выбран соответствующий режим
            if (isComputerMode && _gameState.CurrentPlayer == "O" && !_gameState.IsGameOver)
                ComputerMove();
        }

        // Ход компьютера с выбором оптимального хода посредством алгоритма минимакс
        private void ComputerMove()
        {
            int bestVal = int.MinValue, bestRow = -1, bestCol = -1;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (string.IsNullOrEmpty(_gameState.Board[i, j]))
                    {
                        _gameState.Board[i, j] = "O";
                        int moveVal = Minimax(_gameState.Board, 0, false);
                        _gameState.Board[i, j] = "";
                        if (moveVal > bestVal)
                        {
                            bestVal = moveVal;
                            bestRow = i;
                            bestCol = j;
                        }
                    }
                }
            }

            if (bestRow != -1 && bestCol != -1)
            {
                _gameState.Board[bestRow, bestCol] = "O";
                if (HasWon("O"))
                {
                    UpdateBoard();
                    MessageBox.Show("Компьютер победил!");
                    _gameState.IsGameOver = true;
                }
                else if (IsDraw())
                {
                    UpdateBoard();
                    MessageBox.Show("Ничья!");
                    _gameState.IsGameOver = true;
                }
                else
                {
                    _gameState.CurrentPlayer = "X";
                }
                UpdateBoard();
            }
        }

        // Оценка текущего состояния поля: +10 для победы "O", -10 для победы "X"
        private int EvaluateBoard(string[,] board)
        {
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(board[i, 0]) && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                    return board[i, 0] == "O" ? +10 : -10;
                if (!string.IsNullOrEmpty(board[0, i]) && board[0, i] == board[1, i] && board[1, i] == board[2, i])
                    return board[0, i] == "O" ? +10 : -10;
            }
            if (!string.IsNullOrEmpty(board[0, 0]) && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                return board[0, 0] == "O" ? +10 : -10;
            if (!string.IsNullOrEmpty(board[0, 2]) && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                return board[0, 2] == "O" ? +10 : -10;
            return 0;
        }

        // Проверка наличия свободных ячеек
        private bool IsMovesLeft(string[,] board)
        {
            foreach (var cell in board)
                if (string.IsNullOrEmpty(cell))
                    return true;
            return false;
        }

        // Алгоритм минимакс для поиска оптимального хода
        private int Minimax(string[,] board, int depth, bool isMaximizing)
        {
            int score = EvaluateBoard(board);
            if (score == 10 || score == -10)
                return score;
            if (!IsMovesLeft(board))
                return 0;

            if (isMaximizing)
            {
                int best = int.MinValue;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (string.IsNullOrEmpty(board[i, j]))
                        {
                            board[i, j] = "O";
                            best = Math.Max(best, Minimax(board, depth + 1, false));
                            board[i, j] = "";
                        }
                    }
                }
                return best;
            }
            else
            {
                int best = int.MaxValue;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (string.IsNullOrEmpty(board[i, j]))
                        {
                            board[i, j] = "X";
                            best = Math.Min(best, Minimax(board, depth + 1, true));
                            board[i, j] = "";
                        }
                    }
                }
                return best;
            }
        }

        // Проверка победы для указанного игрока
        private bool HasWon(string player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_gameState.Board[i, 0] == player &&
                    _gameState.Board[i, 1] == player &&
                    _gameState.Board[i, 2] == player)
                    return true;
                if (_gameState.Board[0, i] == player &&
                    _gameState.Board[1, i] == player &&
                    _gameState.Board[2, i] == player)
                    return true;
            }
            return (_gameState.Board[0, 0] == player &&
                    _gameState.Board[1, 1] == player &&
                    _gameState.Board[2, 2] == player)
                || (_gameState.Board[0, 2] == player &&
                    _gameState.Board[1, 1] == player &&
                    _gameState.Board[2, 0] == player);
        }

        // Проверка на ничью: если все ячейки заполнены
        private bool IsDraw() =>
            _gameState.Board.Cast<string>().All(cell => !string.IsNullOrEmpty(cell));

        // Обновление визуального состояния игрового поля и информации о текущем игроке
        private void UpdateBoard()
        {
            foreach (Button button in gameBoard.Children)
            {
                var pos = button.Tag.ToString().Split(',');
                int row = int.Parse(pos[0]);
                int col = int.Parse(pos[1]);
                button.Content = _gameState.Board[row, col];
                button.IsEnabled = !_gameState.IsGameOver && string.IsNullOrEmpty(_gameState.Board[row, col]);
            }
            currentPlayerText.Text = $"Текущий игрок: {_gameState.CurrentPlayer}";
        }

        // Сброс игры для начала новой партии
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            _gameState = new GameState();
            UpdateBoard();
        }

        // Сохранение текущего состояния игры в файл
        private void SaveGame()
        {
            try
            {
                File.WriteAllText(SaveFileName, JsonConvert.SerializeObject(_gameState));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        // Загрузка сохранённого состояния игры
        private void LoadGame()
        {
            try
            {
                if (File.Exists(SaveFileName))
                {
                    _gameState = JsonConvert.DeserializeObject<GameState>(File.ReadAllText(SaveFileName));
                    UpdateBoard();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        // Изменение темы приложения
        private void ChangeTheme(string theme)
        {
            var dict = new ResourceDictionary { Source = new Uri($"/Themes/{theme}Theme.xaml", UriKind.Relative) };
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(dict);
        }

        // Привязка методов к элементам меню
        private void Save_Click(object sender, RoutedEventArgs e) => SaveGame();
        private void Load_Click(object sender, RoutedEventArgs e) => LoadGame();
        private void LightTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Light");
        private void DarkTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Dark");
        private void WinterTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Winter");
        private void SpringTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Spring");
        private void SummerTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Summer");
        private void AutumnTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Autumn");
        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        // Переключение в режим игры против человека
        private void HumanMode_Click(object sender, RoutedEventArgs e)
        {
            isComputerMode = false;
            NewGame_Click(sender, e);
            MessageBox.Show("Режим: Против человека");
        }

        // Переключение в режим игры против компьютера
        private void ComputerMode_Click(object sender, RoutedEventArgs e)
        {
            isComputerMode = true;
            NewGame_Click(sender, e);
            MessageBox.Show("Режим: Против компьютера");
        }
    }
}
