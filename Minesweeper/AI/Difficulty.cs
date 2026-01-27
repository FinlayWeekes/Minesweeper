using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Minesweeper.AI
{
    public class Difficulty
    {
        public List<Func<LogicCell, Grid, int, int, bool>> EasyPatterns
        {
            get
            {
                return easyPatterns;
            }
        }
        private List<Func<LogicCell, Grid, int, int, bool>> easyPatterns;
        public List<Func<LogicCell, Grid, int, int, bool>> OptionalPatterns
        {
            get
            {
                return optionalPatterns;
            }
        }
        private List<Func<LogicCell, Grid, int, int, bool>> optionalPatterns;
        public List<Func<LogicCell, Grid, int, int, bool>> EssentialPatterns
        {
            get
            {
                return essentialPatterns;
            }
        }
        private List<Func<LogicCell, Grid, int, int, bool>> essentialPatterns;
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
        public string Name
        {
            get { return name; }
        }
        private string name;
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
            { "T1R", MasterAI.PT1R },
            { "T2R", MasterAI.PT2R },
            { "T3R", MasterAI.PT3R },
            { "T4R", MasterAI.PT4R },
            { "1-3-VC", MasterAI.P1_3_1VC },
            { "1-3-1VCR", MasterAI.P1_3_1VCR },
            { "2-2-2VC", MasterAI.P2_2_2VC },
            { "2-2-2VCR", MasterAI.P2_2_2VCR },
            { "T-Pattern", MasterAI.PT_Pattern},
            { "MineCounting", MasterAI.PMineCounting }
        };

        public Difficulty(string difficultyName)
        {
            string[] easyNames;
            string[] optionalNames;
            string[] essentialNames;

            using (StreamReader sr = new StreamReader(GetTextFileName(difficultyName)))
            {
                sr.ReadLine();
                this.name = sr.ReadLine();

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

            this.easyPatterns = GetPatterns(easyNames);
            this.optionalPatterns = GetPatterns(optionalNames);
            this.essentialPatterns = GetPatterns(essentialNames);

            CheckForRandom();
        }
        public Difficulty(int width, int height, int mineCount, List<string> easyPatternNames, List<string> optionalPatternNames, List<string> essentialPatternNames)
        {
            this.easyPatterns = GetPatterns(ListToArray(easyPatternNames));
            this.optionalPatterns = GetPatterns(ListToArray(optionalPatternNames));
            this.essentialPatterns = GetPatterns(ListToArray(essentialPatternNames));
            this.width = width;
            this.height = height;
            this.mineCount = mineCount;
            this.name = "Custom";
            this.custom = true;

            CheckForRandom();
        }

        private void CheckForRandom()
        {
            random = easyPatterns.Count == 0 && optionalPatterns.Count == 0 && essentialPatterns.Count == 0; 
        }
        private string GetTextFileName(string difficultyName)
        {
            return "Difficulties/" + difficultyName.ToLower() + "Difficulty.txt";
        }
        private string[] ListToArray(List<string> list)
        {
            string[] array = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                array [i] = list[i];
            }
            return array;
        }
        private List<Func<LogicCell, Grid, int, int, bool>> GetPatterns(string[] patternNames)
        {
            List<Func<LogicCell, Grid, int, int, bool>> patterns = new List<Func<LogicCell, Grid, int, int, bool>>();

            if (patternNames.Length == 0 || patternNames[0] == "null") return patterns;

            foreach (string pattern in patternNames)
            {
                patterns.Add(patternsDict[pattern]);
            }

            return patterns;
        }
    }
}