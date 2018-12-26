using Minesweeper.Game;
using Minesweeper.Solver;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Minesweeper
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private static readonly (int x, int y, int mines) Easy = (8, 8, 10);
        private static readonly (int x, int y, int mines) Expert = (24, 24, 99);

        private ObservableCollection<GameCellViewModel> _gameCells;
        private GameStatus _gameStatus;
        private int _height;
        private string _mineCount;
        private Game.Minesweeper _minesweeperGame;
        private int _secondsElapsed = 0;
        private string _time;
        private CancellationTokenSource _timerCancellationSource;
        private int _width;

        public MainWindowViewModel()
        {
            MineCount = "000";
            Time = "000";

            NewGame();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<GameCellViewModel> GameCells
        {
            get => _gameCells;

            set
            {
                _gameCells = value;

                NotifyPropertyChanged(nameof(GameCells));
            }
        }

        public GameStatus GameStatus
        {
            get => _gameStatus;

            set
            {
                _gameStatus = value;

                NotifyPropertyChanged(nameof(GameStatus));
                OnGameStatusChanged();
            }
        }

        public int Height
        {
            get => _height;

            set
            {
                _height = value;

                NotifyPropertyChanged(nameof(Height));
            }
        }

        public string MineCount
        {
            get => _mineCount;

            set
            {
                _mineCount = value;

                NotifyPropertyChanged(nameof(MineCount));
            }
        }

        public ICommand NewGameCommand => new RelayCommand(NewGame);

        public ICommand SolveCommand => new RelayCommand(Solve);

        public string Time
        {
            get => _time;

            set
            {
                _time = value;

                NotifyPropertyChanged(nameof(Time));
            }
        }

        public int Width
        {
            get => _width;

            set
            {
                _width = value;

                NotifyPropertyChanged(nameof(Width));
            }
        }

        private void AdjustWindowSize(int x, int y)
        {
            Width = x * 20 + 46;
            Height = y * 20 + 145;
        }

        private void MarkWrongCells()
        {
            foreach (var gc in GameCells.Where(c => !c.IsMine && c.IsFlagged).ToArray())
            {
                gc.IsWrong = true;
            }
        }

        private void NewGame()
        {
            if (_minesweeperGame != null)
            {
                _minesweeperGame.Dispose();
            }

            StopTimer();

            _secondsElapsed = 0;
            Time = "000";

            _minesweeperGame = new Game.Minesweeper();
            _minesweeperGame.MinesPlaced += (s, e) =>
            {
                foreach (var c in GameCells)
                {
                    c.UpdateValues();
                }
            };
            _minesweeperGame.GameStatusChanged += (s, e) => GameStatus = ((Game.Minesweeper)s).GameStatus;
            _minesweeperGame.RemainingMinesChanged += (s, e) => MineCount = ((Game.Minesweeper)s).RemainingMines.ToString("000");
            _minesweeperGame.NewGame(Expert.x, Expert.y, Expert.mines);

            GameCells = new ObservableCollection<GameCellViewModel>(_minesweeperGame.GameBoard.OrderBy(c => c.X).ThenBy(c => c.Y).Select(c => new GameCellViewModel(c)));

            foreach (var c in GameCells)
            {
                c.CellClick += (s, e) =>
                {
                    if (GameStatus == GameStatus.New)
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        StartTimer();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }

                    _minesweeperGame.BoardClick(e.X, e.Y, e.ClickAction);
                };
            }

            MineCount = _minesweeperGame.RemainingMines.ToString("000");
            GameStatus = _minesweeperGame.GameStatus;

            AdjustWindowSize(_minesweeperGame.GameBoard.X, _minesweeperGame.GameBoard.Y);
        }

        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void OnGameStatusChanged()
        {
            if (_gameStatus != GameStatus.Playing)
            {
                StopTimer();

                if (_gameStatus == GameStatus.Loss)
                {
                    var mineCells = _minesweeperGame.GetMineLocations();

                    foreach (var (x, y) in mineCells)
                    {
                        var cell = GameCells.Single(gc => gc.X == x && gc.Y == y);
                        cell.IsMine = true;
                        cell.MineVisible = !cell.IsFlagged;
                    }

                    MarkWrongCells();
                }
            }
        }

        private void Solve()
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            StartTimer();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            ISolver solver = new NormalSolver();
            solver.Solve(_minesweeperGame);
        }

        private async Task StartTimer()
        {
            _timerCancellationSource = new CancellationTokenSource();

            try
            {
                while (true)
                {
                    await Task.Delay(1000, _timerCancellationSource.Token);

                    _secondsElapsed++;
                    Time = _secondsElapsed.ToString("000");
                }
            }
            catch { }
        }

        private void StopTimer()
        {
            _timerCancellationSource?.Cancel();
            _timerCancellationSource = null;
        }
    }
}