using System;
using System.IO;

namespace Minesweeper.AI
{
    class BeginnerAI : AI
    {
        public bool PB1(LogicCell currentCell, Grid grid, int x, int y)
        {
            //System.Diagnostics.Debug.WriteLine("CHECKING B1");

            if (currentCell.EffectiveValue == 0 || currentCell.AdjacentHiddenCellsCount != currentCell.Value) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (adjacentCell.IsHidden && !adjacentCell.IsFlagged)
                {
                    adjacentCell.Flag();
                }
            }
            
            return true;
        }
        public bool PB2(LogicCell currentCell, Grid grid, int x, int y)
        {
            //System.Diagnostics.Debug.WriteLine("CHECKING B2");
            if (currentCell.EffectiveValue != 0) return false;
            if (currentCell.AdjacentHiddenCellsCount == currentCell.Value)
            {
                currentCell.Complete();
                return false;
            }

            foreach (LogicCell cell in currentCell.AdjacentCells)
            {
                if (cell.IsHidden && !cell.IsFlagged)
                {
                    cell.Open();
                }
            }

            currentCell.Complete();

            return true;
        }
    }
}