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
    public partial class Stats : Form
    {
        public Stats(string difficulty, int width, int height, int mineCount, int betchels, int clicks, int time)
        {
            InitializeComponent();

            StatsDifficulty.Text += difficulty;
            StatsSize.Text += Convert.ToString(width) + " x " + Convert.ToString(height);
            StatsMineCount.Text += mineCount;
            Stats3BV.Text += betchels;
            StatsTime.Text += Convert.ToString(time/60) + ":" + Convert.ToString(time%60);
            StatsClicks.Text += clicks;
            StatsRate.Text += Math.Round(Convert.ToDouble(betchels) / Convert.ToDouble(clicks), 3);
            StatsRPQ.Text += Math.Round(Convert.ToDouble(betchels) / Convert.ToDouble(time) * 100, 3) + "%";
            StatsIOS.Text += Math.Round((Convert.ToDouble(betchels * betchels)) / (Convert.ToDouble(time * clicks)), 3);
        }
    }
}
