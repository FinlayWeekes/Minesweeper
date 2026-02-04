using System;
using System.Threading;

namespace Minesweeper.AI
{
    class Solver
    {
        private Difficulty difficulty;
        private Grid grid;
        public bool HardestPatternFound
        {
            get
            {
                return hardestPatternFound;
            }
        }
        private bool hardestPatternFound;
        
        public Solver(Difficulty Difficulty, Grid Grid)
        {
            this.difficulty = Difficulty;
            this.grid = Grid;
            this.hardestPatternFound = false;
        }

        public bool IsSolvable()
        {
            bool changed;
            int passes = 0;
            do
            {
                passes++;
                changed = PassThroughGrid();
                //System.Diagnostics.Debug.WriteLine("");
                //grid.DebugDisplayGrid();
                //System.Diagnostics.Debug.WriteLine("Completed pass: " + passes);
            }
            while (changed);

            return grid.IsSolved() && (hardestPatternFound || (difficulty.EssentialPatterns.Count == 0 && !difficulty.MineCountingEssential));
        }

        private bool PassThroughGrid()
        {
            // checks each cell for easy patterns and if it finds and excecutes one, it will continue looking for easy and wil only check for optional after it finds no easy patterns
            // it checks for easy patterns (B1 and B2) first as they are both very common and very easy to check compared to some optional patterns which improves efficency
            // each cell has a boolean value which shows weather it has been completed and completed cells are not checked for patterns
            // cells are completed when either they are hidden or their effective value is 0 (checked after B2 has been done) 
            bool changed = false;
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid.GetCell(x, y).CanBeChecked)
                    {
                        if (CheckEasy(x, y))
                        {
                            changed = true;
                        }
                    }
                }
            }
            if (changed) return true;

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid.GetCell(x, y).CanBeChecked)
                    {
                        if (CheckOptional(x, y))
                        {
                            changed = true;
                        }
                    }
                }
            }
            if (changed) return true;

            //System.Diagnostics.Debug.WriteLine("Checking essential");
            // essential patterns
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid.GetCell(x, y).CanBeChecked)
                    {
                        if (CheckEssential(x, y))
                        {
                            hardestPatternFound = true;
                            return true;
                        }
                    }
                }
            }

            // checks minecount at the very end
            if (difficulty.MineCounting)
            {
                if (CheckMineCount())
                {
                    hardestPatternFound = difficulty.MineCountingEssential;
                    return true;
                }
            }

            return false;
        }

        // returns true if it finds a pattern on a cell and excecutes it
        private bool CheckEasy(int x, int y)
        {
            foreach (Func<LogicCell, Grid, int, int, bool> easyPattern in difficulty.EasyPatterns)
            {
                if (easyPattern(grid.GetCell(x, y), grid, x, y))
                {
                    //System.Diagnostics.Debug.WriteLine("Found easy pattern at: " + x + ", " + y);
                    return true;
                }
            }

            return false;
        }
        private bool CheckOptional(int x, int y)
        {
            foreach (Func<LogicCell, Grid, int, int, bool> optionalPattern in difficulty.OptionalPatterns)
            {
                if (optionalPattern(grid.GetCell(x, y), grid, x, y))
                {
                    //System.Diagnostics.Debug.WriteLine("Found optional pattern at: " + x + ", " + y);
                    return true;
                }
            }

            return false;
        }
        private bool CheckEssential(int x, int y)
        {
            foreach (Func<LogicCell, Grid, int, int, bool> esentialPatterns in difficulty.EssentialPatterns)
            {
                if (esentialPatterns(grid.GetCell(x, y), grid, x, y))
                {
                    //System.Diagnostics.Debug.WriteLine("FOUND ESSENTIAL PATTERN AT: " + x + ", " + y);
                    return true;
                }
            }

            return false;
        }
        private bool CheckMineCount()
        {
            MineCounting mineCountingSolver = new MineCounting(grid);
            return mineCountingSolver.Solve();
        }
    }
}