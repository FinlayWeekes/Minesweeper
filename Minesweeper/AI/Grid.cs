using System;
using System.IO;
using System.Runtime.InteropServices;

class Grid
{
    private DisplayCell[,] RevealedBoard;
    private LogicCell[,] LogicBoard;
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
    private int totalMineCount;
    public int TotalMineCount
    {
        get
        {
            return totalMineCount;
        }
    }
    
    private const string MineMarker = "M";
    private const string FlagMarker = "#";
    private const string HiddenMarker = "H";


    public Grid(int width, int height, int xFirstClick, int yFirstClick, int mineCount)
    {
        this.width = width;
        this.height = height;
        this.mineCount = mineCount;
        this.totalMineCount = mineCount;

        // sets ranomd mine locations
        int[] xMineLocations = new int[mineCount];
        int[] yMineLocations = new int[mineCount];
        Random rand = new Random();
        for (int i = 0; i < mineCount; i++)
        {
            do
            {
                xMineLocations[i] = rand.Next(0, width);
                yMineLocations[i] = rand.Next(0, height);
            }
            while (!ValidMineLocation(xMineLocations, yMineLocations, i, xFirstClick, yFirstClick));
        }
        LogMineLocations(xMineLocations, yMineLocations);

        int id = 0;
        RevealedBoard = new DisplayCell[width, height];
        LogicBoard = new LogicCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                RevealedBoard[x, y] = new DisplayCell(x, y, false, width, height);
                LogicBoard[x, y] = new LogicCell(x, y, true, width, height, id);
                id++;
            }
        }

        for (int i = 0; i < mineCount; i++)
        {
            RevealedBoard[xMineLocations[i], yMineLocations[i]].SetMine();
            LogicBoard[xMineLocations[i], yMineLocations[i]].SetMine();
        }

        AddValuesToLogicCells();

        LogicBoard[xFirstClick, yFirstClick].Open();
    }

    public Grid(int width, int height, int xFirstClick, int yFirstClick, int[] mineXLocations, int[] mineYLocations)
    {
        this.width = width;
        this.height = height;
        this.mineCount = mineXLocations.Length;
        this.totalMineCount = mineCount;
        RevealedBoard = new DisplayCell[width, height];
        LogicBoard = new LogicCell[width, height];

        int id = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                RevealedBoard[x, y] = new DisplayCell(x, y, false, width, height);
                LogicBoard[x, y] = new LogicCell(x, y, true, width, height, id);
                id++;

                for (int i = 0; i < mineXLocations.Length; i++)
                {
                    if (x == mineXLocations[i] && y == mineYLocations[i])
                    {
                        RevealedBoard[x, y].SetMine();
                        LogicBoard[x, y].SetMine();
                    }
                }
            }
        }

        AddValuesToLogicCells();

        LogicBoard[xFirstClick, yFirstClick].Open();
    }

    public void LogMineLocations(int[] xLocations, int[] yLocations)
    {
        using (StreamWriter sw = new StreamWriter("log.txt", true))
        {
            foreach (int x in xLocations)
            {
                sw.Write(x + ", ");
            }
            sw.WriteLine("");
            foreach (int y in yLocations)
            {
                sw.Write(y + ", ");
            }
            sw.WriteLine("");
        }
    }
    public void LogLogic()
    {
        using (StreamWriter sw = new StreamWriter("log.txt", true))
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (LogicBoard[x, y].IsHidden)
                    {
                        if (LogicBoard[x, y].IsFlagged)
                        {
                            sw.Write(FlagMarker);
                        }
                        else
                        {
                            sw.Write(HiddenMarker);
                        }
                    }
                    else if (LogicBoard[x, y].IsMine)
                    {
                        sw.Write(MineMarker);
                    }
                    else
                    {
                        sw.Write(Convert.ToString(LogicBoard[x, y].Value));
                    }
                }
                sw.WriteLine("");
            }
            sw.WriteLine("");
        }
    }
    public void Logrevealed()
    {
        using (StreamWriter sw = new StreamWriter("log.txt", true))
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (RevealedBoard[x, y].IsMine)
                    {
                        sw.Write(MineMarker);
                    }
                    else
                    {
                        sw.Write(Convert.ToString(RevealedBoard[x, y].Value));
                    }
                }
                sw.WriteLine("");
            }
            sw.WriteLine("");
        }
    }
    private void AddValuesToLogicCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    for (int yOffset = -1; yOffset <= 1; yOffset++)
                    {
                        if (x + xOffset >= 0 && x + xOffset < width && y + yOffset >= 0 && y + yOffset < height)
                        {
                            if (!(xOffset == 0 && yOffset == 0))
                            {
                                LogicBoard[x, y].AddAdjacentCell(LogicBoard[x + xOffset, y + yOffset]);
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                LogicBoard[x, y].SetValues();
            }
        }
    }
    private bool ValidMineLocation(int[] xMineLocations, int[] yMineLocations, int currentIndex, int xFirstClick, int yFirstClick)
    {
        for (int i = 0; i < currentIndex; i++)
        {
            if (xMineLocations[currentIndex] == xMineLocations[i] && yMineLocations[currentIndex] == yMineLocations[i])
            {
                return false;
            }
        }

        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                if (xFirstClick == xMineLocations[currentIndex] + xOffset && yFirstClick == yMineLocations[currentIndex] + yOffset)
                {
                    return false;
                }
            }
        }

        return true;
    }
    public LogicCell GetLogicCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return null;
        return LogicBoard[x, y];
    }
    public bool IsSolved()
    {
        foreach (LogicCell cell in LogicBoard)
        {
            if (cell.IsHidden && !cell.IsMine) return false;
        }
        return true;
    }
}