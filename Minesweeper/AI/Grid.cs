using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Minesweeper.AI
{
    public class Grid
    {
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
        private int seed;
        public int Seed
        {
            get
            {
                return seed;
            }
        }

        public Grid(int width, int height, int xFirstClick, int yFirstClick, int mineCount, int seed)
        {
            this.width = width;
            this.height = height;
            this.mineCount = mineCount;
            this.totalMineCount = mineCount;
            this.seed = seed;

            // sets ranomd mine locations
            int[] xMineLocations = new int[mineCount];
            int[] yMineLocations = new int[mineCount];
            Random rand = new Random(seed);
            for (int i = 0; i < mineCount; i++)
            {
                do
                {
                    xMineLocations[i] = rand.Next(0, width);
                    yMineLocations[i] = rand.Next(0, height);
                }
                while (!ValidMineLocation(xMineLocations, yMineLocations, i, xFirstClick, yFirstClick));
            }

            PopulateCells(xMineLocations, yMineLocations);

            AddValuesToCells();
        }

        private void PopulateCells(int[] xMineLocations, int[] yMineLocations)
        {
            int id = 0;
            LogicBoard = new LogicCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    LogicBoard[x, y] = new LogicCell(x, y, true, width, height, id);
                    id++;
                }
            }

            for (int i = 0; i < mineCount; i++)
            {
                LogicBoard[xMineLocations[i], yMineLocations[i]].SetMine();
            }
        }
        private void AddValuesToCells()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int xOffset = -1; xOffset <= 1; xOffset++)
                    {
                        for (int yOffset = -1; yOffset <= 1; yOffset++)
                        {
                            if (x + xOffset >= 0 && 
                                x + xOffset < width && 
                                y + yOffset >= 0 && 
                                y + yOffset < height &&
                                !(xOffset == 0 && yOffset == 0))
                            {
                                LogicBoard[x, y].AddAdjacentCell(LogicBoard[x + xOffset, y + yOffset]);
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
        public LogicCell GetCell(int x, int y)
        {
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
        public void DebugDisplayRevealedGrid()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //if (LogicBoard[x, y].CanBeChecked)
                    //{
                    //    System.Diagnostics.Debug.Write("#");
                    //}
                    //else
                    //{
                    //    System.Diagnostics.Debug.Write(".");
                    //}
                    if (LogicBoard[x, y].IsMine)
                    {
                        System.Diagnostics.Debug.Write("#");
                    }
                    else if (LogicBoard[x, y].Value == 0)
                    {
                        System.Diagnostics.Debug.Write(".");
                    }
                    else
                    {
                        System.Diagnostics.Debug.Write(LogicBoard[x, y].Value);
                    }
                }
                System.Diagnostics.Debug.WriteLine("");
            }
        }
        public void DebugDisplayGrid()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                { 
                    if (LogicBoard[x, y].IsHidden)
                    {
                        if (LogicBoard[x, y].IsFlagged)
                        {
                            System.Diagnostics.Debug.Write("@");
                        }
                        else
                        {
                            System.Diagnostics.Debug.Write("H");
                        }
                    }
                    else if (LogicBoard[x, y].Value == 0)
                    {
                        System.Diagnostics.Debug.Write(".");
                    }
                    else
                    {
                        System.Diagnostics.Debug.Write(LogicBoard[x, y].Value);
                    }
                }
                System.Diagnostics.Debug.WriteLine("");
            }
        }
    }
}