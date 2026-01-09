using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
            { "H1-H3", IntermediateAI.PH1_H3 },
            { "H1", IntermediateAI.PH1 },
            { "H2", IntermediateAI.PH2 },
            { "H3", IntermediateAI.PH3 },
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
        }

        private string GetTextFileName(string difficultyName)
        {
            return "Difficulties/" + difficultyName.ToLower() + "Difficulty.txt";
        }
        private List<Func<LogicCell, Grid, int, int, bool>> GetPatterns(string[] patternNames)
        {
            List<Func<LogicCell, Grid, int, int, bool>> patterns = new List<Func<LogicCell, Grid, int, int, bool>>();

            if (patternNames[0] == "null") return patterns;

            foreach (string pattern in patternNames)
            {
                patterns.Add(patternsDict[pattern]);
            }

            return patterns;
        }
    }
}