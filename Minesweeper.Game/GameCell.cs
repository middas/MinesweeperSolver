using System;

namespace Minesweeper.Game
{
    public class GameCell
    {
        private bool _isFlagged;
        private bool _isRevealed;

        public GameCell(int x, int y)
        {
            X = x;
            Y = y;
            AdjacentMines = 0;
        }

        public event EventHandler FlagChanged;

        public event EventHandler RevealChanged;

        public short AdjacentMines { get; set; }

        public bool IsFlagged
        {
            get => _isFlagged;

            set
            {
                _isFlagged = value;

                FlagChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsMine { get; set; }

        public bool IsRevealed
        {
            get => _isRevealed;

            set
            {
                _isRevealed = value;

                RevealChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int X { get; private set; }

        public int Y { get; private set; }
    }
}