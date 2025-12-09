using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void OpenBoard(int width, int height, int mineCount, string difficulty)
        {
            Board board = new Board(width, height, mineCount, difficulty);
            board.Show();
            this.Hide();
        }

        private void customButton_Click(object sender, EventArgs e)
        {
            CustomForm customForm = new CustomForm();
            customForm.Show();
            this.Hide();
        }

        private void beginnerButton_Click(object sender, EventArgs e)
        {
            OpenBoard(10, 10, 10, "Beginner");
        }

        private void amateurButton_Click(object sender, EventArgs e)
        {
            OpenBoard(12, 12, 20, "Amateur");
        }
        private void intermediateButton_Click(object sender, EventArgs e)
        {
            OpenBoard(20, 20, 40, "Intermediate");
        }

        private void expertButton_Click(object sender, EventArgs e)
        {
            OpenBoard(30, 16, 99, "Expert");
        }

        private void buttonMaster_Click(object sender, EventArgs e)
        {
            OpenBoard(40, 20, 200, "Master");
        }
    }
}
