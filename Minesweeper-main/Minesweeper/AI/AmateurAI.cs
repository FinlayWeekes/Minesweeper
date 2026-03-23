using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.AI
{
    class AmateurAI : AI
    {
        public static bool P2_1(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 2 || currentCell.EffectiveValue != 2) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (Pr2_1(adjacentCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr2_1(LogicCell adjacentCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            if (adjacentCell.IsHidden ||
                adjacentCell.Value != 1 ||
                adjacentCell.EffectiveValue != 1)
            {
                return false;
            }

            HashSet<LogicCell> cellsToFlag = GetNonOverlapingHiddenCells(currentCell, adjacentCell);

            // .Single gets the value in the hash set if there is only one value in it
            if (cellsToFlag.Count != 1 || cellsToFlag.Single().IsFlagged) return false;

            cellsToFlag.Single().Flag();
            return true;
        }

        public static bool P1_1C(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 1 || currentCell.EffectiveValue != 1) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (Pr1_1C(adjacentCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr1_1C(LogicCell adjacentCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            if (adjacentCell.IsHidden ||
                adjacentCell.Value != 1 ||
                adjacentCell.EffectiveValue != 1 ||
                !CellsContainEachother(currentCell, adjacentCell))
            {
                return false;
            }

            HashSet<LogicCell> cellsToOpen = GetNonOverlapingHiddenCells(currentCell, adjacentCell);

            if (cellsToOpen.Count != 1) return false;

            cellsToOpen.Single().Open();
            return true;
        }
    }
}