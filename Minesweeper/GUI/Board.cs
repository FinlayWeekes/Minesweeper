using Minesweeper.AI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Minesweeper.GUI
{
    public partial class Board : Form
    {
        private DisplayCell[,] cells;
        private Difficulty difficulty;
        private Stats statsForm;
        private List<int> times;
        private bool firstClick;
        private string username;
        private int time;
        private int remaningHiddenCells;
        private int remaningMines;
        private int cellSize;
        private int height;
        private int width;
        private int mineCount;
        private int clickCount;
        private int topTimesCount;
        private int betchels;

        private Color hiddenCol;
        private Color revealedCol;
        private Color borderCol;
        private Color explodedMineCol;
        private Color revealedMineCol;
        private Color buttonBackCol;
        private Color buttonForeCol;
        private Color backgroundCol;
        private Dictionary<int, Color> numColours = new Dictionary<int, Color>();

        private DisplayCell GetDisplayCell(Button btn)
        {
            return cells[((Point)btn.Tag).X, ((Point)btn.Tag).Y];
        }

        // Handles the generation of the board
        public Board(Difficulty Difficulty, bool LightMode, string name)
        {
            username = name;
            difficulty = Difficulty;
            height = Difficulty.Height;
            width = Difficulty.Width;
            mineCount = Difficulty.MineCount;
            statsForm = null;
            firstClick = true;

            if (!difficulty.Custom) times = GetTopTimes();

            InitializeComponent();

            clickCount = 0;
            remaningHiddenCells = width * height;
            remaningMines = mineCount;
            MineCountDisplay.Text = remaningMines.ToString();

            cellSize = CalculateCellSize();

            this.ClientSize = new Size(width * cellSize, height * cellSize + Program.HeaderSize);

            CreateColours(LightMode);

            PopulateCells();
        }
        private List<int> GetTopTimes()
        {
            List<int> scores = new List<int>();

            using (StreamReader sr = new StreamReader(TimesTextFileName()))
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
            this.cells = new DisplayCell[width, height];
            int fontSize = Math.Max(cellSize - 19, 5);

            SuspendLayout();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Button btn = new Button();
                    btn.Width = cellSize;
                    btn.Height = cellSize;
                    btn.Location = new Point(x * cellSize, y * cellSize + Program.HeaderSize);
                    btn.Size = new Size(cellSize, cellSize);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.BackColor = hiddenCol;
                    btn.FlatAppearance.BorderColor = borderCol;
                    btn.Font = new Font("Microsoft Sans Serif", fontSize);
                    btn.Tag = new Point(x, y);
                    btn.MouseUp += CellClick;
                    btn.Anchor = AnchorStyles.None;

                    Controls.Add(btn);
                    cells[x, y] = new DisplayCell(btn);

                    if (y - 1 >= 0)
                    {
                        AddAdjacentCells(cells[x, y], cells[x, y - 1]);
                        if (x - 1 >= 0) AddAdjacentCells(cells[x, y], cells[x - 1, y - 1]);
                        if (x + 1 < width) AddAdjacentCells(cells[x, y], cells[x + 1, y - 1]);
                    }
                    if (x - 1 >= 0)
                    {
                        AddAdjacentCells(cells[x, y], cells[x - 1, y]);
                    }
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
                betchels = grid.Find3BV();
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
                        cells[x, y].SetValue(grid.GetCell(x, y).Value);
                    }
                }
            }
        }
        private Grid GetSolvableGrid(int firstCol, int firstRow)
        {
            bool solvable;
            int count = 0;
            Solver solver;
            Grid grid;

            do
            {
                count++;

                int seed = GetSeed();

                if (Program.TestingModeOn) RandomiseStarter(ref firstCol, ref firstRow, seed);
                
                //System.Diagnostics.Debug.WriteLine("");

                System.Diagnostics.Debug.WriteLine("GENERATING NEW BOARD: " + count + " SEED: " + seed);
                //if (count % 1000 == 0) System.Diagnostics.Debug.WriteLine("SEARCHED " + count + " GRIDS");

                grid = new Grid(width, height, firstCol, firstRow, mineCount, seed, false);

                //grid.DebugDisplayRevealedGrid();
                grid.GetCell(firstCol, firstRow).Open();

                solver = new Solver(difficulty, grid);
                //if (solver.IsSolvable()) solvableCount++;
                //System.Diagnostics.Debug.WriteLine("solved " + solvableCount);

                //System.Diagnostics.Debug.WriteLine("Solved");
                solvable = solver.IsSolvable();

                if (solvable)
                {
                    betchels = grid.Find3BV();
                }
            }
            while (!solvable || (!difficulty.Custom && betchels < Program.ConstBetchelsMin[difficulty.NameNum]));
            //while (true);

            System.Diagnostics.Debug.WriteLine("Created after " + count + " grids");
            System.Diagnostics.Debug.WriteLine("Seed: " + grid.Seed);

            return grid;
        }
        private int CalculateCellSize()
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            int maxCellWidth = workingArea.Width / width;
            int maxCellHeight = (workingArea.Height - Program.HeaderSize - Program.TopFormBarHeight) / height;
            int cellSize = Math.Min(maxCellWidth, maxCellHeight);

            return Math.Min(Program.MaxCellSize, cellSize);
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
        private string TimesTextFileName()
        {
            return "Times/" + difficulty.NameString.ToLower() + "Times.txt";
        }
        private void AddAdjacentCells(DisplayCell cell1, DisplayCell cell2)
        {
            cell1.AddAdjacentCell(cell2);
            cell2.AddAdjacentCell(cell1);
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

            this.BackColor = backgroundCol;
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

            backgroundCol = ColorTranslator.FromHtml("#F1F0F0");
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

            backgroundCol = ColorTranslator.FromHtml("#464E56");
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

                        if (!GetDisplayCell(btn).IsHidden) Chord(btn);
                        else if (!GetDisplayCell(btn).IsFlagged) revealedMine = RevealCell(btn);

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
            foreach (DisplayCell cell in GetDisplayCell(btn).AdjacentCells)
            { 
                if (cell.IsHidden)
                {
                    adjacentHiddenCells++;
                    if (cell.IsFlagged)
                    {
                        adjacentFlags++;
                    }
                }
            }

            // opens all adjacent cells if the cells effective value = 0
            if (GetDisplayCell(btn).Value == adjacentFlags)
            {
                foreach (DisplayCell cellToOpen in GetDisplayCell(btn).AdjacentCells)
                {
                    if (!cellToOpen.IsFlagged && cellToOpen.IsHidden)
                    {
                        RevealCell(cellToOpen.Btn);
                    }
                }
            }

            // flags all adjacent cells if the cells value is the number of adjacent cells
            else if (GetDisplayCell(btn).Value == adjacentHiddenCells)
            {
                foreach (DisplayCell cellToFlag in GetDisplayCell(btn).AdjacentCells)
                {
                    if (!cellToFlag.IsFlagged && cellToFlag.IsHidden)
                    {
                        FlagCell(cellToFlag.Btn);
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
                int firstX = ((Point)btn.Tag).X;
                int firstY = ((Point)btn.Tag).Y;

                firstClick = false;
                PopulateMines(firstX, firstY);
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

                foreach (DisplayCell cellToOpen in GetDisplayCell(btn).AdjacentCells)
                {
                    if (cellToOpen.IsHidden)
                    {
                        RevealCell(cellToOpen.Btn);
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
            using (StreamReader sr = new StreamReader(TimesTextFileName()))
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
                using (StreamWriter sw = new StreamWriter(TimesTextFileName()))
                {
                    sw.WriteLine(username + "-" + Convert.ToString(time));
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
            string stringTime = username + "-" + time;
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

            using (StreamWriter sw = new StreamWriter(TimesTextFileName()))
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

            // dissables all buttons
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

            CloseStatsForm();
            statsForm = new Stats(difficulty.NameString, width, height, mineCount, betchels, clickCount, seconds);
            statsForm.Show();
        }
        private bool CheckHasWon()
        {
            return remaningHiddenCells - mineCount == 0;
        }
        private void CloseStatsForm()
        {
            if (statsForm != null)
            {
                statsForm.Hide();
                statsForm = null;
            }
        }


        // Handles testing mode
        private void RandomiseStarter(ref int firstCol, ref int firstRow, int seed)
        {
            Random random = new Random(seed);

            firstCol = random.Next(0, cells.GetLength(0));
            firstRow = random.Next(0, cells.GetLength(1));

            System.Diagnostics.Debug.WriteLine(firstCol + "," + firstRow);
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

            CloseStatsForm();
        }
        private void menuButton_Click(object sender, EventArgs e)
        {
            Menu form = new Menu();
            form.Show();
            CloseStatsForm();
            this.Close();
        }

        // Timer
        private void timer_Tick(object sender, EventArgs e)
        {
            time++;

            if (time % 10 == 0)
            {
                timeLabel.Text = Convert.ToString(time / 10);
            }
        }
    }
}
