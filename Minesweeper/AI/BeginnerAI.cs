using System;
using System.IO;

namespace Minesweeper.AI
{
    class BeginnerAI : AI
    {
        public bool PB1(LogicCell currentCell, Grid grid, int x, int y)
        {
            //System.Diagnostics.Debug.WriteLine("CHECKING B1");
            bool returnValue = false;

            if (currentCell.AdjacentHiddenCellsCount == currentCell.Value)
            {
                foreach (LogicCell cell in currentCell.AdjacentCells)
                {
                    if (cell.IsHidden && !cell.IsFlagged)
                    {
                        returnValue = true;
                        cell.Flag();
                    }
                }
            }

            return returnValue;
        }
        public bool PB2(LogicCell currentCell, Grid grid, int x, int y)
        {
            //System.Diagnostics.Debug.WriteLine("CHECKING B2");
            bool returnValue = false;
            if (currentCell.EffectiveValue == 0)
            {
                foreach (LogicCell cell in currentCell.AdjacentCells)
                {
                    if (cell.IsHidden && !cell.IsFlagged)
                    {
                        returnValue = true;
                        cell.Open();
                    }
                }

                if (returnValue)
                {
                    grid.GetCell(x, y).Complete();
                }
            }

            return returnValue;
        }
    }
}