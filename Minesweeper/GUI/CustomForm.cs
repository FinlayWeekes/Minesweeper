using Minesweeper.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Minesweeper.GUI
{
    public partial class CustomForm : Form
    {
        private bool lightMode;
        private string userName;
        private int topTimesCount;
        private string[] difNames;
        private string[] standardDifNames;
        System.Windows.Forms.ListView[] lists;
        private const int namePadRight = 24;
        private enum State
        {
            NotIncluded = 0,
            Optional = 1,
            Essential = 2,
        }

        public CustomForm(bool LightMode, string UserName, int TopTimesCount)
        {
            this.topTimesCount = TopTimesCount;
            this.userName = UserName;
            this.lightMode = LightMode;
            this.difNames = new string[] { "beginner", "amateur", "intermediate", "expert", "master" };
            this.standardDifNames = GetStandardDifNames();

            InitializeComponent();

            this.lists = new System.Windows.Forms.ListView[] { this.beginnerPatList, this.amateurPatList, this.intermediatePatList, this.expertPatList, this.masterPatList };

            PopulatePatternLists();
            
            beginnerPatList.MouseClick += beginnerPatList_MouseClick;
            amateurPatList.MouseClick += beginnerPatList_MouseClick;
            intermediatePatList.MouseClick += beginnerPatList_MouseClick;
            expertPatList.MouseClick += beginnerPatList_MouseClick;
            masterPatList.MouseClick += beginnerPatList_MouseClick;
        }

        
        // Handles generating pattern lists
        private void PopulatePatternLists()
        {
            // beginner has no essential patterns so this is needed
            string[] beginnerPatterns = new string[] { "B1".PadRight(namePadRight), "B2".PadRight(namePadRight) };
            AddNamesToList(beginnerPatterns, beginnerPatList);

            // goes throuh each difficulty exepty beginner
            for (int i = 1; i < difNames.Length; i++)
            {
                string[] patNames = GetDiffPatternNames(difNames[i]);
                AddNamesToList(patNames, lists[i]);
            }
        }
        private void AddNamesToList(string[] names, System.Windows.Forms.ListView list)
        {
            foreach (string patternName in names)
            {
                // the names are padded right to make it easier for the user to click short names
                ListViewItem pattern = new ListViewItem(patternName.PadRight(namePadRight));
                
                pattern.StateImageIndex = (int)State.NotIncluded;
                list.Items.Add(pattern);
            }
        }
        private string[] GetDiffPatternNames(string difName)
        {
            using (StreamReader sr = new StreamReader(GetTextFileName(difName)))
            {
                for (int i = 0; i < 15; i++)
                {
                    sr.ReadLine();
                }

                return sr.ReadLine().Split(',');
            }
        }

        
        // handles difficulties in text files
        private string GetTextFileName(string difName)
        {
            return "Difficulties/" + difName.ToLower() + "Difficulty.txt";
        }
        private void CreateNewDifFile(string name, int height, int width, int mineCount, List<string> easyNames, List<string> optionalNames, List<string> essentialNames)
        {
            using (StreamWriter sw = new StreamWriter(GetTextFileName(name)))
            {
                sw.WriteLine("Name: ");
                sw.WriteLine(name);

                sw.WriteLine("Width: ");
                sw.WriteLine(width);

                sw.WriteLine("Height: ");
                sw.WriteLine(height);

                sw.WriteLine("MineCount: ");
                sw.WriteLine(mineCount);

                sw.WriteLine("Custom: ");
                sw.WriteLine("true");

                sw.WriteLine("EasyPatterns: ");
                sw.WriteLine(ListToString(name, easyNames));

                sw.WriteLine("OptionalPatterns: ");
                sw.WriteLine(ListToString(name, optionalNames));

                sw.WriteLine("EssentialPatterns: ");
                sw.WriteLine(ListToString(name, essentialNames));
            }
        }
        private string ListToString(string name, List<string> list)
        {
            if (list.Count == 0) return "null";

            string returnString = list[0];
            if (list.Count == 1) return returnString;

            for (int i = 1; i < list.Count; i++)
            {
                returnString += "," + list[i];
            }

            return returnString;
        }
             
        
        // handles when the patterns are clicked
        private void ListClicked(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.ListView list = (System.Windows.Forms.ListView)sender;

            ListViewItem pattern = list.GetItemAt(e.X, e.Y);
            if (pattern == null) return;

            CycleState(pattern);

            // stops the text from being perma highlighted
            pattern.Selected = false;
        }
        private void CycleState(ListViewItem pattern)
        {
            pattern.StateImageIndex = (pattern.StateImageIndex + 1) % 3;
        }


        // handles getting the data the user selected
        private (List<string> easyPatternNames,  List<string> optionalPatternNames, List<string> essentialPatternNames) GetSelectedPatternNames()
        {
            // puts the selected beginner patterns in easy patterns
            List<string> easyPatterns = new List<string>();
            foreach (ListViewItem pattern in beginnerPatList.Items)
            {
                if ((State)pattern.StateImageIndex != State.NotIncluded)
                {
                    easyPatterns.Add(RemoveExcessSpaces(pattern.Text));

                }
            }

            // puts the pattern names into each list from each difficulty
            List<string> optionalPatterns = new List<string>();
            List<string> essentialPatterns = new List<string>();
            for (int difIndex = 1; difIndex < difNames.Length; difIndex++)
            {
                foreach (ListViewItem pattern in lists[difIndex].Items)
                {
                    if ((State)pattern.StateImageIndex == State.Optional)
                    {
                        optionalPatterns.Add(RemoveExcessSpaces(pattern.Text));
                    }
                    else if ((State)pattern.StateImageIndex == State.Essential)
                    {
                        essentialPatterns.Add(RemoveExcessSpaces(pattern.Text));
                    }
                }
            }

            return (easyPatterns, optionalPatterns, essentialPatterns);
        }
        private (int width, int height, int mineCount, string name, bool valid) GetBoardData()
        {
            string name = "";
            int width = 0;
            int height = 0;
            int mineCount = 0;
            bool valid = false;

            try
            {
                 height = int.Parse(this.heightTextBox.Text);
            }
            catch
            {
                OutputToUser("Height value is not in the right format", true);
                return (width, height, mineCount, name, valid);
            }
            if (height <= 0)
            {
                OutputToUser("Height value must be positive", true);
                return (width, height, mineCount, name, valid);
            }

            try
            {
                width = int.Parse(this.widthTextBox.Text);
            }
            catch
            {
                OutputToUser("Width value is not in the right format", true);
                return (width, height, mineCount, name, valid);
            }
            if (width <= 0)
            {
                OutputToUser("Width value must be positive", true);
                return (width, height, mineCount, name, valid);
            }

            try
            {
                mineCount = int.Parse(this.mineCountTextBox.Text);
            }
            catch
            {
                OutputToUser("Mine count value is not in the right format", true);
                return (width, height, mineCount, name, valid);
            }
            if (mineCount < 0)
            {
                OutputToUser("Mine count must not be negative", true);
                return (width, height, mineCount, name, valid);
            }
            // makes sure there is not too many mines so that the first cell can always be empty
            // will check clear conditions if board has been decied it is not random
            else if (mineCount > width * height - 1)
            {
                OutputToUser("Mine count is too large", true);
                return (width, height, mineCount, name, valid);
            }


            valid = true; 
            name = difTextBox.Text;

            return (width, height, mineCount, name, valid);
        }
        private string RemoveExcessSpaces(string name)
        {
            string returnName = "";

            foreach(char c in name)
            {
                if (c == ' ') return returnName;

                returnName += c;
            }

            return returnName;
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
        private bool NameIsTaken(string name)
        {
            foreach (string standardName in standardDifNames)
            {
                if (standardName == name) return true;
            }
            return false;
        }
        private bool NameHasInvalidChars(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return true;

            char[] invalidChars = Path.GetInvalidFileNameChars().Concat(new[] { ' ', '.' }).ToArray();

            foreach (char invalidChar in invalidChars)
            {
                foreach (char c in name)
                {
                    if (c == invalidChar)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private bool IsInvalidMineCount (int mineCount, int height, int width)
        {
            return mineCount > (width * height) - (Math.Min(width, 3) * Math.Min(height, 3));
        }


        // handles loading the difficulties
        private void loadButton_Click(object sender, EventArgs e)
        {
            string difName = difTextBox.Text;

            if (NameHasInvalidChars(difName))
            {
                OutputToUser("Difficulty name is not in the right format", true);
                return;
            }

            if (!File.Exists(GetTextFileName(difName)))
            {
                OutputToUser("Difficulty does not exist", true);
                return;
            }

            // no easyNames as easy patterns are shows the same as optional patterns to the user so they are combined
            string[] optionalNames;
            string[] essentialNames;
            using (StreamReader sr = new StreamReader(GetTextFileName(difName)))
            {
                // the actual name does not need to be read
                sr.ReadLine();
                sr.ReadLine();

                sr.ReadLine();
                widthTextBox.Text = sr.ReadLine();

                sr.ReadLine();
                heightTextBox.Text = sr.ReadLine();

                sr.ReadLine();
                mineCountTextBox.Text = sr.ReadLine();

                // custom bool does not need to be read
                sr.ReadLine();
                sr.ReadLine();

                sr.ReadLine();
                string easyLine = sr.ReadLine();

                sr.ReadLine();
                optionalNames = (sr.ReadLine() + "," + easyLine).Split(',');

                sr.ReadLine();
                essentialNames = sr.ReadLine().Split(',');
            }

            // sets the states of each pattern instead of storing the patterns as strings to pass so that the use can see what patterns there are
            string[][] nameStates = new string[][] { optionalNames, essentialNames };
            foreach (System.Windows.Forms.ListView list in lists)
            {
                foreach (ListViewItem item in list.Items)
                {
                    item.StateImageIndex = (int)GetStateToLoad(RemoveExcessSpaces(item.Text), nameStates);
                }
            }

            OutputToUser("Difficulty successfully loaded", false);
        }
        private State GetStateToLoad(string name, string[][] nameStates)
        {
            for (int state = 0; state < nameStates.Length; state++)
            {
                foreach (string nameState in nameStates[state])
                {
                    if (nameState == name) return (State)(state + 1);
                }
            }

            return State.NotIncluded;
        }


        // other
        private void OutputToUser(string line, bool error)
        {
            System.Drawing.Color foreColour = System.Drawing.Color.Black;

            if (error) foreColour = System.Drawing.Color.Red;

            debugLabel.ForeColor = foreColour;
            debugLabel.Text = line;
        }


        // buttons
        private void generateButton_Click(object sender, EventArgs e)
        {
            (int width, int height, int mineCount, string name, bool valid) dataTup = GetBoardData();
            if (!dataTup.valid) return;

            (List<string> easyPatternNames, List<string> optionalPatternNames, List<string> essentialPatternNames) paternNamesTup = GetSelectedPatternNames();
            
            // has already checked that the minecount is less than width x height
            // needs to check if its possible for the adjacent cell to be clear in every situation (only not random boards)
            // takes into account boards with a dimension less than 4
            bool isRandom = paternNamesTup.easyPatternNames.Count == 0 && paternNamesTup.optionalPatternNames.Count == 0 && paternNamesTup.essentialPatternNames.Count == 0;
            if (!isRandom && IsInvalidMineCount(dataTup.mineCount, dataTup.height, dataTup.width))
            {
                OutputToUser("Mine count is too large", true);
                return;
            }

            Difficulty difficulty = new Difficulty(dataTup.width, dataTup.height, dataTup.mineCount, paternNamesTup.easyPatternNames, paternNamesTup.optionalPatternNames, paternNamesTup.essentialPatternNames);
            Board board = new Board(difficulty, lightMode, userName, topTimesCount);
            board.Show();
            this.Hide();
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            (int width, int height, int mineCount, string name, bool valid) dataTup = GetBoardData();
            if (!dataTup.valid) return;

            // checks that the name isnt one of the defined ones such as "beginner" or "master"
            // and checks if the name can be stored as a text file
            // formatting validation is already done by the GetBoardData method
            if (NameHasInvalidChars(dataTup.name))
            {
                OutputToUser("Difficulty name is not in the right format", true);
                return;
            }
            if (NameIsTaken(dataTup.name))
            {
                OutputToUser("Difficulty name is invalid", true);
                return;
            }

            (List<string> easyPatternNames, List<string> optionalPatternNames, List<string> essentialPatternNames) paternNamesTup = GetSelectedPatternNames();

            CreateNewDifFile(dataTup.name, dataTup.height, dataTup.width, dataTup.mineCount, paternNamesTup.easyPatternNames, paternNamesTup.optionalPatternNames, paternNamesTup.essentialPatternNames);

            OutputToUser("Difficulty successfully saved", false);
        }

        private void beginnerPatList_MouseClick(object sender, MouseEventArgs e)
        {
            ListClicked(sender, e);
        }
        private void amateurPatList_MouseClick(object sender, MouseEventArgs e)
        {
            ListClicked(sender, e);
        }
        private void intermediatePatList_MouseClick(object sender, MouseEventArgs e)
        {
            ListClicked(sender, e);
        }
        private void expertPatList_MouseClick(object sender, MouseEventArgs e)
        {
            ListClicked(sender, e);
        }
        private void masterPatList_MouseClick(object sender, MouseEventArgs e)
        {
            ListClicked(sender, e);
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            Menu form = new Menu();
            form.Show();
            this.Close();
        }
    }
}
