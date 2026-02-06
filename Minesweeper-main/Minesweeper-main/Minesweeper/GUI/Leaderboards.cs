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

            string[] difNames = GetStandardDifNames();

            foreach (string difName in difNames)
            {
                List<string> times = GetTimes(difName);

                System.Windows.Forms.Label label = GetTimesLabel(difName);

                string betchels = Get3BVMin(difName);

                AddTimesToLabel(label, times, betchels);
            }
        }

        private string[] GetStandardDifNames()
        {
            List<string> namesList = new List<string>();
            using (StreamReader sr = new StreamReader("StandardDifNames.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    namesList.Add(line.ToLower());
                }
            }

            string[] namesArray = new string[namesList.Count];
            for (int i = 0; i < namesList.Count; i++)
            {
                namesArray[i] = namesList[i];
            }

            return namesArray;
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
                case "beginner":
                    {
                        return beginnerTimes;
                    }
                case "amateur":
                    {
                        return amateurTimes;
                    }
                case "intermediate":
                    {
                        return intermediateTimes;
                    }
                case "expert":
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
                case "beginner":
                    {
                        return Convert.ToString(Program.ConstBetchelsMin[0]);
                    }
                case "amateur":
                    {
                        return Convert.ToString(Program.ConstBetchelsMin[1]);
                    }
                case "intermediate":
                    {
                        return Convert.ToString(Program.ConstBetchelsMin[2]);
                    }
                case "expert":
                    {
                        return Convert.ToString(Program.ConstBetchelsMin[3]);
                    }
                default:
                    {
                        return Convert.ToString(Program.ConstBetchelsMin[4]);
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
                label.Text += Convert.ToString(i + 1) + ". " + GetNameFromTime(times[i]) + "- " + FormatTime(GetTimeFromRecord(times[i])) + "\n";
            }

            for (int i = 0; i < Program.TopTimesCount - times.Count; i++)
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

        private void masterTitle_Click(object sender, EventArgs e)
        {
            this.BackgroundImage = Resources.secret;
        }
    }
}
