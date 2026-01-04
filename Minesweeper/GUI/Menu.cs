using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Minesweeper.AI;

namespace Minesweeper.GUI
{
    public partial class Menu : Form
    {
        private string name;
        private bool lightMode;

        private const int maxNameSize = 16;
        private const string defaultName = "Guest";

        public Menu()
        {
            InitializeComponent();

            name = GetRecentName();
            nameTextBox.Text = name;

            lightMode = false;
        }
        
        private void OpenBoard(Difficulty difficulty)
        {
            HideForm();
            Board board = new Board(difficulty, lightMode, name);
            board.Show();
        }
        private string GetRecentName()
        {
            string name;
            using (StreamReader sr = new StreamReader("recentName.txt"))
            {
                name = sr.ReadLine();
            }

            if (name == null) name = defaultName;

            return name;
        }
        private void UpdateRecentName()
        {
            name = nameTextBox.Text;
            if (name.Length > 16) name = name.Substring(0, 16);

            // need to adde regex if the name is all spaces
            if (name == null || name == "") name = defaultName;

            using (StreamWriter sw = new StreamWriter("recentName.txt"))
            {
                sw.WriteLine(name);
            }
        }
        private void HideForm()
        {
            UpdateRecentName();

            this.Hide();
        }

        private void customButton_Click(object sender, EventArgs e)
        {
            HideForm();
            CustomForm customForm = new CustomForm(lightMode, name);
            customForm.Show();
        }
        private void beginnerButton_Click(object sender, EventArgs e)
        {
            OpenBoard(new Difficulty(1));
        }
        private void amateurButton_Click(object sender, EventArgs e)
        {
            OpenBoard(new Difficulty(2));
        }
        private void intermediateButton_Click(object sender, EventArgs e)
        {
            OpenBoard(new Difficulty(3));
        }
        private void expertButton_Click(object sender, EventArgs e)
        {
            OpenBoard(new Difficulty(4));
        }
        private void masterButton_Click(object sender, EventArgs e)
        {
            OpenBoard(new Difficulty(5));
        }

        private void ColourModeButton_Click(object sender, EventArgs e)
        {
            if (lightMode)
            {
                lightMode = false;
                ColourModeButton.Text = "Theme: Dark";
            }
            else
            {
                lightMode = true;
                ColourModeButton.Text = "Theme: Light";
            }
        }
        private void leaderboardBtn_Click(object sender, EventArgs e)
        {
            HideForm();
            Leaderboards leaderboards = new Leaderboards();
            leaderboards.Show();
        }
    }
}
