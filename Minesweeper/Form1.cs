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

        private void customButton_Click(object sender, EventArgs e)
        {
            CustomForm customForm = new CustomForm();
            customForm.Show();
            this.Hide();
        }

        private void beginnerButton_Click(object sender, EventArgs e)
        {

        }

        private void amateurButton_Click(object sender, EventArgs e)
        {

        }

        private void expertButton_Click(object sender, EventArgs e)
        {

        }

        private void buttonMaster_Click(object sender, EventArgs e)
        {

        }

        private void intermediateButton_Click(object sender, EventArgs e)
        {

        }
    }
}
