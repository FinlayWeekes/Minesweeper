using System;
using System.IO;

class BeginnerAI : AI
{
    public bool PB1(LogicCell currentCell, Grid grid, int x, int y)
    {
        bool returnValue = false;

        using (StreamWriter sw = new StreamWriter("log.txt", true))
        {
            if (currentCell.AdjacentHiddenCellsCount == currentCell.Value)
            {
                foreach (LogicCell cell in currentCell.AdjacentCells)
                {
                    if (cell.IsHidden && !cell.IsFlagged)
                    {
                        sw.WriteLine("B1 at " + x + ", " + y);
                        returnValue = true;
                        cell.Flag();
                    }
                }
            }
        }

        return returnValue;
    }
    public bool PB2(LogicCell currentCell, Grid grid, int x, int y)
    {
        bool returnValue = false;
        //Console.WriteLine("Checking B2 at {0},{1}", x, y);
        if (currentCell.EffectiveValue == 0)
        {
            using (StreamWriter sw = new StreamWriter("log.txt", true))
            {
                foreach (LogicCell cell in currentCell.AdjacentCells)
                {
                    if (cell.IsHidden && !cell.IsFlagged)
                    {
                        sw.WriteLine("B2 at " + x + ", " + y);
                        returnValue = true;
                        cell.Open();
                    }
                }
            }
        }
        return returnValue;
    }
}