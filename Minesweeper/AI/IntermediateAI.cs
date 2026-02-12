using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.Linq;
using System.Windows.Forms;

namespace Minesweeper.AI
{
    class IntermediateAI : AI
    {
        public static bool P1_1CR(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 1) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (Pr1_1CR(adjacentCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr1_1CR(LogicCell adjacentCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            if (adjacentCell.IsHidden ||
                adjacentCell.EffectiveValue != 1 ||
                (currentCell.Value == 1 && adjacentCell.Value == 1) ||
                !NonFlagCellsContainEachother(currentCell, adjacentCell))
            {
                return false;
            }

            HashSet<LogicCell> cells = GetNonOverlapingNonFlaggedHiddenCells(currentCell, adjacentCell);

            if (cells.Count != 1) return false;

            cells.Single().Open();
            return true;
        }

        public static bool P2_1R(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 2) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (Pr2_1R(adjacentCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr2_1R(LogicCell adjacentCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            if (adjacentCell.IsHidden ||
                adjacentCell.EffectiveValue != 1 ||
                (currentCell.Value == 2 && adjacentCell.Value == 1))
            {
                return false;
            }

            HashSet<LogicCell> cells = GetNonOverlapingNonFlaggedHiddenCells(currentCell, adjacentCell);

            if (cells.Count() != 1) return false;

            cells.Single().Flag();
            return true;
        }

        public static bool P2_1C(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 2 || currentCell.EffectiveValue != 2) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (Pr2_1C(adjacentCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr2_1C(LogicCell adjacentCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            if (adjacentCell.IsHidden ||
                adjacentCell.Value != 1 ||
                adjacentCell.EffectiveValue != 1)
            {
                return false;
            }

            HashSet<LogicCell> cells = GetNonOverlapingHiddenCells(currentCell, adjacentCell);

            if (cells.Count != 1) return false;

            HashSet<LogicCell> cellToOpen = GetNonOverlapingHiddenCells(adjacentCell, currentCell);

            if (cellToOpen.Count != 1) return false;

            cellToOpen.Single().Open();
            return true;
        }

        public static bool P2_1CR(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 2) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (Pr2_1CR(adjacentCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr2_1CR(LogicCell adjacentCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            if (adjacentCell.IsHidden ||
                adjacentCell.EffectiveValue != 1 ||
                (currentCell.Value == 2 && adjacentCell.Value == 1))
            {
                return false;
            }

            HashSet<LogicCell> cellsToFlag = GetNonOverlapingNonFlaggedHiddenCells(currentCell, adjacentCell);

            if (cellsToFlag.Count != 1) return false;

            HashSet<LogicCell> cellToOpen = GetNonOverlapingHiddenCells(adjacentCell, currentCell);

            if (cellToOpen.Count != 1 || cellToOpen.Single().IsFlagged) return false;

            cellToOpen.Single().Open();
            return true;
        }

        public static bool PH1(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 1 || currentCell.AdjacentHiddenCellsCount != 2) return false;
            //System.Diagnostics.Debug.WriteLine("checking H1 at " + x + "," + y);

            HashSet<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                if (Pr_H1(perpCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr_H1((int x, int y) perpCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            if (grid.GetCellTup(perpCell).IsHidden ||
                grid.GetCellTup(perpCell).Value != 1 ||
                grid.GetCellTup(perpCell).EffectiveValue != 1 ||
                grid.GetCellTup(perpCell).AdjacentHiddenCellsCount <= 2)
            {
                return false;
            }

            HashSet<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
            bool wallCellsHidden = wallCells.Count == 2;
            foreach ((int x, int y) wallCell in wallCells)
            {
                wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden;
            }

            if (!wallCellsHidden) return false;

            HashSet<(int x, int y)> holeCells = GetHoleCells(perpCell, x, y, grid, wallCells);

            bool hiddenCellExists = false;
            foreach ((int x, int y) cellToOpen in holeCells)
            {
                hiddenCellExists = grid.GetCellTup(cellToOpen).IsHidden || hiddenCellExists;
            }

            if (!hiddenCellExists) return false;

            foreach ((int x, int y) cellToOpen in holeCells)
            {
                if (grid.GetCellTup(cellToOpen).IsHidden) grid.GetCellTup(cellToOpen).Open();
            }

            return true;
        }

        public static bool PH2(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 1 || currentCell.AdjacentHiddenCellsCount != 2) return false;
            //System.Diagnostics.Debug.WriteLine("checking H2 at " + x + "," + y);

            HashSet<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                if (Pr_H2(perpCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr_H2((int x, int y) perpCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            // the bit in bounds is one more in the direction than H1 as it extends the hole
            if (!grid.IsInBounds(x + xDirection * 3, y + yDirection * 3) ||
                grid.GetCellTup(perpCell).IsHidden ||
                grid.GetCellTup(perpCell).Value != 1 ||
                grid.GetCellTup(perpCell).EffectiveValue != 1 ||
                grid.GetCellTup(perpCell).AdjacentHiddenCellsCount != 2)
            {
                return false;
            }

            HashSet<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
            bool wallCellsHidden = wallCells.Count == 2;
            foreach ((int x, int y) wallCell in wallCells)
            {
                wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden;
            }

            if (!wallCellsHidden) return false;

            // H1 is guaranteed to have already been done at this point as the perp cell is always adjacent to two hidden cells (the 2 wall cells)
            (int x, int y) holeCell = (perpCell.x + xDirection, perpCell.y + yDirection);

            if (grid.GetCellTup(holeCell).Value != 1 || grid.GetCellTup(holeCell).EffectiveValue != 1) return false;

            // the end2Cells are not hidden, they are the cells next to the middle cell in the hole and represent where the walls would be so the GetHoleCells method works
            HashSet<(int x, int y)> end2Cells = GetWallCells(holeCell, perpCell.x, perpCell.y, grid);
            HashSet<(int x, int y)> hole2Cells = GetHoleCells(holeCell, perpCell.x, perpCell.y, grid, end2Cells);

            bool hiddenCellExists = false;
            foreach ((int x, int y) cellToOpen in hole2Cells)
            {
                hiddenCellExists = grid.GetCellTup(cellToOpen).IsHidden || hiddenCellExists;
            }

            if (!hiddenCellExists) return false;

            foreach ((int x, int y) cellToOpen in hole2Cells)
            {
                if (grid.GetCellTup(cellToOpen).IsHidden)
                {
                    grid.GetCellTup(cellToOpen).Open();
                }
            }
            //System.Diagnostics.Debug.WriteLine("found H2 at " + x + "," + y);
            return true;
        }

        public static bool PH3(LogicCell currentCell, Grid grid, int x, int y)
        {
            HashSet<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                if (Pr_H3(perpCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr_H3((int x, int y) perpCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            if (!grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) ||
                grid.GetCellTup(perpCell).IsHidden)
            {
                return false;
            }

            (int x, int y) holeCell1 = (perpCell.x + xDirection, perpCell.y + yDirection);

            if (grid.GetCellTup(holeCell1).IsHidden) return false;

            (int x, int y)[] insideWallCells = new (int x, int y)[] {
        (holeCell1.x + yDirection, holeCell1.y + xDirection),
        (holeCell1.x - yDirection, holeCell1.y - xDirection)};

            (int x, int y)[] outsideWallCells = new (int x, int y)[] {
        (x + yDirection, y + xDirection),
        (x - yDirection, y - xDirection)};

            bool returnValue = false;
            for (int i = 0; i < insideWallCells.Length; i++)
            {
                if (grid.IsInBounds(insideWallCells[i].x, insideWallCells[i].y) &&
                    grid.IsInBounds(outsideWallCells[i].x, outsideWallCells[i].y) &&
                    !grid.GetCellTup(insideWallCells[i]).IsHidden &&
                    !grid.GetCellTup(outsideWallCells[i]).IsHidden &&
                    grid.GetCellTup(insideWallCells[i]).Value == 1 &&
                    grid.GetCellTup(outsideWallCells[i]).Value == 1 &&
                    grid.GetCellTup(insideWallCells[i]).EffectiveValue == 1 &&
                    grid.GetCellTup(outsideWallCells[i]).EffectiveValue == 1 &&
                    CellsContainEachother(grid.GetCellTup(insideWallCells[i]), grid.GetCellTup(outsideWallCells[i])))
                {
                    HashSet<LogicCell> cellsToOpen = GetNonOverlapingHiddenCells(grid.GetCellTup(insideWallCells[i]), grid.GetCellTup(outsideWallCells[i]));

                    returnValue = cellsToOpen.Count > 0 || returnValue;

                    foreach (LogicCell cell in cellsToOpen)
                    {
                        if (cell.IsHidden) cell.Open();
                    }
                }
            }

            return returnValue;
        }
    }
}
