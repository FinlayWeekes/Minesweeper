using System;
using System.Collections.Generic;

namespace Minesweeper.AI
{
    class AmateurAI : AI
    {
        public static bool P2_1(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 2 || currentCell.EffectiveValue != 2) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (!adjacentCell.IsHidden &&
                    adjacentCell.Value == 1 && 
                    adjacentCell.EffectiveValue == 1)
                {
                    List<LogicCell> cellsToFlag = GetNonOverlapingHiddenCells(currentCell, adjacentCell);
                    if (cellsToFlag.Count == 1 && !cellsToFlag[0].IsFlagged)
                    {
                        cellsToFlag[0].Flag();
                        return true;
                    }
                }
            }
            

            return false;
        }
        public static bool P1_1C(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 1 || currentCell.EffectiveValue != 1) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (!adjacentCell.IsHidden &&
                    adjacentCell.Value == 1 && 
                    adjacentCell.EffectiveValue == 1 && 
                    CellsContainEachother(currentCell, adjacentCell))
                {
                    List<LogicCell> cells = GetNonOverlapingHiddenCells(currentCell, adjacentCell);

                    if (cells.Count == 1)
                    {
                        cells[0].Open();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}