using System;
using System.Windows.Forms;

namespace Minesweeper.GUI
{
    public partial class Stats : Form
    {
        public Stats(string difficulty, int width, int height, int mineCount, int betchels, int clicks, double time)
        {
            InitializeComponent();

            StatsDifficulty.Text += difficulty;
            StatsSize.Text += FormatDimensions(width, height);
            StatsMineCount.Text += mineCount;
            Stats3BV.Text += betchels;
            StatsTime.Text += FormatTime(time);
            StatsClicks.Text += clicks;
            StatsRate.Text += CalcRate(betchels, time);
            StatsRPQ.Text += CalcRPQ(betchels, clicks) + "%";
            StatsIOS.Text += CalcIOS(betchels, clicks, time);
        }

        private string FormatDimensions(int width, int height)
        {
            return Convert.ToString(width) + " x " + Convert.ToString(height);
        }
        private string FormatTime(double time)
        {
            return Convert.ToString(Math.Floor(time / 600)) + ":" + Convert.ToString((int)(time/10) % 60) + "." + Convert.ToString(time % 10);
        }
        private double CalcRate(int betchels, double time)
        {
            return Math.Round(Convert.ToDouble(betchels) / Convert.ToDouble(time), 3);
        }
        private double CalcRPQ(int betchels, int clicks)
        {
            return Math.Round(Convert.ToDouble(betchels) / Convert.ToDouble(clicks) * 100, 3);
        }
        private double CalcIOS(int betchels, int clicks, double time)
        {
            return Math.Round((Convert.ToDouble(betchels * betchels)) / (Convert.ToDouble(time * clicks)), 3);
        }
    }
}
