using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TicTacToeWPF
{
    public partial class MainWindow : Window
    {
        private GameState _gameState = new();
        private const string SaveFileName = "savegame.json";

        public MainWindow()
        {
            InitializeComponent();
            InitializeGameBoard();
            UpdateBoard();
            ChangeTheme("Light");
        }

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

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var position = button.Tag.ToString().Split(',');
            int row = int.Parse(position[0]);
            int col = int.Parse(position[1]);

            if (!string.IsNullOrEmpty(_gameState.Board[row, col])) return;

            _gameState.Board[row, col] = _gameState.CurrentPlayer;
            button.Content = _gameState.CurrentPlayer;

            if (CheckForWin(row, col))
            {
                MessageBox.Show($"Игрок {_gameState.CurrentPlayer} победил!");
                _gameState.IsGameOver = true;
            }
            else if (CheckForDraw())
            {
                MessageBox.Show("Ничья!");
                _gameState.IsGameOver = true;
            }
            else
            {
                _gameState.CurrentPlayer = _gameState.CurrentPlayer == "X" ? "O" : "X";
            }

            UpdateBoard();
        }

        private bool CheckForWin(int row, int col)
        {
            return CheckRow(row) || CheckColumn(col) || CheckDiagonals();
        }

        private bool CheckRow(int row)
        {
            return _gameState.Board[row, 0] == _gameState.CurrentPlayer &&
                   _gameState.Board[row, 1] == _gameState.CurrentPlayer &&
                   _gameState.Board[row, 2] == _gameState.CurrentPlayer;
        }

        private bool CheckColumn(int col)
        {
            return _gameState.Board[0, col] == _gameState.CurrentPlayer &&
                   _gameState.Board[1, col] == _gameState.CurrentPlayer &&
                   _gameState.Board[2, col] == _gameState.CurrentPlayer;
        }

        private bool CheckDiagonals()
        {
            return (_gameState.Board[0, 0] == _gameState.CurrentPlayer &&
                    _gameState.Board[1, 1] == _gameState.CurrentPlayer &&
                    _gameState.Board[2, 2] == _gameState.CurrentPlayer)
                ||
                   (_gameState.Board[0, 2] == _gameState.CurrentPlayer &&
                    _gameState.Board[1, 1] == _gameState.CurrentPlayer &&
                    _gameState.Board[2, 0] == _gameState.CurrentPlayer);
        }

        private bool CheckForDraw()
        {
            return _gameState.Board.Cast<string>().All(cell => !string.IsNullOrEmpty(cell));
        }


        private void UpdateBoard()
        {
            foreach (Button button in gameBoard.Children)
            {
                var position = button.Tag.ToString().Split(',');
                int row = int.Parse(position[0]);
                int col = int.Parse(position[1]);

                button.Content = _gameState.Board[row, col];
                button.IsEnabled = !_gameState.IsGameOver && string.IsNullOrEmpty(_gameState.Board[row, col]);
            }
            currentPlayerText.Text = $"Текущий игрок: {_gameState.CurrentPlayer}";
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            _gameState = new GameState();
            UpdateBoard();
        }

        private void SaveGame()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_gameState);
                File.WriteAllText(SaveFileName, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void LoadGame()
        {
            try
            {
                if (File.Exists(SaveFileName))
                {
                    string json = File.ReadAllText(SaveFileName);
                    _gameState = JsonConvert.DeserializeObject<GameState>(json);
                    UpdateBoard();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void ChangeTheme(string theme)
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri($"/Themes/{theme}Theme.xaml", UriKind.Relative)
            };
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(dict);
        }

        // Обработчики событий меню
        private void Save_Click(object sender, RoutedEventArgs e) => SaveGame();
        private void Load_Click(object sender, RoutedEventArgs e) => LoadGame();
        private void LightTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Light");
        private void DarkTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Dark");
        private void WinterTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Winter");
        private void SpringTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Spring");
        private void SummerTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Summer");
        private void AutumnTheme_Click(object sender, RoutedEventArgs e) => ChangeTheme("Autumn");
        private void Exit_Click(object sender, RoutedEventArgs e) => Close();
    }
}