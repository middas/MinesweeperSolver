using System.Threading.Tasks;

namespace Minesweeper.Solver
{
    public interface ISolver
    {
        Task Solve(Game.Minesweeper minesweeper);
    }
}