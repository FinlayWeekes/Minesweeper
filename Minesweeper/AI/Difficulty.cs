using System;
using System.Collections.Generic;

class Difficulty
{
    private List<Func<LogicCell, Grid, int, int, bool>> beginnerPatterns;
    public List<Func<LogicCell, Grid, int, int, bool>> BeginnerPatterns
    {
        get
        {
            return beginnerPatterns;
        }
    }
    public int BeginnerPatternCount
    {
        get
        {
            return beginnerPatterns.Count;
        }
    }
    private List<Func<LogicCell, Grid, int, int, bool>> amateurPatterns;
    public List<Func<LogicCell, Grid, int, int, bool>> AmateurPatterns
    {
        get
        {
            return amateurPatterns;
        }
    }
    public int AmateurPatternCount
    {
        get
        {
            return amateurPatterns.Count;
        }
    }
    private int hardestDifficulty;
    public int HardestDifficulty
    {
        get
        {
            return hardestDifficulty;
        }
    }
    private int width;
    public int Width
    {
        get
        {
            return width;
        }
    }
    private int height;
    public int Height
    {
        get
        {
            return height;
        }
    }
    private int mineCount;
    public int MineCount
    {
        get
        {
            return mineCount;
        }
    }

    public Difficulty(int difficulty)
    {
        hardestDifficulty = difficulty;

        switch (difficulty)
        {
            case 1:
                {
                    AddBeginnerPatterns();
                    width = 9;
                    height = 9;
                    mineCount = 10;
                    break;
                }
            case 2:
                {
                    AddAmateurPatterns();
                    width = 12;
                    height = 12;
                    mineCount = 20;
                    break;
                }
        }
    }

    private void AddBeginnerPatterns()
    {
        beginnerPatterns = new List<Func<LogicCell, Grid, int, int, bool>>();
        BeginnerAI beginnerAI = new BeginnerAI();

        beginnerPatterns.Add(beginnerAI.PB1);
        beginnerPatterns.Add(beginnerAI.PB2);
    }
    private void AddAmateurPatterns()
    {
        AddBeginnerPatterns();
        amateurPatterns = new List<Func<LogicCell, Grid, int, int, bool>>();
        AmateurAI amateurAI = new AmateurAI();

        amateurPatterns.Add(amateurAI.P2_1);
        amateurPatterns.Add(amateurAI.P1_1C);
    }

    public Func<LogicCell, Grid, int, int, bool> GetBeginnerPattern(int i)
    {
        return beginnerPatterns[i];
    }
    public Func<LogicCell, Grid, int, int, bool> GetAmateurPattern(int i)
    {
        return amateurPatterns[i];
    }
}