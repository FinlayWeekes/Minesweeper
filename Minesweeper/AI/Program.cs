using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Minesweeper.AI
{
    class Program
    {
        static Grid GetDiffcultyGrid(Difficulty difficulty)
        {
            using (StreamWriter sw = new StreamWriter("log.txt"))
            {
                sw.WriteLine();
            }
            using (StreamWriter sw = new StreamWriter("newLog.txt"))
            {
                sw.WriteLine();
            }


            Grid grid;
            Solver solver;
            int count = 0;

            do
            {
                count++;
                using (StreamWriter sw = new StreamWriter("input.txt"))
                {
                    sw.WriteLine("Generating grid " + count);
                }
                grid = new Grid(difficulty.Width, difficulty.Height, 5, 5, difficulty.MineCount);

                solver = new Solver(difficulty, grid);
            }
            while (!solver.IsSolvable());


            return grid;
        }
    }
}