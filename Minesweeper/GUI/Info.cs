using System.Windows.Forms;

namespace Minesweeper.GUI
{
    public partial class Info : Form
    {
        public Info(string text)
        {
            InitializeComponent();

            InfoLabel.Text = text;
        }
    }
}
