using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Game
{
    public class GameBoard : IEnumerable<GameCell>
    {
        private GameCell[,] _Grid;

        public int MineCount { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public GameCell this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= X)
                {
                    throw new ArgumentException("Invalid value", nameof(x));
                }

                if (y < 0 || y >= Y)
                {
                    throw new ArgumentException("Invalid value", nameof(y));
                }

                return _Grid[x, y];
            }
        }

        public void AddMines((int x, int y) ignoredCell)
        {
            Random random = new Random();
            List<(int, int)> cells = new List<(int, int)>();
            for (int x = 0; x < X; x++)
            {
                for (int y = 0; y < Y; y++)
                {
                    if (ignoredCell.x != x && ignoredCell.y != y)
                    {
                        cells.Add((x, y));
                    }
                }
            }

            for (int i = 0; i < MineCount; i++)
            {
                int index = random.Next(0, cells.Count);
                var (x, y) = cells[index];

                _Grid[x, y].IsMine = true;

                cells.RemoveAt(index);
            }

            CalculateAdjacentMineValues();
        }

        public IEnumerator<GameCell> GetEnumerator()
        {
            return _Grid.Cast<GameCell>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Grid.GetEnumerator();
        }

        public void PopulateBoard(int x, int y, int mineCount)
        {
            X = x;
            Y = y;
            MineCount = mineCount;

            _Grid = new GameCell[x, y];

            FillGrid();
        }

        private void CalculateAdjacentMineValues()
        {
            for (int x = 0; x < X; x++)
            {
                for (int y = 0; y < Y; y++)
                {
                    if (_Grid[x, y].IsMine)
                    {
                        // increment all adjacent cell counts
                        for (int ax = Math.Max(0, x - 1); ax <= Math.Min(X - 1, x + 1); ax++)
                        {
                            for (int ay = Math.Max(0, y - 1); ay <= Math.Min(Y - 1, y + 1); ay++)
                            {
                                _Grid[ax, ay].AdjacentMines++;
                            }
                        }
                    }
                }
            }
        }

        private void FillGrid()
        {
            for (int x = 0; x < X; x++)
            {
                for (int y = 0; y < Y; y++)
                {
                    _Grid[x, y] = new GameCell(x, y);
                }
            }
        }
    }
}