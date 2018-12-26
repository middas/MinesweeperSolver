using Minesweeper.Game;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Minesweeper
{
    public class GameCellViewModel : INotifyPropertyChanged
    {
        private readonly GameCell _gameCell;
        private int _adjacentMines;
        private bool _hasNumber;
        private bool _isFlagged;
        private bool _isMine;
        private bool _isRevealed;
        private bool _isWrong;
        private bool _mineVisible;

        public GameCellViewModel(GameCell gameCell)
        {
            _gameCell = gameCell;

            _gameCell.RevealChanged += (s, e) => IsRevealed = ((GameCell)s).IsRevealed;
            _gameCell.FlagChanged += (s, e) => IsFlagged = ((GameCell)s).IsFlagged;
        }

        public event EventHandler<CellClickEventArgs> CellClick;

        public event PropertyChangedEventHandler PropertyChanged;

        public int AdjacentMines
        {
            get => _adjacentMines;

            set
            {
                _adjacentMines = value;

                NotifyPropertyChanged(nameof(AdjacentMines));
            }
        }

        public bool HasNumber
        {
            get => _hasNumber;

            set
            {
                _hasNumber = value;

                NotifyPropertyChanged(nameof(HasNumber));
            }
        }

        public bool IsFlagged
        {
            get => _isFlagged;

            set
            {
                _isFlagged = value;

                NotifyPropertyChanged(nameof(IsFlagged));
            }
        }

        public bool IsMine
        {
            get => _isMine;

            set
            {
                _isMine = value;

                NotifyPropertyChanged(nameof(IsMine));
            }
        }

        public bool IsRevealed
        {
            get => _isRevealed;

            set
            {
                _isRevealed = value;

                NotifyPropertyChanged(nameof(IsRevealed));
                OnIsRevealedChanged();
            }
        }

        public bool IsWrong
        {
            get => _isWrong;

            set
            {
                _isWrong = value;

                NotifyPropertyChanged(nameof(IsWrong));
            }
        }

        public ICommand LeftClickCommand => new RelayCommand(LeftClicked);

        public bool MineVisible
        {
            get => _mineVisible;

            set
            {
                _mineVisible = value;

                NotifyPropertyChanged(nameof(MineVisible));
            }
        }

        public ICommand RightClickCommand => new RelayCommand(RightClicked);

        public void UpdateValues()
        {
            IsFlagged = _gameCell.IsFlagged;
            IsMine = _gameCell.IsMine;
            IsRevealed = _gameCell.IsRevealed;
            AdjacentMines = _gameCell.AdjacentMines;
            HasNumber = _gameCell.AdjacentMines > 0;
        }

        private void LeftClicked()
        {
            CellClick?.Invoke(this, new CellClickEventArgs(_gameCell.X, _gameCell.Y, ClickAction.Reveal));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnIsRevealedChanged()
        {
            if (IsRevealed)
            {
                MineVisible = IsMine;
            }
        }

        private void RightClicked()
        {
            CellClick?.Invoke(this, new CellClickEventArgs(_gameCell.X, _gameCell.Y, ClickAction.Flag));
        }
    }
}