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

    public class Minesweeper
    {
        private bool _firstClick;

        public Minesweeper()
        {
            GameBoard = new GameBoard();
            GameStatus = GameStatus.New;
        }

        public event EventHandler MinesPlaced;

        public GameBoard GameBoard { get; private set; }

        public GameStatus GameStatus { get; private set; }

        public int RemainingMines { get; private set; }

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
                for (int x = Math.Max(0, cell.X - 1); x <= Math.Min(cell.X + 1, GameBoard.X - 1); x++)
                {
                    for (int y = Math.Max(0, cell.Y - 1); y <= Math.Min(cell.Y + 1, GameBoard.Y - 1); y++)
                    {
                        var adjCell = GameBoard[x, y];
                        if (!adjCell.IsRevealed)
                        {
                            RevealCell(adjCell);
                        }
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