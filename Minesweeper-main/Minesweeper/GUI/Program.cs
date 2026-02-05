using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper.GUI
{
    internal static class Program
    {
        public const bool Testing = true;
        public const int MaxNameSize = 16;
        public const int TopTimesCount = 10;
        public const string DefaultName = "Guest";
        public static int[] ConstBetchelsMin = new int[] { 0, 0, 0, 0, 0 }; 

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Menu());
        }
    }
}
