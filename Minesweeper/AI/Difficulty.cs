using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Minesweeper.AI
{
    public class Difficulty
    {
        public HashSet<Func<LogicCell, Grid, int, int, bool>> EasyPatterns
        {
            get
            {
                return easyPatterns;
            }
        }
        private HashSet<Func<LogicCell, Grid, int, int, bool>> easyPatterns;
        public HashSet<Func<LogicCell, Grid, int, int, bool>> OptionalPatterns
        {
            get
            {
                return optionalPatterns;
            }
        }
        private HashSet<Func<LogicCell, Grid, int, int, bool>> optionalPatterns;
        public HashSet<Func<LogicCell, Grid, int, int, bool>> EssentialPatterns
        {
            get
            {
                return essentialPatterns;
            }
        }
        private HashSet<Func<LogicCell, Grid, int, int, bool>> essentialPatterns;
        public int Width
        {
            get
            {
                return width;
            }
        }
        private int width;
        public int Height
        {
            get
            {
                return height;
            }
        }
        private int height;
        public int MineCount
        {
            get
            {
                return mineCount;
            }
        }
        private int mineCount;
        public int NameNum
        {
            get { return nameNum; }
        }
        private int nameNum;
        public string NameString
        {
            get
            {
                if (custom) return "Custom";
                return nameString;
            }
        }
        private string nameString;
        public bool Custom
        {
            get
            {
                return custom;
            }
        }
        private bool custom;
        public bool Random
        {
            get { return random; }
        }
        private bool random;
        public bool MineCounting
        { 
            get
            {
                return mineCounting;
            }
        }
        private bool mineCounting;
        public bool MineCountingEssential
        {
            get
            {
                return mineCountingEssential;
            }
        }
        private bool mineCountingEssential;

        private Dictionary<string, Func<LogicCell, Grid, int, int, bool>> patternsDict = new Dictionary<string, Func<LogicCell, Grid, int, int, bool>>
        {
            // Beginner
            { "B1", BeginnerAI.PB1 },
            { "B2", BeginnerAI.PB2 },

            // Amateur
            { "1-1C", AmateurAI.P1_1C },
            { "2-1", AmateurAI.P2_1 },

            // Intermediate
            { "1-1CR", IntermediateAI.P1_1CR },
            { "2-1C", IntermediateAI.P2_1C },
            { "2-1R", IntermediateAI.P2_1R },
            { "2-1CR", IntermediateAI.P2_1CR },
            { "H1", IntermediateAI.PH1 },
            { "H2", IntermediateAI.PH2 },
            { "H3", IntermediateAI.PH3 },

            // Expert
            { "1-1CR+", ExpertAI.P1_1CRx },
            { "2-1CR+", ExpertAI.P2_1CRx },
            { "2-1R+", ExpertAI.P2_1Rx },
            { "H1R", ExpertAI.PH1R },
            { "H2R", ExpertAI.PH2R },
            { "H3R", ExpertAI.PH3R },
            { "1-1T", ExpertAI.P1_1T },
            { "2-2T", ExpertAI.P2_2T},
            { "3-2T", ExpertAI.P3_2T },

            // Master
            { "1-1TR", MasterAI.P1_1TR },
            { "2-2TR", MasterAI.P2_2TR },
            { "3-2TR", MasterAI.P3_2TR },
            { "1-3-1VC", MasterAI.P1_3_1VC },
            { "1-3-1VCR", MasterAI.P1_3_1VCR },
            { "2-2-2VC", MasterAI.P2_2_2VC },
            { "2-2-2VCR", MasterAI.P2_2_2VCR },
            { "T-Pattern", MasterAI.PT_Pattern}
        };

        public Difficulty(int nameNum)
        {
            // nameNum is the index for the string name in the Program Class
            this.nameNum = nameNum;
            this.nameString = Program.ConstDifficultyNames[nameNum];
            string[] easyNames;
            string[] optionalNames;
            string[] essentialNames;
            mineCounting = false;
            mineCountingEssential = false;

            using (StreamReader sr = new StreamReader(GetTextFileName(nameString)))
            {
                sr.ReadLine();
                this.nameString = sr.ReadLine();

                sr.ReadLine();
                this.width = Convert.ToInt32(sr.ReadLine());

                sr.ReadLine();
                this.height = Convert.ToInt32(sr.ReadLine());

                sr.ReadLine();
                this.mineCount = Convert.ToInt32(sr.ReadLine());

                sr.ReadLine();
                this.custom = Convert.ToBoolean(sr.ReadLine());

                sr.ReadLine();
                easyNames = sr.ReadLine().Split(',');

                sr.ReadLine();
                optionalNames = sr.ReadLine().Split(',');

                sr.ReadLine();
                essentialNames = sr.ReadLine().Split(',');
            }

            this.easyPatterns = GetPatterns(ArrayToHashSet(easyNames), false);
            this.optionalPatterns = GetPatterns(ArrayToHashSet(optionalNames), false);
            this.essentialPatterns = GetPatterns(ArrayToHashSet(essentialNames), true);

            CheckForRandom();

            // returns true if Program.TestingModeOn is true, there is enough data to be analysed and its not custom
            if (CanDoTestingModeOn())
            {
                StartTest();
            }
        }
        public Difficulty(int width, int height, int mineCount, HashSet<string> easyPatternNames, HashSet<string> optionalPatternNames, HashSet<string> essentialPatternNames)
        {
            this.mineCounting = false;
            this.mineCountingEssential = false;
            this.easyPatterns = GetPatterns(easyPatternNames, false);
            this.optionalPatterns = GetPatterns(optionalPatternNames, false);
            this.essentialPatterns = GetPatterns(essentialPatternNames, true);
            this.width = width;
            this.height = height;
            this.mineCount = mineCount;
            this.custom = true;

            CheckForRandom();
        }

        private void CheckForRandom()
        {
            random = easyPatterns.Count == 0 && optionalPatterns.Count == 0 && essentialPatterns.Count == 0 && !mineCounting; 
        }
        private string GetTextFileName(string difficultyName)
        {
            return "Difficulties/" + difficultyName.ToLower() + "Difficulty.txt";
        }
        private HashSet<string> ArrayToHashSet(string[] array)
        {
            HashSet<string> set = new HashSet<string>();
            foreach (string s in array)
            {
                set.Add(s); 
            }
            return set;
        }
        private HashSet<Func<LogicCell, Grid, int, int, bool>> GetPatterns(HashSet<string> patternNames, bool essential)
        {
            HashSet<Func<LogicCell, Grid, int, int, bool>> patterns = new HashSet<Func<LogicCell, Grid, int, int, bool>>();

            if (patternNames.Count == 0 || (patternNames.Count == 1 && patternNames.Single() == "null")) return patterns;

            foreach (string pattern in patternNames)
            {
                if (pattern == "MineCounting")
                {
                    mineCounting = true;
                    mineCountingEssential = essential;
                }
                else patterns.Add(patternsDict[pattern]);
            }

            return patterns;
        }


        // handles the testing mode which is run when the difficulty is selected and the constant bool TestingModeOn is true in Program
        private bool CanDoTestingModeOn()
        {
            if (!Program.TestingModeOn || custom) return false;

            using (StreamReader sr = new StreamReader(Program.Get3BVTextFileName(nameString)))
            {
                for (int i = 0; i < 3; i++)
                {
                    if (sr.ReadLine() == null) return false;
                }
            }

            return true;
        }
        private void StartTest()
        {
            List<int> values3BV = Get3BVValues();
            values3BV.Sort();

            double standDev = Find3BVStandardDeviation(values3BV);
            double mean = values3BV.Average();

            int count = values3BV.Count();
            int min = values3BV[0];
            int max = values3BV[count - 1];

            double median = values3BV[count / 2];
            if (count % 2 == 0)
            {
                median = (median + values3BV[(count / 2) + 1]) / 2;
            }

            int boardsGenerated = int.Parse(GetData(Program.GetBoardCountFileName(nameString)));
            int boardsSolved = int.Parse(GetData(Program.GetSolvableFileName(nameString)));
            double fractionSolved = Convert.ToDouble(boardsSolved) / Convert.ToDouble(boardsGenerated);
            double meanTime = Convert.ToDouble(GetData(Program.GetMeanTimeFileName(nameString)));

            int betchelsLimit = Program.ConstBetchelsMin[nameNum];
            int solvableUnderLimit = values3BV.IndexOf(betchelsLimit);
            double fractionLimitValid = Convert.ToDouble(values3BV.Count - solvableUnderLimit) / Convert.ToDouble(boardsSolved);

            System.Diagnostics.Debug.WriteLine("Difficulty: " + nameString);
            System.Diagnostics.Debug.WriteLine("Mean Time to Generate Solvable Board (milliseconds): " + meanTime);
            System.Diagnostics.Debug.WriteLine("Total Boards Generated: " + boardsGenerated);
            System.Diagnostics.Debug.WriteLine("Total Solvable Boards Generated: " + boardsSolved);
            System.Diagnostics.Debug.WriteLine("Fraction of Boards Solvable: " + fractionSolved);
            System.Diagnostics.Debug.WriteLine("3BV Limit: " + betchelsLimit);
            System.Diagnostics.Debug.WriteLine("Number of Solvable Boards Removed due to 3BV Limit: " + solvableUnderLimit);
            System.Diagnostics.Debug.WriteLine("Fraction of Solvable Boards Above or Equal too 3BV Limit: " + fractionLimitValid);
            System.Diagnostics.Debug.WriteLine("---3BV Values of Solvable Boards---");
            System.Diagnostics.Debug.WriteLine("Luckiest Board: " + min);
            System.Diagnostics.Debug.WriteLine("Unluckiest Board: " + max);
            System.Diagnostics.Debug.WriteLine("Mean: " + mean);
            System.Diagnostics.Debug.WriteLine("Median: " + median);
            System.Diagnostics.Debug.WriteLine("Standard Deviation: " + standDev);
            
            // outputs the 3BV limit for the percentages 90%-10%, the 9%-1%, repeating for each magnitude of 10 until 0.01%
            for (int magnitude = 1; magnitude >= -2; magnitude--)
            {
                for (double percent = Math.Pow(10, magnitude) * 9; percent > 0; percent -= Math.Pow(10, magnitude))
                {
                    System.Diagnostics.Debug.WriteLine("Luckiest " + percent + "%: " + FindPercentageCutoff(percent, values3BV));
                }
            }

            double deviations = mean;
            int devCount = 0;
            while (deviations > 0)
            {
                deviations -= standDev;
                System.Diagnostics.Debug.WriteLine(devCount + " Standard Deviations from the Mean: " + deviations);
                devCount++;
            }
        }
        private int FindPercentageCutoff(double percentage, List<int> values)
        {
            int index = Convert.ToInt32(values.Count() * (percentage / 100));
            return values[index];
        }
        private double Find3BVStandardDeviation(List<int> values)
        {
            int sampleSize = values.Count();

            double sum = values.Sum();
            double sumSquares = values.Sum(x => Math.Pow(x, 2));

            double numerator = sumSquares - Math.Pow(sum, 2) / sampleSize;
            double variance = numerator / (sampleSize - 1);

            double standardDeviatoin = Math.Sqrt(variance);
            return standardDeviatoin;
        }
        
        
        // methods that get the data from text file during testing mode
        private List<int> Get3BVValues()
        {
            List<int> values = new List<int>();
            using (StreamReader sr = new StreamReader(Program.Get3BVTextFileName(nameString)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    values.Add(int.Parse(line));
                }
            }
            return values;
        }
        private string GetData(string fileDirectory)
        {
            string line;
            using (StreamReader sr = new StreamReader(fileDirectory))
            {
                line = sr.ReadLine();
            }

            if (string.IsNullOrEmpty(line))
            {
                return "0";
            }
            return line;
        }
    }
}