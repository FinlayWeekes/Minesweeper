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
    public partial class CustomForm : Form
    {
        public CustomForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int height = int.Parse(this.heightTextBox.Text);
            int width = int.Parse(this.widthTextBox.Text);
            int mineCount = int.Parse(this.mineCountTextBox.Text);

            Board board = new Board(height, width, mineCount);
            board.Show();
            this.Hide();
        }
    }
}
