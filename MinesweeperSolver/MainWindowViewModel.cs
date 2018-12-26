﻿using Minesweeper.Game;
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
            Width = x * 20 + 45;
            Height = y * 20 + 144;
        }

        private void MarkWrongCells()
        {
            foreach (var gc in GameCells.Where(c => !c.IsMine && c.IsFlagged).ToArray())
            {
                gc.IsWrong = true;
            }
        }

        private void MinesPlaced(object sender, EventArgs e)
        {
            foreach (var c in GameCells)
            {
                c.UpdateValues();
            }
        }

        private void NewGame()
        {
            if (_minesweeperGame != null)
            {
                _minesweeperGame.MinesPlaced -= MinesPlaced;
            }

            StopTimer();

            _secondsElapsed = 0;
            Time = "000";

            _minesweeperGame = new Game.Minesweeper();
            _minesweeperGame.MinesPlaced += MinesPlaced;
            _minesweeperGame.NewGame(Expert.x, Expert.y, Expert.mines);

            GameCells = new ObservableCollection<GameCellViewModel>(_minesweeperGame.GameBoard.OrderBy(c => c.X).ThenBy(c => c.Y).Select(c => new GameCellViewModel(c)));

            foreach (var c in GameCells)
            {
                c.CellClick += (s, e) =>
                {
                    if (GameStatus == GameStatus.New)
                    {
                        _timerCancellationSource = new CancellationTokenSource();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        StartTimer(_timerCancellationSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }

                    _minesweeperGame.BoardClick(e.X, e.Y, e.ClickAction);

                    if (e.ClickAction == ClickAction.Flag)
                    {
                        MineCount = _minesweeperGame.RemainingMines.ToString("000");
                    }

                    GameStatus = _minesweeperGame.GameStatus;
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

                MarkWrongCells();
            }
        }

        private async Task StartTimer(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    _secondsElapsed++;
                    Time = _secondsElapsed.ToString("000");

                    await Task.Delay(1000, cancellationToken);
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