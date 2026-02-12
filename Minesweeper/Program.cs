using Minesweeper.AI;
using Minesweeper.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Minesweeper
{
    internal static class Program
    {
        public const bool TestingModeOn = false;
        public const int MaxNameSize = 16;
        public const int TopTimesCount = 10;
        public const int FrontierSizeCutoff = 50;
        public const int HeaderSize = 40;
        public const int MaxCellSize = 40;
        public const int TopFormBarHeight = 21;
        public const string DefaultName = "Guest";
        public static Dictionary<int, int> ConstBetchelsMin = new Dictionary<int, int>
        {
            {0, 5 },
            {1, 17 },
            {2, 42 },
            {3, 136 },
            {4, 263 },
        };
        public static Dictionary<int, string> ConstDifficultyNames = new Dictionary<int, string>
        {
            {0, "Beginner"},
            {1, "Amateur"},
            {2, "Intermediate"},
            {3, "Expert"},
            {4, "Master"},
        };

        // stopwatch is used during testing mode to record how long it takes to generate a solvable grid
        // declared in Program as it needs to be accessed by both Board and Solver classes
        public static Stopwatch stopwatch = new Stopwatch();


        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Minesweeper.GUI.Menu());
        }


        // methods used to get the directories to store the data during testing mode
        // in Program rather than hard coding the names into each class so they can be easily changed if needed
        public static string Get3BVTextFileName(string name)
        {
            return "Difficulty Data/3BV/" + name + "3BV.txt";
        }
        public static string GetSolvableFileName(string name)
        {
            return "Difficulty Data/Solvable Count/" + name + "SolvableCount.txt";
        }
        public static string GetBoardCountFileName(string name)
        {
            return "Difficulty Data/Boards Checked/" + name + "BoardsChecked.txt";
        }
        public static string GetMeanTimeFileName(string name)
        {
            return "Difficulty Data/Mean Time Taken/" + name + "MeanTime.txt";
        }
    }
}
