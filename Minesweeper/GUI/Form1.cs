using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Minesweeper.AI;

namespace Minesweeper.GUI
{
    public partial class Form1 : Form
    {
        private bool lightMode;
        public Form1()
        {
            InitializeComponent();

            lightMode = false;
        }
        private void OpenBoard(Difficulty difficulty)
        {
            Board board = new Board(difficulty, lightMode);
            board.Show();
            this.Hide();
        }

        private void customButton_Click(object sender, EventArgs e)
        {
            CustomForm customForm = new CustomForm(lightMode);
            customForm.Show();
            this.Hide();
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
        private void buttonMaster_Click(object sender, EventArgs e)
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
    }
}
