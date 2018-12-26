using Minesweeper.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Solver
{
    public class NormalSolver : ISolver
    {
        public void Solve(Game.Minesweeper minesweeper)
        {
            Random random = new Random();

            while (minesweeper.GameStatus != GameStatus.Win && minesweeper.GameStatus != GameStatus.Loss)
            {
                RevealRandomCell(minesweeper, random);
                MarkMines(minesweeper);
            }
        }

        private IEnumerable<(int x, int y)> GetAvailableGameCellLocations(Game.Minesweeper minesweeper)
        {
            return GetAvailableGameCells(minesweeper).Select(gc => (gc.X, gc.Y));
        }

        private IEnumerable<GameCell> GetAvailableGameCells(Game.Minesweeper minesweeper)
        {
            return minesweeper.GameBoard.Where(gc => !gc.IsFlagged && !gc.IsRevealed);
        }

        private void MarkMines(Game.Minesweeper minesweeper)
        {
            // this is broken
            for (int i = 1; i <= 8; i++)
            {
                foreach (var revealedCell in minesweeper.GameBoard.Where(gc => gc.IsRevealed && gc.AdjacentMines == i))
                {
                    var unrevealedAdjCells = minesweeper.GameBoard.GetAdjacentCells(revealedCell).Where(gc => !gc.IsRevealed);
                    if (unrevealedAdjCells.Count() - minesweeper.GameBoard.GetAdjacentCells(revealedCell).Where(gc => gc.IsFlagged).Count() == i)
                    {
                        foreach (var cell in unrevealedAdjCells.Where(gc => !gc.IsFlagged))
                        {
                            minesweeper.BoardClick(cell.X, cell.Y, ClickAction.Flag);
                        }
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