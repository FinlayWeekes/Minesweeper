using System;
using System.Windows.Forms;
using Minesweeper.AI;

namespace Minesweeper.GUI
{
    public partial class CustomForm : Form
    {
        private bool lightMode;
        private string name;
        public CustomForm(bool LightMode, string Name)
        {
            InitializeComponent();

            name = Name;
            LightMode = lightMode;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int height = int.Parse(this.heightTextBox.Text);
            int width = int.Parse(this.widthTextBox.Text);
            int mineCount = int.Parse(this.mineCountTextBox.Text);

            Board board = new Board(new Difficulty(width, height, mineCount, null), lightMode, name);
            board.Show();
            this.Hide();
        }
    }
}
