using Minesweeper.Game;
using System;

namespace Minesweeper
{
    public class CellClickEventArgs : EventArgs
    {
        public CellClickEventArgs(int x, int y, ClickAction clickAction)
        {
            X = x;
            Y = y;
            ClickAction = clickAction;
        }

        public ClickAction ClickAction { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}