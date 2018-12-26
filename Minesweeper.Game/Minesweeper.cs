using System;
using System.Linq;

namespace Minesweeper.Game
{
    public enum ClickAction
    {
        Reveal,
        Flag
    }

    public enum GameStatus
    {
        New,
        Playing,
        Win,
        Loss
    }

    public class Minesweeper : IDisposable
    {
        private bool _firstClick;
        private GameStatus _gameStatus;
        private int _remainingMines;

        public Minesweeper()
        {
            GameBoard = new GameBoard();
            GameStatus = GameStatus.New;
        }

        public event EventHandler GameStatusChanged;

        public event EventHandler MinesPlaced;

        public event EventHandler RemainingMinesChanged;

        public GameBoard GameBoard { get; private set; }

        public GameStatus GameStatus
        {
            get => _gameStatus;

            private set
            {
                _gameStatus = value;

                GameStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int RemainingMines
        {
            get => _remainingMines; private set

            {
                _remainingMines = value;

                RemainingMinesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void BoardClick(int x, int y, ClickAction clickAction)
        {
            if (GameStatus == GameStatus.Playing || GameStatus == GameStatus.New)
            {
                var cell = GameBoard[x, y];

                if (_firstClick)
                {
                    _firstClick = false;
                    GameStatus = GameStatus.Playing;
                    GameBoard.AddMines((x, y));
                    MinesPlaced?.Invoke(this, EventArgs.Empty);
                }

                if (clickAction == ClickAction.Flag)
                {
                    if (cell.IsFlagged)
                    {
                        cell.IsFlagged = false;
                        RemainingMines++;
                    }
                    else if (RemainingMines > 0)
                    {
                        cell.IsFlagged = true;
                        RemainingMines--;
                    }
                }
                else
                {
                    if (!cell.IsFlagged)
                    {
                        if (cell.IsMine)
                        {
                            GameStatus = GameStatus.Loss;

                            RevealMines();
                        }
                        else
                        {
                            RevealCell(cell);

                            if (!GameBoard.Any(c => !c.IsMine && !c.IsRevealed))
                            {
                                GameStatus = GameStatus.Win;
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            GameStatusChanged = null;
            MinesPlaced = null;
            RemainingMinesChanged = null;
            GameBoard = null;
        }

        public (int x, int y)[] GetMineLocations()
        {
            if (GameStatus != GameStatus.Loss)
            {
                throw new Exception("Cannot get mine locations until the game is lost.");
            }

            return GameBoard.Where(gc => gc.IsMine).Select(gc => (gc.X, gc.Y)).ToArray();
        }

        public void NewGame(int x, int y, int mineCount)
        {
            GameBoard.PopulateBoard(x, y, mineCount);
            _firstClick = true;
            RemainingMines = mineCount;
            GameStatus = GameStatus.New;
        }

        private void RevealCell(GameCell cell)
        {
            cell.IsRevealed = true;

            if (cell.AdjacentMines == 0)
            {
                foreach (var adjCell in GameBoard.GetAdjacentCells(cell))
                {
                    if (!adjCell.IsRevealed)
                    {
                        RevealCell(adjCell);
                    }
                }
            }
        }

        private void RevealMines()
        {
            foreach (var cell in GameBoard.Where(gc => gc.IsMine && !gc.IsFlagged))
            {
                cell.IsRevealed = true;
            }
        }
    }
}