using System;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Authentication.ExtendedProtection;

class Solver
{
    private Difficulty difficulty;
    private Grid grid;
    public Solver(Difficulty Difficulty, Grid Grid)
    {
        this.difficulty = Difficulty;
        this.grid = Grid;
        this.hardestPatternFound = false;
    }
    private bool hardestPatternFound;
    public bool HardestPatternFound
    {
        get
        {
            return hardestPatternFound;
        }
    }
    
    public bool IsSolvable()
    {
        int passes = 0;
        //grid.OutputLogicBoard();

        using (StreamWriter sw = new StreamWriter("log.txt", true))
        {
            while (PassThroughGrid())
            {
                passes++;

                //grid.Log();
                sw.WriteLine("Completed pass " + passes);
            }
        }

        return grid.IsSolved() && hardestPatternFound;
    }

    private bool PassThroughGrid()
    {
        bool changed = false;
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.GetLogicCell(x, y).CanBeChecked)
                {
                    changed = CheckBeginner(x, y) || changed;
                }
            }
        }

        if (changed)
        {
            if (difficulty.HardestDifficulty == 1)
            {
                hardestPatternFound = true;
            }
            return true;
        }
        else if (difficulty.HardestDifficulty == 1)
        {
            return false;
        }

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.GetLogicCell(x, y).CanBeChecked)
                {
                    changed = CheckAmateur(x, y) || changed;
                }
            }
        }

        if (changed)
        {
            if (difficulty.HardestDifficulty == 2)
            {
                hardestPatternFound = true;
            }
            return true;
        }

        return false;


    }
    
    private bool CheckBeginner(int x, int y)
    {
        foreach (Func<LogicCell, Grid, int, int, bool> beginnerPattern in difficulty.BeginnerPatterns)
        {
            if (beginnerPattern(grid.GetLogicCell(x, y), grid, x, y))
            {
                return true;
            }
        }

        if (grid.GetLogicCell(x, y).EffectiveValue == 0) grid.GetLogicCell(x, y).Complete();

        return false;
    }
    private bool CheckAmateur(int x, int y)
    {
        foreach (Func<LogicCell, Grid, int, int, bool> amateurPattern in difficulty.AmateurPatterns)
        {
            if (amateurPattern(grid.GetLogicCell(x, y), grid, x, y))
            {
                if (difficulty.HardestDifficulty == 2)
                {
                    hardestPatternFound = true;
                }
                return true;
            }
        }
        return false;
    }
}