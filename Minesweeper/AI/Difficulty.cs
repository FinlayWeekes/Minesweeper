using System;
using System.Collections.Generic;

namespace Minesweeper.AI
{
    public class Difficulty
    {
        private List<Func<LogicCell, Grid, int, int, bool>> easyPatterns = new List<Func<LogicCell, Grid, int, int, bool>>();
        public List<Func<LogicCell, Grid, int, int, bool>> EasyPatterns
        {
            get
            {
                return easyPatterns;
            }
        }
        private List<Func<LogicCell, Grid, int, int, bool>> optionalPatterns = new List<Func<LogicCell, Grid, int, int, bool>>();
        public List<Func<LogicCell, Grid, int, int, bool>> OptionalPatterns
        {
            get
            {
                return optionalPatterns;
            }
        }
        private List<Func<LogicCell, Grid, int, int, bool>> essentialPatterns = new List<Func<LogicCell, Grid, int, int, bool>>();
        public List<Func<LogicCell, Grid, int, int, bool>> EssentialPatterns
        {
            get
            {
                return essentialPatterns;
            }
        }
        private int hardestDifficulty;
        public int HardestDifficulty
        {
            get
            {
                return hardestDifficulty;
            }
        }
        private int width;
        public int Width
        {
            get
            {
                return width;
            }
        }
        private int height;
        public int Height
        {
            get
            {
                return height;
            }
        }
        private int mineCount;
        public int MineCount
        {
            get
            {
                return mineCount;
            }
        }
        private string name;
        public string Name
        {
            get { return name; }
        }
        private bool custom;
        public bool Custom
        {
            get
            {
                return custom;
            }
        }

        public Difficulty(int difficulty)
        {
            hardestDifficulty = difficulty;
            custom = false;

            switch (difficulty)
            {
                case 1:
                    {
                        AddBeginnerPatterns();
                        width = 10;
                        height = 10;
                        mineCount = 10;
                        name = "Beginner";
                        break;
                    }
                case 2:
                    {
                        AddAmateurPatterns();
                        width = 12;
                        height = 12;
                        mineCount = 20;
                        name = "Amateur";
                        break;
                    }
                case 3:
                    {
                        AddIntermediatePatterns();
                        width = 16;
                        height = 16;
                        mineCount = 40;
                        break;
                    }
            }
        }
        public Difficulty(int Width, int Height, int MineCount, List<Func<LogicCell, Grid, int, int, bool>> patterns)
        {
            this.essentialPatterns = patterns;
            this.width = Width;
            this.height = Height;
            this.mineCount = MineCount;
            this.name = "Custom";
            this.custom = true;
            this.hardestDifficulty = 1;
        }

        private void AddEasyPatterns()
        {
            BeginnerAI beginnerAI = new BeginnerAI();
            easyPatterns.Add(beginnerAI.PB1);
            easyPatterns.Add(beginnerAI.PB2);
        }
        private void AddBeginnerPatterns()
        {
            AddEasyPatterns();
        }
        private void AddAmateurPatterns()
        {
            AddEasyPatterns();

            AmateurAI amateurAI = new AmateurAI();
            essentialPatterns.Add(amateurAI.P2_1);
            essentialPatterns.Add(amateurAI.P1_1C);
        }
        private void AddIntermediatePatterns()
        {
            AddEasyPatterns();

            // The 2-1C and 2-1CR are not included as they can both be done by a 2-1/2-1R followed by a 1-1R
            // Including them will only make the searching process slower as it will need to check for these more difficult patterns which take longer
            // Not including them doesnt impact the diffuculty either as 1-1R is an essential pattern
            IntermediateAI intermediateAI = new IntermediateAI();
            essentialPatterns.Add(intermediateAI.P1_1CR);
            essentialPatterns.Add(intermediateAI.P2_1R);
            essentialPatterns.Add(intermediateAI.PH1_H3);

            AmateurAI amateurAI = new AmateurAI();
            optionalPatterns.Add(amateurAI.P1_1C);
            optionalPatterns.Add(amateurAI.P2_1);
        }
    }
}