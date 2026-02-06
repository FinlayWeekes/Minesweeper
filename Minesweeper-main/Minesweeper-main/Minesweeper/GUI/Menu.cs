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

        public Menu()
        {
            InitializeComponent();
            name = GetRecentName();
            nameTextBox.Text = name;


            lightMode = false;
        }
        
        private void OpenBoard(Difficulty difficulty)
        {
            // updateRecentName will return false if the name isnt valid
            if (!UpdateRecentName()) return;

            Board board = new Board(difficulty, lightMode, name);
            this.Hide();
            board.Show();
        }
        private string GetRecentName()
        {
            string name;
            using (StreamReader sr = new StreamReader("recentName.txt"))
            {
                name = sr.ReadLine();
            }

            if (name == null) name = Program.DefaultName;

            return name;
        }
        private bool UpdateRecentName()
        {
            // returns false if its an invalid name
            name = nameTextBox.Text;

            if (name.Length > Program.MaxNameSize)
            {
                OutputToUser("Name is too long", true);
                return false;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = Program.DefaultName;
            }

            using (StreamWriter sw = new StreamWriter("recentName.txt"))
            {
                sw.WriteLine(name);
            }

            return true;    
        }
        private void OutputToUser(string line, bool error)
        {
            System.Drawing.Color foreColour = System.Drawing.Color.Black;

            if (error) foreColour = System.Drawing.Color.Red;

            debugLabel.ForeColor = foreColour;
            debugLabel.Text = line;
        }


        private void customButton_Click(object sender, EventArgs e)
        {
            if (!UpdateRecentName()) return;
            CustomForm customForm = new CustomForm(lightMode, name);
            this.Hide();
            customForm.Show();
        }
        private void beginnerButton_Click(object sender, EventArgs e)
        {
            OpenBoard(new Difficulty("beginner"));
        }
        private void amateurButton_Click(object sender, EventArgs e)
        {
            OpenBoard(new Difficulty("amateur"));
        }
        private void intermediateButton_Click(object sender, EventArgs e)
        {
            OpenBoard(new Difficulty("intermediate"));
        }
        private void expertButton_Click(object sender, EventArgs e)
        {
            OpenBoard(new Difficulty("expert"));
        }
        private void masterButton_Click(object sender, EventArgs e)
        {
            OpenBoard(new Difficulty("master"));
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
            if (!UpdateRecentName()) return;

            Leaderboards leaderboards = new Leaderboards();
            this.Hide();
            leaderboards.Show();
        }
    }
}
