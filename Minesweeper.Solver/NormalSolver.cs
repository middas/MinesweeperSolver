using Minesweeper.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minesweeper.Solver
{
    public class NormalSolver : ISolver
    {
        public Task Solve(Game.Minesweeper minesweeper)
        {
            return Task.Run(() =>
            {
                Random random = new Random();

                while (minesweeper.GameStatus != GameStatus.Win && minesweeper.GameStatus != GameStatus.Loss)
                {
                    if (!MarkMines(minesweeper))
                    {
                        RevealRandomCell(minesweeper, random);
                    }
                    else
                    {
                        RevealCellsAroundMines(minesweeper);
                    }
                }
            });
        }

        private IEnumerable<(int x, int y)> GetAvailableGameCellLocations(Game.Minesweeper minesweeper)
        {
            return GetAvailableGameCells(minesweeper).Select(gc => (gc.X, gc.Y));
        }

        private IEnumerable<GameCell> GetAvailableGameCells(Game.Minesweeper minesweeper)
        {
            return minesweeper.GameBoard.Where(gc => !gc.IsFlagged && !gc.IsRevealed);
        }

        private bool MarkMines(Game.Minesweeper minesweeper)
        {
            bool minesFound = false;

            for (int i = 1; i <= 8; i++)
            {
                foreach (var revealedCell in minesweeper.GameBoard.Where(gc => gc.IsRevealed && gc.AdjacentMines == i))
                {
                    var unrevealedAdjCells = minesweeper.GameBoard.GetAdjacentCells(revealedCell).Where(gc => !gc.IsRevealed);
                    int flagCount = unrevealedAdjCells.Count(gc => gc.IsFlagged);
                    int remainingMines = revealedCell.AdjacentMines - flagCount;

                    if (unrevealedAdjCells.Count(gc => !gc.IsFlagged) == remainingMines)
                    {
                        foreach (var cell in unrevealedAdjCells.Where(gc => !gc.IsFlagged))
                        {
                            minesweeper.BoardClick(cell.X, cell.Y, ClickAction.Flag);
                            minesFound = true;
                        }
                    }
                }
            }

            return minesFound;
        }

        private void RevealCellsAroundMines(Game.Minesweeper minesweeper)
        {
            var flaggedCells = minesweeper.GameBoard.Where(gc => gc.IsFlagged);

            foreach (var flaggedCell in flaggedCells)
            {
                foreach (var cell in minesweeper.GameBoard.GetAdjacentCells(flaggedCell).Where(gc => gc.IsRevealed))
                {
                    RevealIfConditionMet(cell, minesweeper);
                }
            }
        }

        private void RevealIfConditionMet(GameCell cell, Game.Minesweeper minesweeper)
        {
            if (cell.AdjacentMines != 0)
            {
                var unrevealedAdjCells = minesweeper.GameBoard.GetAdjacentCells(cell).Where(gc => !gc.IsRevealed);
                int flagCount = unrevealedAdjCells.Count(gc => gc.IsFlagged);
                int remainingMines = cell.AdjacentMines - flagCount;

                if (remainingMines == 0)
                {
                    foreach (var unrevealedCell in unrevealedAdjCells.Where(gc => !gc.IsFlagged))
                    {
                        minesweeper.BoardClick(unrevealedCell.X, unrevealedCell.Y, ClickAction.Reveal);

                        RevealIfConditionMet(unrevealedCell, minesweeper);
                    }
                }
            }
        }

        private void RevealRandomCell(Game.Minesweeper minesweeper, Random random)
        {
            var availablePoints = GetAvailableGameCellLocations(minesweeper);
            var (x, y) = availablePoints.ElementAt(random.Next(availablePoints.Count()));
            minesweeper.BoardClick(x, y, ClickAction.Reveal);
        }
    }
}