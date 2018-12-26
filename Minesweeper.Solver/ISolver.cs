using Minesweeper.Game;
using System;

namespace Minesweeper.Solver
{
    public interface ISolver
    {
        void Solve(Game.Minesweeper minesweeper);
    }
}
