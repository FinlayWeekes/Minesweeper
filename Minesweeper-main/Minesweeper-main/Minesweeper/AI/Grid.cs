using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Minesweeper.AI
{
    public class Grid
    {
        private LogicCell[,] LogicBoard;
        public int Width
        {
            get
            {
                return width;
            }
        }
        private int width;
        public int Height
        {
            get
            {
                return height;
            }
        }
        private int height;
        public int MineCount
        {
            get
            {
                return GetMineCount();
            }
        }
        private int mineCount;
        public int TotalMineCount
        {
            get
            {
                return totalMineCount;
            }
        }
        private int totalMineCount;
        public int Seed
        {
            get
            {
                return seed;
            }
        }
        private int seed;
        
        public Grid(int width, int height, int xFirstClick, int yFirstClick, int mineCount, int seed, bool isRandom)
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
                while (!ValidMineLocation(xMineLocations, yMineLocations, i, xFirstClick, yFirstClick, isRandom));
            }

            PopulateCells(xMineLocations, yMineLocations);

            AddValuesToCells();
        }
        private void PopulateCells(int[] xMineLocations, int[] yMineLocations)
        {
            int id = 0;
            LogicBoard = new LogicCell[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool StartHidden = true;
                    LogicBoard[x, y] = new LogicCell(StartHidden, id);
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
        private bool ValidMineLocation(int[] xMineLocations, int[] yMineLocations, int currentIndex, int xFirstClick, int yFirstClick, bool isRandom)
        {
            for (int i = 0; i < currentIndex; i++)
            {
                if (xMineLocations[currentIndex] == xMineLocations[i] && yMineLocations[currentIndex] == yMineLocations[i])
                {
                    return false;
                }
            }

            if (isRandom)
            {
                return xMineLocations[currentIndex] != xFirstClick || yMineLocations[currentIndex] != yFirstClick;
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
        
        // Methods that give info about the grid
        public bool IsSolved()
        {
            foreach (LogicCell cell in LogicBoard)
            {
                if (cell.IsHidden && !cell.IsMine) return false;
            }
            return true;
        }
        public LogicCell GetCell(int x, int y)
        {
            return LogicBoard[x, y];
        }
        public LogicCell GetCellTup((int x, int y) point)
        {
            return LogicBoard[point.x, point.y];
        }
        public bool IsInBounds(int x, int y)
        {
            return x < width && x >= 0 && y < height && y >= 0;
        }
        public bool IsOnEdge(int x, int y)
        {
            return x == 0 || x == width - 1 || y == 0 || y == height - 1;
        }
        private int GetMineCount()
        {
            int count = totalMineCount;
            foreach (LogicCell cell in LogicBoard)
            {
                if (cell.IsFlagged) count--;
            }

            return count;   
        }
        public int Find3BV()
        {
            int betchels = 0;

            // finds all clicks that are not revealed indriectly by a clear cell
            foreach (LogicCell cell in LogicBoard)
            {
                if (!cell.IsMine && !cell.IsAdjacentToClear())
                {
                    betchels++;
                }
            }


            // finds all groups of cells that clicking a clear cell opens
            // explained more in the documentation
            bool[,] grid = new bool[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (LogicBoard[x, y].Value == 0 && !LogicBoard[x, y].IsMine) grid[x, y] = true;
                    else grid[x, y] = false;
                }
            }
            // finds all clusters of trues in the grid array
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] == true)
                    {
                        // Open3BV removes a whole cluster using recursion
                        Open3BV(grid, x, y);
                        betchels++;
                    }
                }
            }

            return betchels;
        }
        private void Open3BV(bool[,] grid, int row, int col)
        {
            // used in the Find3BV method
            grid[row, col] = false;
            for (int xOfset = -1; xOfset <= 1; xOfset++)
            {
                for (int yOfset = -1; yOfset <= 1; yOfset++)
                {
                    if (col + xOfset >= 0 &&
                        col + xOfset < grid.GetLength(1) &&
                        row + yOfset >= 0 &&
                        row + yOfset < grid.GetLength(0) &&
                        grid[row + yOfset, col + xOfset] == true)
                    {
                        Open3BV(grid, row + yOfset, col + xOfset);
                    }
                }
            }
        }

        // Methods that prints the grid in the output menu
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
            string board = "";
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                { 
                    if (LogicBoard[x, y].IsHidden)
                    {
                        if (LogicBoard[x, y].IsFlagged)
                        {
                            board += "@";
                        }
                        else
                        {
                            board += "H";
                        }
                    }
                    else if (LogicBoard[x, y].Value == 0)
                    {
                        board += ".";
                    }
                    else
                    {
                       board += LogicBoard[x, y].Value;
                    }
                }
                board += "\n";
            }

            System.Diagnostics.Debug.WriteLine(board);
        }
    }
}