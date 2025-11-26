using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Board : Form
    {
        private DisplayCell[,] cells;
        private bool firstClick = true;
        private const int headerSize = 40;
        private int remaningHiddenCells;
        private int remaningMines;
        private int cellSize;
        private int height;
        private int width;
        private int mineCount;

        private Color hiddenCol = Color.White;
        private Color revealedCol = Color.LightGray;
        private Color explodedMineCol = Color.Red;
        private Color revealedMineCol = Color.DarkRed;


        public Board(int Height, int Width, int Minecount)
        {
            InitializeComponent();

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

                    btn.Tag = new Point(col, row);

                    btn.MouseUp += CellClick;

                    Controls.Add(btn);
                    cells[row, col] = new DisplayCell(btn);
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
                int col = rnd.Next(0, width);
                int row = rnd.Next(0, height);
                if (!cells[row, col].IsMine)
                {
                    bool nextToStart = false;

                    for (int xOfset = -1; xOfset <= 1; xOfset++)
                    {
                        for (int yOfset = -1; yOfset <= 1; yOfset++)
                        {
                            if (firstCol + xOfset == col && firstRow + yOfset == row)
                            {
                                nextToStart = true;
                            }
                        }
                    }

                    if (!nextToStart)
                    {
                        cells[row, col].SetMine();

                        for (int xOfset = -1; xOfset <= 1; xOfset++)
                        {
                            for (int yOfset = -1; yOfset <= 1; yOfset++)
                            {
                                if (col + xOfset >= 0 && col + xOfset < cells.GetLength(0) && row + yOfset >= 0 && row + yOfset < cells.GetLength(1))
                                {
                                    cells[row + yOfset, col + xOfset].IncreaseValue();
                                }
                            }
                        }
                    }
                }
            }


        }
        private void CellClick(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            if (e.Button == MouseButtons.Right)
            {
                FlagCell(btn);
            }
            else if (e.Button == MouseButtons.Left)
            {
                RevealCell(btn);
            }
        }
        private void FlagCell(Button btn)
        {
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
            // disables the cell from interactions
            // makes the background grey if no mine
            // makes the background red if mine and runs RevealMine()
            remaningHiddenCells--;

            if (firstClick)
            {
                firstClick = false;
                PopulateMines(((Point)btn.Tag).X, ((Point)btn.Tag).Y);
            }

            if (cells[((Point)btn.Tag).Y, ((Point)btn.Tag).X].IsMine)
            {
                btn.BackColor = explodedMineCol;
                btn.Text = "";
                RevealMine();
                return;
            }

            btn.BackColor = revealedCol;
            btn.Text = cells[((Point)btn.Tag).Y, ((Point)btn.Tag).X].Value.ToString();
            btn.Enabled = false;

            if (HasWon())
            {
                foreach (DisplayCell cell in cells)
                {
                    cell.Btn.Enabled = false;
                }
            }
        }
        private bool HasWon()
        {
            // checks if the player has won
            if (remaningHiddenCells == mineCount)
            {
                return true;
            }
            return false;
        }
        private void RevealMine()
        {
            // reveals all mines on the board and disables all cells

            foreach (DisplayCell cell in cells)
            {
                if (cell.IsMine && (cell.Btn.BackColor != explodedMineCol || explodedMineCol == revealedMineCol))
                {
                    cell.Btn.BackColor = revealedMineCol;
                }
                cell.Btn.Enabled = false;
            }
        }
        private int CalculateCellSize(int maxSize)
        {
            Rectangle workingArea;
            workingArea = Screen.PrimaryScreen.WorkingArea;

            int maxCellWidth = workingArea.Width / width;
            int maxCellHeight = (workingArea.Height-headerSize) / height;
            int maxCell = Math.Min(maxCellWidth, maxCellHeight);

            return Math.Min(maxSize, maxCell);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            firstClick = true;
            remaningMines = mineCount;
            remaningHiddenCells = width * height;

            foreach (DisplayCell cell in cells)
            {
                cell.ResetValues();
                cell.Btn.BackColor = hiddenCol;
            }
        }
    }
}
