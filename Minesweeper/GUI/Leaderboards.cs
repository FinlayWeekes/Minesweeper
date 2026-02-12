using Minesweeper.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper.GUI
{
    public partial class Leaderboards : Form
    {
        public Leaderboards()
        {
            InitializeComponent();

            for (int i = 0; i < Program.ConstDifficultyNames.Count; i++)
            {
                List<string> times = GetTimes(Program.ConstDifficultyNames[i]);

                System.Windows.Forms.Label label = GetTimesLabel(i);

                string betchelsLimit = Convert.ToString(Program.ConstBetchelsMin[i]);

                AddTimesToLabel(label, times, betchelsLimit);
            }
        }
        
        private string FormatTime(int time)
        {
            // returns the time in the form: MM:SS.D or SS.D or S.D 
            string stringTime = "";

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
        private void AddTimesToLabel(System.Windows.Forms.Label label, List<string> times, string betchels)
        {
            // times are in the form: {name}-{time}
            label.Text = "";

            for (int i = 0; i < times.Count; i++)
            {
                label.Text += Convert.ToString(i + 1) + ". " + GetNameFromTime(times[i]) + "- " + FormatTime(GetTimeFromRecord(times[i])) + "\n";
            }

            for (int i = 0; i < Program.TopTimesCount - times.Count; i++)
            {
                label.Text += "\n";
            }

            label.Text += "\n3BV Minimum: " + betchels;
        }
        private System.Windows.Forms.Label GetTimesLabel(int dif)
        {
            // cannot use a dictionary as a dictionary of labels cannot exist as a property
            switch (dif)
            {
                case 0:
                    {
                        return beginnerTimes;
                    }
                case 1:
                    {
                        return amateurTimes;
                    }
                case 2:
                    {
                        return intermediateTimes;
                    }
                case 3:
                    {
                        return expertTimes;
                    }
                default:
                    {
                        return masterTimes;
                    }
            }
        }
        
        
        // methods that handle getting the data from text files
        private List<string> GetTimes(string difName)
        {
            List<string> times = new List<string>();

            using (StreamReader sr = new StreamReader(TimesTextFileName(difName)))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    times.Add(line);
                }
            }

            return times;
        }
        private string TimesTextFileName(string difName)
        {
            return "Times/" + difName.ToLower() + "Times.txt";
        }
        private int GetTimeFromRecord(string stringTime)
        {
            string[] splitTime = stringTime.Split('-');
            return Convert.ToInt32(splitTime[splitTime.Length - 1]);
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
        
        
        // buttons
        private void menuButton_Click(object sender, EventArgs e)
        {
            Menu form = new Menu();
            form.Show();
            this.Close();
        }
        private void masterTitle_Click(object sender, EventArgs e)
        {
            this.BackgroundImage = Resources.dif5;
        }

    }
}
