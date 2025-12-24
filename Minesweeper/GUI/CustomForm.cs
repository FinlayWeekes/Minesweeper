using System;
using System.Windows.Forms;
using Minesweeper.AI;

namespace Minesweeper.GUI
{
    public partial class CustomForm : Form
    {
        private bool lightMode;
        public CustomForm(bool LightMode)
        {
            InitializeComponent();
            LightMode = lightMode;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int height = int.Parse(this.heightTextBox.Text);
            int width = int.Parse(this.widthTextBox.Text);
            int mineCount = int.Parse(this.mineCountTextBox.Text);

            Board board = new Board(new Difficulty(width, height, mineCount, null), lightMode);
            board.Show();
            this.Hide();
        }
    }
}
