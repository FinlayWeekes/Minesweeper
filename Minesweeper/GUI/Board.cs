using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Configuration;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Windows.Forms;
using Minesweeper;

namespace Minesweeper
{
    public partial class Board : Form
    {
        private DisplayCell[,] cells;
        private double time;
        private string difficulty;
        private bool firstClick = true;
        private const int headerSize = 40;
        private int remaningHiddenCells;
        private int remaningMines;
        private int cellSize;
        private int height;
        private int width;
        private int mineCount;
        private int clickCount;

        private Color hiddenCol = Color.Pink;
        private Color revealedCol = Color.LavenderBlush;
        private Color explodedMineCol = Color.Red;
        private Color revealedMineCol = Color.DarkRed;

        public Board(int Width, int Height, int Minecount, string Difficulty)
        {
            InitializeComponent();

            difficulty = Difficulty;
            clickCount = 0;
            height = Height;
            width = Width;
            mineCount = Minecount;
            remaningHiddenCells = width*height;
            remaningMines = mineCount;
            MineCountDisplay.Text = remaningMines.ToString();

            int maxSize = 40;
            cellSize = CalculateCellSize(maxSize);

            this.ClientSize = new Size(width * cellSize, height * cellSize + headerSize);

            PopulateCells();
        }
        private void PopulateCells()
        {
            cells = new DisplayCell[height, width];
            int fontSize = Math.Max(cellSize - 19, 5);

            SuspendLayout();

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Button btn = new Button();
                    btn.Width = cellSize;
                    btn.Height = cellSize;
                    btn.Location = new Point(col * cellSize, row * cellSize + headerSize);
                    btn.Name = "Cell" + (row * width + col);
                    btn.Size = new Size(cellSize, cellSize);
                    btn.BackColor = hiddenCol;
                    btn.Font = new Font("Microsoft Sans Serif", fontSize);
                    btn.Tag = new Point(col, row);
                    btn.MouseUp += CellClick;

                    Controls.Add(btn);
                    cells[row, col] = new DisplayCell(btn);
                    cells[row, col].ResetValues();
                }
            }

            ResumeLayout(false);
        }
        private void PopulateMines(int firstCol, int firstRow)
        {
            Random rnd = new Random();

            // places mines where there are no overlapping and to make te first cell clear
            for (int i = 0; i < mineCount; i++)
            {
                int col;
                int row;
                bool mineAdded;

                do
                {
                    mineAdded = false;
                    col = rnd.Next(0, width);
                    row = rnd.Next(0, height);

                    bool nextToStart = false;

                    for (int xOfset = -1; xOfset <= 1; xOfset++)
                    {
                        for (int yOfset = -1; yOfset <= 1; yOfset++)
                        {
                            if (col + xOfset >= 0 &&
                                col + xOfset < cells.GetLength(1) &&
                                row + yOfset >= 0 &&
                                row + yOfset < cells.GetLength(0) &&
                                col + xOfset == firstCol &&
                                row + yOfset == firstRow)
                            {
                                nextToStart = true;
                            }
                        }
                    }

                    if (!nextToStart && !cells[row, col].IsMine)
                    {
                        mineAdded = true;
                        cells[row, col].SetMine();

                        for (int xOfset = -1; xOfset <= 1; xOfset++)
                        {
                            for (int yOfset = -1; yOfset <= 1; yOfset++)
                            {
                                if (col + xOfset >= 0 &&
                                    col + xOfset < cells.GetLength(1) &&
                                    row + yOfset >= 0 &&
                                    row + yOfset < cells.GetLength(0))
                                {
                                    cells[row + yOfset, col + xOfset].IncreaseValue();
                                }
                            }
                        }
                    }
                }
                while (!mineAdded);
            }
        }
        private void CellClick(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            clickCount++;

            if (e.Button == MouseButtons.Right)
            {
                if (GetDisplayCell(btn).IsHidden)
                {
                    FlagCell(btn);
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (GetDisplayCell(btn).IsHidden)
                {
                    RevealCell(btn);
                }
                else
                {
                    Chord(btn);
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                if (!GetDisplayCell(btn).IsHidden)
                {
                    Chord(btn);
                }
            }

        }
        private void Chord(Button btn)
        {
            int adjacentHiddenCells = 0;
            int adjacentFlags = 0;
            for (int xOfset = -1; xOfset <= 1; xOfset++)
            {
                for (int yOfset = -1; yOfset <= 1; yOfset++)
                {
                    if (((Point)btn.Tag).X + xOfset >= 0 && ((Point)btn.Tag).X + xOfset < cells.GetLength(1) && ((Point)btn.Tag).Y + yOfset >= 0 && ((Point)btn.Tag).Y + yOfset < cells.GetLength(0))
                    {
                        if (cells[((Point)btn.Tag).Y + yOfset, ((Point)btn.Tag).X + xOfset].IsHidden)
                        {
                            adjacentHiddenCells++;
                            if (cells[((Point)btn.Tag).Y + yOfset, ((Point)btn.Tag).X + xOfset].IsFlagged)
                            {
                                adjacentFlags++;
                            }
                        }
                    }
                }
            }

            // opens all adjacent cells if the cells effective value = 0
            if (GetDisplayCell(btn).Value == adjacentFlags)
            {
                for (int xOfset = -1; xOfset <= 1; xOfset++)
                {
                    for (int yOfset = -1; yOfset <= 1; yOfset++)
                    {
                        if (((Point)btn.Tag).X + xOfset >= 0 &&
                            ((Point)btn.Tag).X + xOfset < cells.GetLength(1) &&
                            ((Point)btn.Tag).Y + yOfset >= 0 && 
                            ((Point)btn.Tag).Y + yOfset < cells.GetLength(0) &&
                            !cells[((Point)btn.Tag).Y + yOfset, ((Point)btn.Tag).X + xOfset].IsFlagged &&
                            cells[((Point)btn.Tag).Y + yOfset, ((Point)btn.Tag).X + xOfset].IsHidden)
                        {
                            RevealCell(cells[((Point)btn.Tag).Y + yOfset, ((Point)btn.Tag).X + xOfset].Btn);
                        }
                    }
                }
            }
            // flags all adjacent cells if the cells value is the number of adjacent cells
            else if (GetDisplayCell(btn).Value == adjacentHiddenCells)
            {
                for (int xOfset = -1; xOfset <= 1; xOfset++)
                {
                    for (int yOfset = -1; yOfset <= 1; yOfset++)
                    {
                        if (((Point)btn.Tag).X + xOfset >= 0 && 
                            ((Point)btn.Tag).X + xOfset < cells.GetLength(1) &&
                            ((Point)btn.Tag).Y + yOfset >= 0 && 
                            ((Point)btn.Tag).Y + yOfset < cells.GetLength(0) &&
                            !cells[((Point)btn.Tag).Y + yOfset, ((Point)btn.Tag).X + xOfset].IsFlagged &&
                            cells[((Point)btn.Tag).Y + yOfset, ((Point)btn.Tag).X + xOfset].IsHidden)
                        {
                            FlagCell(cells[((Point)btn.Tag).Y + yOfset, ((Point)btn.Tag).X + xOfset].Btn);
                        }
                    }
                }
            }
        }
        private void FlagCell(Button btn)
        {
            if (!GetDisplayCell(btn).IsHidden) return;

            GetDisplayCell(btn).Flag();

            if (btn.Text == "F")
            {
                btn.Text = "";
                remaningMines++;
                MineCountDisplay.Text = remaningMines.ToString();
            }
            else
            {
                btn.Text = "F";
                remaningMines--;
                MineCountDisplay.Text = remaningMines.ToString();
            }
        }
        private void RevealCell(Button btn)
        {
            // makes the background grey if no mine
            // makes the background red if mine and runs RevealMine()
            remaningHiddenCells--;

            if (firstClick)
            {
                firstClick = false;
                PopulateMines(((Point)btn.Tag).X, ((Point)btn.Tag).Y);
                timer.Start();
            }

            if (GetDisplayCell(btn).IsMine)
            {
                btn.BackColor = explodedMineCol;
                btn.Text = "";
                RevealMine();
                return;
            }

            btn.BackColor = revealedCol;
            GetDisplayCell(btn).Open();

            // opens all ajacent is a clear cell is revealed
            if (GetDisplayCell(btn).Value == 0)
            {
                btn.Text = "";

                for (int xOfset = -1; xOfset <= 1; xOfset++)
                {
                    for (int yOfset = -1; yOfset <= 1; yOfset++)
                    {
                        if (((Point)btn.Tag).X + xOfset >= 0 && 
                            ((Point)btn.Tag).X + xOfset < cells.GetLength(1) && 
                            ((Point)btn.Tag).Y + yOfset >= 0 && 
                            ((Point)btn.Tag).Y + yOfset < cells.GetLength(0) &&
                            cells[((Point)btn.Tag).Y + yOfset, ((Point)btn.Tag).X + xOfset].IsHidden)
                        {
                            RevealCell(cells[((Point)btn.Tag).Y + yOfset, ((Point)btn.Tag).X + xOfset].Btn);
                        }
                        
                    }
                }

            }
            else
            {
                btn.Text = GetDisplayCell(btn).Value.ToString();
                AddColourToText(int.Parse(btn.Text), btn);
            }

            if (CheckHasWon()) WonGame();
        }
        private void WonGame()
        {
            // makes all cells unclickable
            resetButton.Text = "YOU WIN";
            timer.Stop();
            foreach (DisplayCell cell in cells)
            {
                cell.Btn.Enabled = false;
            }

            Stats stats = new Stats(difficulty, width, height, mineCount, Find3BV(), clickCount, time/10);
            stats.Show();
        }
        private DisplayCell GetDisplayCell(Button btn)
        {
            return cells[((Point)btn.Tag).Y, ((Point)btn.Tag).X];
        }
        private bool CheckHasWon()
        {
            // checks if the player has won
            if (remaningHiddenCells == mineCount)
            {
                return true;
            }
            return false;
        }
        private int Find3BV()
        {
            int betchels = 0;

            // finds all clicks that are not revealed indriectly by a clear cell
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (!cells[row, col].IsMine && !IsAdjacentToClear(row, col))
                    {
                        betchels++;
                    }
                }
            }
            
            // finds all groups of cells that clicking a clear cell opens
            bool[,] grid = new bool[height, width];
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (cells[row, col].Value == 0)
                    {
                        grid[row, col] = true;
                    }
                    else
                    {
                        grid[row, col] = false;
                    }
                }
            }
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (grid[row, col])
                    {
                        Open3BV(grid, row, col);
                        betchels++;
                    }
                }
            }

            return betchels;
        }
        private void Open3BV(bool[,] grid, int row, int col)
        {
            grid[row, col] = false;
            for (int xOfset = -1; xOfset <= 1; xOfset++)
            {
                for (int yOfset = -1; yOfset <= 1; yOfset++)
                {
                    if (col + xOfset >= 0 &&
                        col + xOfset < grid.GetLength(1) &&
                        row + yOfset >= 0 &&
                        row + yOfset < grid.GetLength(0) &&
                        grid[row, col]
                        )
                    {
                        Open3BV(grid, row + yOfset, col + xOfset);
                    }
                }
            }
        }
        private bool IsAdjacentToClear(int row, int col)
        {
            for (int xOfset = -1; xOfset <= 1; xOfset++)
            {
                for (int yOfset = -1; yOfset <= 1; yOfset++)
                {
                    if (((Point)cells[row, col].Btn.Tag).X + xOfset >= 0 &&
                        ((Point)cells[row, col].Btn.Tag).X + xOfset < cells.GetLength(1) &&
                        ((Point)cells[row, col].Btn.Tag).Y + yOfset >= 0 &&
                        ((Point)cells[row, col].Btn.Tag).Y + yOfset < cells.GetLength(0) &&
                        cells[((Point)cells[row, col].Btn.Tag).Y + yOfset, ((Point)cells[row, col].Btn.Tag).X + xOfset].Value == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void RevealMine()
        {
            // reveals all mines on the board and disables all cells

            foreach (DisplayCell cell in cells)
            {
                if (cell.IsMine)
                {
                    if (cell.Btn.BackColor != explodedMineCol || explodedMineCol == revealedMineCol)
                    {
                        cell.Btn.BackColor = revealedMineCol;
                    }
                    cell.Btn.Text = "";
                }
                
                cell.Btn.Enabled = false;
            }
        }
        private int CalculateCellSize(int maxSize)
        {
            Rectangle workingArea;
            workingArea = Screen.PrimaryScreen.WorkingArea;
            int topBarHeight = 21;

            int maxCellWidth = workingArea.Width / width;
            int maxCellHeight = (workingArea.Height-headerSize-topBarHeight) / height;
            int maxCell = Math.Min(maxCellWidth, maxCellHeight);

            return Math.Min(maxSize, maxCell);
        }
        public void AddColourToText(int value, Button btn)
        {
            switch (value)
            {
                case 1: btn.ForeColor = Color.DarkBlue; break;
                case 2: btn.ForeColor = Color.Green; break;
                case 3: btn.ForeColor = Color.Red; break;
                case 4: btn.ForeColor = Color.Navy; break;
                case 5: btn.ForeColor = Color.DarkRed; break;
                case 6: btn.ForeColor = Color.MediumTurquoise; break;
                case 7: btn.ForeColor = Color.Black; break;
                case 8: btn.ForeColor = Color.DarkGray; break;
            }
        }


        private void resetButton_Click(object sender, EventArgs e)
        {
            firstClick = true;
            remaningMines = mineCount;
            MineCountDisplay.Text = mineCount.ToString();
            clickCount = 0;
            remaningHiddenCells = width * height;
            resetButton.Text = "Reset";
            time = 0;
            timeLabel.Text = "0";

            foreach (DisplayCell cell in cells)
            {
                cell.ResetValues();
                cell.Btn.BackColor = hiddenCol;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            time++;

            if (time % 10 == 0)
            {
                timeLabel.Text = Convert.ToString(time/10);
            }
        }
        private void menuButton_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Close();
        }
    }
}
