using Minesweeper.AI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Minesweeper.GUI
{
    public partial class Board : Form
    {
        private DisplayCell[,] cells;
        private Difficulty difficulty;
        private List<int> times;
        private int time;
        private bool firstClick = true;
        private int remaningHiddenCells;
        private int remaningMines;
        private int cellSize;
        private int height;
        private int width;
        private int mineCount;
        private int clickCount;
        private int topTimesCount;
        private string name;

        private const int headerSize = 40;

        private Color hiddenCol;
        private Color revealedCol;
        private Color borderCol;
        private Color explodedMineCol;
        private Color revealedMineCol;
        private Color buttonBackCol;
        private Color buttonForeCol;
        private Dictionary<int, Color> numColours = new Dictionary<int, Color>();

        private DisplayCell GetDisplayCell(Button btn)
        {
            return cells[((Point)btn.Tag).X, ((Point)btn.Tag).Y];
        }

        // Handles the generation of the board
        public Board(Difficulty Difficulty, bool LightMode, string name, int topTimesCount)
        {
            this.topTimesCount = topTimesCount;
            this.name = name;
            difficulty = Difficulty;
            height = Difficulty.Height;
            width = Difficulty.Width;
            mineCount = Difficulty.MineCount;
            if (!difficulty.Custom) times = GetTopTimes();

            InitializeComponent();

            clickCount = 0;
            remaningHiddenCells = width * height;
            remaningMines = mineCount;
            MineCountDisplay.Text = remaningMines.ToString();

            int maxSize = 40;
            cellSize = CalculateCellSize(maxSize);

            this.ClientSize = new Size(width * cellSize, height * cellSize + headerSize);

            CreateColours(LightMode);

            PopulateCells();
        }
        private List<int> GetTopTimes()
        {
            List<int> scores = new List<int>();

            using (StreamReader sr = new StreamReader(TimesTextName()))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    scores.Add(Convert.ToInt32(line.Split('-')[1]));
                }
            }

            return scores;
        }
        private void PopulateCells()
        {
            cells = new DisplayCell[width, height];
            int fontSize = Math.Max(cellSize - 19, 5);

            SuspendLayout();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Button btn = new Button();
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.Width = cellSize;
                    btn.Height = cellSize;
                    btn.Location = new Point(x * cellSize, y * cellSize + headerSize);
                    btn.Name = "Cell" + (y * width + x);
                    btn.Size = new Size(cellSize, cellSize);
                    btn.BackColor = hiddenCol;
                    btn.FlatAppearance.BorderColor = borderCol;
                    btn.Font = new Font("Microsoft Sans Serif", fontSize);
                    btn.Tag = new Point(x, y);
                    btn.MouseUp += CellClick;

                    Controls.Add(btn);
                    cells[x, y] = new DisplayCell(btn);
                    cells[x, y].ResetValues();
                }
            }

            ResumeLayout(false);
        }
        private void PopulateMines(int firstCol, int firstRow)
        {
            Grid grid;

            // creates a completley random grid that hasnt been checked if its solvable if no patterns are selected on custom
            if (difficulty.Random)
            {
                grid = new Grid(width, height, firstCol, firstRow, mineCount, GetSeed(), difficulty.Random);
            }
            else
            {
                grid = GetSolvableGrid(firstCol, firstRow);
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid.GetCell(x, y).IsMine)
                    {
                        cells[x, y].SetMine();
                    }
                    else
                    {
                        cells[x, y].Value = grid.GetCell(x, y).Value;
                    }
                }
            }
        }
        private Grid GetSolvableGrid(int firstCol, int firstRow)
        {
            int count = 0;
            Solver solver;
            Grid grid;
            do
            {
                count++;

                int seed = GetSeed();

                //System.Diagnostics.Debug.WriteLine("");
                if (count %1000 == 0) System.Diagnostics.Debug.WriteLine("GENERATING NEW BOARD: " + count + " SEED: " + seed);

                grid = new Grid(width, height, firstCol, firstRow, mineCount, seed, false);

                //grid.DebugDisplayRevealedGrid();
                grid.GetCell(firstCol, firstRow).Open();

                solver = new Solver(difficulty, grid);
            }
            while (!solver.IsSolvable());

            System.Diagnostics.Debug.WriteLine("Created after " + count + " grids");
            System.Diagnostics.Debug.WriteLine("Seed: " + grid.Seed);

            return grid;
        }
        private int CalculateCellSize(int maxSize)
        {
            Rectangle workingArea;
            workingArea = Screen.PrimaryScreen.WorkingArea;
            int topBarHeight = 21;

            int maxCellWidth = workingArea.Width / width;
            int maxCellHeight = (workingArea.Height - headerSize - topBarHeight) / height;
            int maxCell = Math.Min(maxCellWidth, maxCellHeight);

            return Math.Min(maxSize, maxCell);
        }
        private int GetSeed()
        {
            int seed;
            using (StreamReader sr = new StreamReader("seed.txt"))
            {
                seed = int.Parse(sr.ReadLine());
            }
            using (StreamWriter sw = new StreamWriter("seed.txt"))
            {
                Random random = new Random(seed);
                sw.WriteLine(random.Next());
            }
            return seed;
        }
        private string TimesTextName()
        {
            return "Times/" + difficulty.Name.ToLower() + "Times.txt";
        }


        // Handles the colours of the board
        private void CreateColours(bool lightMode)
        {
            if (lightMode)
            {
                PopulateLightColours();
            }
            else
            {
                PopulateDarkColours();
            }

            resetButton.BackColor = buttonBackCol;
            resetButton.ForeColor = buttonForeCol;
            resetButton.FlatAppearance.BorderColor = borderCol;

            menuButton.BackColor = buttonBackCol;
            menuButton.ForeColor = buttonForeCol;
            menuButton.FlatAppearance.BorderColor = borderCol;

            MineCountDisplay.ForeColor = buttonForeCol;

            timeLabel.ForeColor = buttonForeCol;
        }
        private void PopulateLightColours()
        {
            numColours.Add(1, ColorTranslator.FromHtml("#0000F0"));
            numColours.Add(2, ColorTranslator.FromHtml("#008400"));
            numColours.Add(3, ColorTranslator.FromHtml("#F91011"));
            numColours.Add(4, ColorTranslator.FromHtml("#000084"));
            numColours.Add(5, ColorTranslator.FromHtml("#840000"));
            numColours.Add(6, ColorTranslator.FromHtml("#018484"));
            numColours.Add(7, ColorTranslator.FromHtml("#840084"));
            numColours.Add(8, ColorTranslator.FromHtml("#757575"));

            hiddenCol = ColorTranslator.FromHtml("#F0F0F0");
            revealedCol = ColorTranslator.FromHtml("#D3D3D3");
            borderCol = ColorTranslator.FromHtml("#818080");
            explodedMineCol = ColorTranslator.FromHtml("#FE0101");
            revealedMineCol = ColorTranslator.FromHtml("#8B0101");

            buttonBackCol = hiddenCol;
            buttonForeCol = Color.Black;

            this.BackColor = ColorTranslator.FromHtml("#F1F0F0");
        }
        private void PopulateDarkColours()
        {
            numColours.Add(1, ColorTranslator.FromHtml("#7CC7FF"));
            numColours.Add(2, ColorTranslator.FromHtml("#66C266"));
            numColours.Add(3, ColorTranslator.FromHtml("#FE7689"));
            numColours.Add(4, ColorTranslator.FromHtml("#EE88FE"));
            numColours.Add(5, ColorTranslator.FromHtml("#DDAA23"));
            numColours.Add(6, ColorTranslator.FromHtml("#66CCCC"));
            numColours.Add(7, ColorTranslator.FromHtml("#999999"));
            numColours.Add(8, ColorTranslator.FromHtml("#D1D9E0"));

            hiddenCol = ColorTranslator.FromHtml("#4C545C");
            revealedCol = ColorTranslator.FromHtml("#394148");
            borderCol = ColorTranslator.FromHtml("#1F272E");
            explodedMineCol = ColorTranslator.FromHtml("#FE0101");
            revealedMineCol = ColorTranslator.FromHtml("#8B0101");

            buttonBackCol = hiddenCol;
            buttonForeCol = Color.White;

            this.BackColor = ColorTranslator.FromHtml("#464E56");
        }
        

        // Handles things that hapen during the game
        private void CellClick(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            clickCount++;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        bool revealedMine = false;
                        if (GetDisplayCell(btn).IsHidden)
                        {
                            if (!GetDisplayCell(btn).IsFlagged) revealedMine = RevealCell(btn);
                        }
                        else
                        {
                            Chord(btn);
                        }

                        if (!revealedMine && CheckHasWon()) WonGame();

                        break;
                    }
                case MouseButtons.Right:
                    {
                        if (GetDisplayCell(btn).IsHidden) FlagCell(btn);

                        break;
                    }
                case MouseButtons.Middle:
                    {
                        if (!GetDisplayCell(btn).IsHidden) Chord(btn);

                        break;
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
                    if (((Point)btn.Tag).X + xOfset >= 0 && 
                        ((Point)btn.Tag).X + xOfset < cells.GetLength(0) && 
                        ((Point)btn.Tag).Y + yOfset >= 0 && 
                        ((Point)btn.Tag).Y + yOfset < cells.GetLength(1))
                    {
                        if (cells[((Point)btn.Tag).X + xOfset, ((Point)btn.Tag).Y + yOfset].IsHidden)
                        {
                            adjacentHiddenCells++;
                            if (cells[((Point)btn.Tag).X + xOfset, ((Point)btn.Tag).Y + yOfset].IsFlagged)
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
                            ((Point)btn.Tag).X + xOfset < cells.GetLength(0) &&
                            ((Point)btn.Tag).Y + yOfset >= 0 && 
                            ((Point)btn.Tag).Y + yOfset < cells.GetLength(1) &&
                            !cells[((Point)btn.Tag).X + xOfset, ((Point)btn.Tag).Y + yOfset].IsFlagged &&
                            cells[((Point)btn.Tag).X + xOfset, ((Point)btn.Tag).Y + yOfset].IsHidden)
                        {
                            RevealCell(cells[((Point)btn.Tag).X + xOfset, ((Point)btn.Tag).Y + yOfset].Btn);
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
                            ((Point)btn.Tag).X + xOfset < cells.GetLength(0) &&
                            ((Point)btn.Tag).Y + yOfset >= 0 && 
                            ((Point)btn.Tag).Y + yOfset < cells.GetLength(1) &&
                            !cells[((Point)btn.Tag).X + xOfset, ((Point)btn.Tag).Y + yOfset].IsFlagged &&
                            cells[((Point)btn.Tag).X + xOfset, ((Point)btn.Tag).Y + yOfset].IsHidden)
                        {
                            FlagCell(cells[((Point)btn.Tag).X + xOfset, ((Point)btn.Tag).Y + yOfset].Btn);
                        }
                    }
                }
            }
        }
        private void FlagCell(Button btn)
        {
            if (!GetDisplayCell(btn).IsHidden) return;

            if (GetDisplayCell(btn).IsFlagged)
            {
                remaningMines++;
            }
            else
            {
                remaningMines--;
            }

            MineCountDisplay.Text = remaningMines.ToString();

            GetDisplayCell(btn).Flag();
        }
        private bool RevealCell(Button btn)
        {
            // returns true if a mine was opened
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
                return true;
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
                            ((Point)btn.Tag).X + xOfset < cells.GetLength(0) && 
                            ((Point)btn.Tag).Y + yOfset >= 0 && 
                            ((Point)btn.Tag).Y + yOfset < cells.GetLength(1) &&
                            cells[((Point)btn.Tag).X + xOfset, ((Point)btn.Tag).Y + yOfset].IsHidden)
                        {
                            RevealCell(cells[((Point)btn.Tag).X + xOfset, ((Point)btn.Tag).Y + yOfset].Btn);
                        }
                        
                    }
                }

            }
            else
            {
                btn.Text = GetDisplayCell(btn).Value.ToString();
                AddColourToText(int.Parse(btn.Text), btn);
            }

            return false;
        }
        private void RevealMine()
        {
            // reveals all mines on the board and disables all cells
            timer.Stop();

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
        private void AddColourToText(int value, Button btn)
        {
            btn.ForeColor = numColours[value];
        }


        // Handles winning the game and stats page
        private void AddTopTime()
        {
            // puts the sorted times in times and the full string into stringTimes to store the names of other players
            List<int> times = new List<int>();
            List<string> stringTimes = new List<string>();
            using (StreamReader sr = new StreamReader(TimesTextName()))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    times.Add(Convert.ToInt32(line.Split('-')[1]));
                    stringTimes.Add(line);
                }
            }

            if (times.Count == 0)
            {
                using (StreamWriter sw = new StreamWriter(TimesTextName()))
                {
                    sw.WriteLine(name + "-" + Convert.ToString(time));
                }

                return;
            }

            // binary search to find where the new time goes
            int topPointer = times.Count - 1;
            int rearPointer = 0;
            int midPointer = times.Count / 2;
            while (topPointer >= rearPointer)
            {
                if (time < times[midPointer])
                {
                    topPointer = midPointer - 1;
                }
                else
                {
                    rearPointer = midPointer + 1;
                }
                midPointer = (topPointer + rearPointer) / 2;
            }

            // adds the time to the list based on where the midpointer is by the binary search
            string stringTime = name + "-" + time;
            if (times[midPointer] > time)
            {
                stringTimes.Insert(midPointer, stringTime);
            }
            else
            {
                stringTimes.Insert(midPointer + 1, stringTime);
            }
            if (times.Count > topTimesCount)
            {
                stringTimes.RemoveAt(times.Count - 1);
            }

            using (StreamWriter sw = new StreamWriter(TimesTextName()))
            {
                for (int i = 0; i < times.Count; i++)
                {
                    sw.WriteLine(stringTimes[i]);
                }
            }
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

            if (!difficulty.Custom &&
                (times.Count < topTimesCount || time < times[times.Count - 1]))
            {
                AddTopTime();
            }

            int seconds = time / 10;
            Stats stats = new Stats(difficulty.Name, width, height, mineCount, Find3BV(), clickCount, seconds);
            stats.Show();
        }
        private bool CheckHasWon()
        {
            return remaningHiddenCells - mineCount == 0;
        }
        private int Find3BV()
        {
            int betchels = 0;

            // finds all clicks that are not revealed indriectly by a clear cell
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!cells[x, y].IsMine && !IsAdjacentToClear(x, y))
                    {
                        betchels++;
                    }
                }
            }
            

            // finds all groups of cells that clicking a clear cell opens
            bool[,] grid = new bool[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (cells[x, y].Value == 0)
                    {
                        grid[x, y] = true;
                    }
                    else
                    {
                        grid[x, y] = false;
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] == true)
                    {
                        Open3BV(grid, x, y);
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
                        grid[row + yOfset, col + xOfset] == true
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
                        ((Point)cells[row, col].Btn.Tag).X + xOfset < cells.GetLength(0) &&
                        ((Point)cells[row, col].Btn.Tag).Y + yOfset >= 0 &&
                        ((Point)cells[row, col].Btn.Tag).Y + yOfset < cells.GetLength(1) &&
                        cells[((Point)cells[row, col].Btn.Tag).X + xOfset, ((Point)cells[row, col].Btn.Tag).Y + yOfset].Value == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        // Buttons
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
            timer.Stop();

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
            Menu form = new Menu();
            form.Show();
            this.Close();
        }
    }
}
