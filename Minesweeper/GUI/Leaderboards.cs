using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper.GUI
{
    public partial class Leaderboards : Form
    {
        private int topTimesCount;
        private int[] betchelsMin;

        public Leaderboards(int topTimesCount, int[] betchelsMin)
        {
            this.topTimesCount = topTimesCount;
            this.betchelsMin = betchelsMin;

            InitializeComponent();

            string[] difNames = { "Beginner", "Amateur", "Intermediate", "Expert", "Master" };

            foreach (string difName in difNames)
            {
                List<string> times = GetTimes(difName);

                System.Windows.Forms.Label label = GetTimesLabel(difName);

                string betchels = Get3BVMin(difName);

                AddTimesToLabel(label, times, betchels);
            }
        }

        private List<string> GetTimes(string difName)
        {
            List<string> times = new List<string>();

            using (StreamReader sr = new StreamReader(TimesTextName(difName)))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    times.Add(line);
                }
            }

            return times;
        }
        private System.Windows.Forms.Label GetTimesLabel(string dif)
        {
            switch (dif)
            {
                case "Beginner":
                    {
                        return beginnerTimes;
                    }
                case "Amateur":
                    {
                        return amateurTimes;
                    }
                case "Intermediate":
                    {
                        return intermediateTimes;
                    }
                case "Expert":
                    {
                        return expertTimes;
                    }
                default:
                    {
                        return masterTimes;
                    }
            }
        }
        private string Get3BVMin(string dif)
        {
            switch (dif)
            {
                case "Beginner":
                    {
                        return Convert.ToString(betchelsMin[0]);
                    }
                case "Amateur":
                    {
                        return Convert.ToString(betchelsMin[1]);
                    }
                case "Intermediate":
                    {
                        return Convert.ToString(betchelsMin[2]);
                    }
                case "Expert":
                    {
                        return Convert.ToString(betchelsMin[3]);
                    }
                default:
                    {
                        return Convert.ToString(betchelsMin[4]);
                    }
            }
        }
        private string TimesTextName(string difName)
        {
            return "Times/" + difName.ToLower() + "Times.txt";
        }


        private void AddTimesToLabel(System.Windows.Forms.Label label, List<string> times, string betchels)
        {
            // times are in the form: {name}-{time}
            label.Text = "";

            for (int i = 0; i < times.Count; i++)
            {
                label.Text += Convert.ToString(i + 1) + ". " + GetNameFromTime(times[i]) + "- " + FormatTime(times[i]) + "\n";
            }

            for (int i = 0; i < topTimesCount - times.Count; i++)
            {
                label.Text += "\n";
            }

            label.Text += "\n3BV Minimum: " + betchels;
        }
        private string GetNameFromTime(string record)
        {
            string[] splitRecord = record.Split('-');
            string name = splitRecord[0];
            
            // itterates incase the user has a "-" in their name which is the character used to sepparate the name and time in the text file
            for (int i = 1; i < splitRecord.Length-1; i++)
            {
                name += "-" + splitRecord[i];
            }

            return name;
        }
        private string FormatTime(string stringTime)
        {
            // returns the time in the form: MM:SS.D or SS.D or S.D 

            int time = GetTimeFromRecord(stringTime);
            stringTime = "";

            int mins = time / 600;
            int secs = ((time - mins*600) % 600)/10;
            int oneDP = (time - secs * 10) % 10;

            if (mins > 0)
            {
                stringTime += mins + ":";
                
                if (secs <= 9)
                {
                    stringTime += "0";
                }
            }

            stringTime += secs + "." + oneDP;

            return stringTime;
        }
        private int GetTimeFromRecord(string stringTime)
        {
            string[] splitTime = stringTime.Split('-');
            return Convert.ToInt32(splitTime[splitTime.Length - 1]);
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            Menu form = new Menu();
            form.Show();
            this.Close();
        }
    }
}
