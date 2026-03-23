using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

namespace Minesweeper.AI
{
    class MasterAI : AI
    {
        public static bool P1_1TR(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 1 || currentCell.AdjacentHiddenNonFlagCellsCount != 3) return false;

            HashSet<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                if (Pr1_1TR(perpCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr1_1TR((int x, int y) perpCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            if (!grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) ||
                grid.GetCellTup(perpCell).IsHidden ||
                grid.GetCellTup(perpCell).EffectiveValue != 1 ||
                grid.GetCellTup(perpCell).AdjacentHiddenNonFlagCellsCount < 4 ||
                !NonFlagCellsContainEachother(grid.GetCellTup(perpCell), currentCell) ||
                (currentCell.EffectiveValue == currentCell.Value && grid.GetCellTup(perpCell).EffectiveValue == grid.GetCellTup(perpCell).Value))
            {
                return false;
            } 

            HashSet<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
            bool wallCellsHidden = wallCells.Count == 2;
            foreach ((int x, int y) wallCell in wallCells)
            {
                wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden;
            }

            // the extra cell differenciating this from the Hs is already checked by making sure the current cell is adjacent to 3 hidden cells
            // and that it is contained in the perp cell
            if (!wallCellsHidden) return false;

            // no need to check if theer is a hidden non-flagged cell in the hole as this has already been checked by the previous conditions about hidden non flag adjacent cells
            HashSet<LogicCell> holeCells = GetNonOverlapingNonFlaggedHiddenCells(grid.GetCellTup(perpCell), currentCell);

            foreach (LogicCell cellToOpen in holeCells)
            {
                if (cellToOpen.IsHidden && !cellToOpen.IsFlagged) cellToOpen.Open();
            }

            return true;
        }
        
        public static bool P2_2TR(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 2 || currentCell.AdjacentHiddenNonFlagCellsCount != 3) return false;

            HashSet<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                if (Pr2_2TR(perpCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr2_2TR((int x, int y) perpCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            if (!grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) ||
                grid.GetCellTup(perpCell).IsHidden ||
                grid.GetCellTup(perpCell).EffectiveValue != 2 ||
                grid.GetCellTup(perpCell).AdjacentHiddenNonFlagCellsCount < 4 ||
                !NonFlagCellsContainEachother(grid.GetCellTup(perpCell), currentCell) ||
                (currentCell.EffectiveValue == currentCell.Value && grid.GetCellTup(perpCell).EffectiveValue == grid.GetCellTup(perpCell).Value))
            {
                return false;
            }

            HashSet<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
            bool wallCellsHidden = wallCells.Count == 2;
            foreach ((int x, int y) wallCell in wallCells)
            {
                wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden;
            }

            // the extra cell differenciating this from the Hs is already checked by making sure the current cell is adjacent to 3 hidden cells
            // and that it is contained in the perp cell
            if (!wallCellsHidden) return false;

            // no need to check if theer is a hidden non-flagged cell in the hole as this has already been checked by the previous conditions about hidden non flag adjacent cells
            HashSet<LogicCell> holeCells = GetNonOverlapingNonFlaggedHiddenCells(grid.GetCellTup(perpCell), currentCell);

            foreach (LogicCell cellToOpen in holeCells)
            {
                if (cellToOpen.IsHidden && !cellToOpen.IsFlagged) cellToOpen.Open();
            }

            return true;
        }
        
        public static bool P3_2TR(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 3 || currentCell.AdjacentHiddenNonFlagCellsCount != 4) return false;

            HashSet<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                if (Pr3_2TR(perpCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr3_2TR((int x, int y) perpCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            if (!grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) ||
                grid.GetCellTup(perpCell).IsHidden ||
                grid.GetCellTup(perpCell).EffectiveValue != 2 ||
                grid.GetCellTup(perpCell).AdjacentHiddenNonFlagCellsCount < 4 ||
                (currentCell.EffectiveValue == currentCell.Value && grid.GetCellTup(perpCell).EffectiveValue == grid.GetCellTup(perpCell).Value))
            {
                return false;
            }

            HashSet<LogicCell> cellToFlag = GetNonOverlapingNonFlaggedHiddenCells(currentCell, grid.GetCellTup(perpCell));

            if (cellToFlag.Count != 1) return false;

            cellToFlag.Single().Flag();

            HashSet<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
            bool wallCellsHidden = wallCells.Count == 2;
            foreach ((int x, int y) wallCell in wallCells)
            {
                wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden;
            }

            // the extra cell differenciating this from the Hs is already checked by making sure the current cell is adjacent to 3 hidden cells
            // and that it is contained in the perp cell
            if (wallCellsHidden)
            {
                // no need to check if there is a hidden non-flagged cell in the hole as this has already been checked by the previous conditions about hidden non flag adjacent cells
                HashSet<LogicCell> holeCells = GetNonOverlapingNonFlaggedHiddenCells(grid.GetCellTup(perpCell), currentCell);

                foreach (LogicCell cellToOpen in holeCells)
                {
                    if (cellToOpen.IsHidden) cellToOpen.Open();
                }
            }

            return true;
        }
        
        public static bool P1_3_1VC(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 3 ||
                currentCell.EffectiveValue != 3 ||
                currentCell.AdjacentHiddenCellsCount != 5 ||
                grid.IsOnEdge(x, y))
            {
                return false;
            }

            HashSet<(int x, int y)> diagCells = GetDiagonalCellIndexes(grid, x, y);

            foreach ((int x, int y) cornerCell in diagCells)
            {
                int xDirection = cornerCell.x - x;
                int yDirection = cornerCell.y - y;

                if (Pr1_3_1VC(xDirection, yDirection, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr1_3_1VC(int xDirection, int yDirection, Grid grid, int x, int y)
        {
            // checks every corner cell is hidden
            HashSet<LogicCell> cornerCells = GetCornerCells(grid, xDirection, yDirection, x, y);
            foreach (LogicCell cornerCell in cornerCells)
            {
                if (!cornerCell.IsHidden) return false;
            }

            // checks the perp 1s
            HashSet<LogicCell> edgeCells = GetEdgeCells(grid, xDirection, yDirection, x, y);
            foreach (LogicCell edgeCell in edgeCells)
            {
                if (edgeCell.Value != 1 ||
                    edgeCell.EffectiveValue != 1)
                {
                    return false;
                }
            }

            // flags corner cell
            grid.GetCell(x + xDirection, y + yDirection).Flag();

            // opens the cells adjacent to 1s if possible
            foreach (LogicCell edgeCell in edgeCells)
            {
                HashSet<LogicCell> cellsToOpen = GetNonOverlapingHiddenCells(edgeCell, grid.GetCell(x, y));

                if (cellsToOpen.Count == 1)
                {
                    cellsToOpen.Single().Open();
                }
            }

            return true;
        }
        
        public static bool P1_3_1VCR(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 3 ||
                currentCell.AdjacentHiddenNonFlagCellsCount != 5 ||
                grid.IsOnEdge(x, y))
            {
                return false;
            }

            HashSet<(int x, int y)> diagCells = GetDiagonalCellIndexes(grid, x, y);

            foreach ((int x, int y) cornerCell in diagCells)
            {
                int xDirection = cornerCell.x - x;
                int yDirection = cornerCell.y - y;

                if (Pr1_3_1VCR(xDirection, yDirection, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr1_3_1VCR(int xDirection, int yDirection, Grid grid, int x, int y)
        {
            // checks every corner cell is hidden
            HashSet<LogicCell> cornerCells = GetCornerCells(grid, xDirection, yDirection, x, y);
            foreach (LogicCell cornerCell in cornerCells)
            {
                if (!cornerCell.IsHidden) return false;
            }

            // checks the perp 1s
            bool reductionDone = false;
            HashSet<LogicCell> edgeCells = GetEdgeCells(grid, xDirection, yDirection, x, y);
            foreach (LogicCell edgeCell in edgeCells)
            {
                if (edgeCell.IsHidden ||
                    edgeCell.EffectiveValue != 1)
                {
                    return false;
                }

                reductionDone = reductionDone || edgeCell.Value != edgeCell.EffectiveValue;
            }

            if (!reductionDone && grid.GetCell(x, y).Value == grid.GetCell(x, y).EffectiveValue) return false;

            // flags corner cell
            bool returnValue = false;
            if (!grid.GetCell(x + xDirection, y + yDirection).IsFlagged)
            {
                grid.GetCell(x + xDirection, y + yDirection).Flag();
                returnValue = true;
            }

            // opens the cells adjacent to 1s if possible
            foreach (LogicCell edgeCell in edgeCells)
            {
                HashSet<LogicCell> cellsToOpen = GetNonOverlapingNonFlaggedHiddenCells(edgeCell, grid.GetCell(x, y));

                if (cellsToOpen.Count == 1)
                {
                    cellsToOpen.Single().Open();
                    returnValue = true;
                }
            }

            return returnValue;
        }
       
        public static bool P2_2_2VC(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 2 ||
                currentCell.EffectiveValue != 2 ||
                currentCell.AdjacentHiddenCellsCount != 5 ||
                grid.IsOnEdge(x, y))
            {
                return false;
            }

            HashSet<(int x, int y)> diagCells = GetDiagonalCellIndexes(grid, x, y);

            foreach ((int x, int y) cornerCell in diagCells)
            {
                int xDirection = cornerCell.x - x;
                int yDirection = cornerCell.y - y;

                if (Pr2_2_2VC(xDirection, yDirection, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr2_2_2VC(int xDirection, int yDirection, Grid grid, int x, int y)
        {
            // checks every corner cell is hidden
            HashSet<LogicCell> cornerCells = GetCornerCells(grid, xDirection, yDirection, x, y);
            foreach (LogicCell cornerCell in cornerCells)
            {
                if (!cornerCell.IsHidden) return false;
            }

            // checks the perp 2s
            HashSet<LogicCell> edgeCells = GetEdgeCells(grid, xDirection, yDirection, x, y);
            foreach (LogicCell edgeCell in edgeCells)
            {
                if (edgeCell.Value != 2 ||
                    edgeCell.EffectiveValue != 2 ||
                    edgeCell.AdjacentHiddenCellsCount != 3)
                {
                    return false;
                }
            }

            // opens corner cell
            grid.GetCell(x + xDirection, y + yDirection).Open();

            // flags the cells adjacent to 2s if possible
            foreach (LogicCell edgeCell in edgeCells)
            {
                HashSet<LogicCell> cellsToOpen = GetNonOverlapingHiddenCells(edgeCell, grid.GetCell(x, y));

                if (cellsToOpen.Count == 1)
                {
                    cellsToOpen.Single().Flag();
                }
            }

            return true;
        }
        
        public static bool P2_2_2VCR(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 2 ||
                currentCell.AdjacentHiddenNonFlagCellsCount != 5 ||
                grid.IsOnEdge(x, y))
            {
                return false;
            }

            HashSet<(int x, int y)> diagCells = GetDiagonalCellIndexes(grid, x, y);

            foreach ((int x, int y) cornerCell in diagCells)
            {
                int xDirection = cornerCell.x - x;
                int yDirection = cornerCell.y - y;

                if (Pr2_2_2VCR(xDirection, yDirection, grid, x, y)) return true;
            }

            return false;
        }
        private static bool Pr2_2_2VCR(int xDirection, int yDirection, Grid grid, int x, int y)
        {
            // checks every corner cell is hidden
            HashSet<LogicCell> cornerCells = GetCornerCells(grid, xDirection, yDirection, x, y);
            foreach (LogicCell cornerCell in cornerCells)
            {
                if (!cornerCell.IsHidden) return false;
            }

            // checks the perp 2s
            bool reductionDone = false;
            HashSet<LogicCell> edgeCells = GetEdgeCells(grid, xDirection, yDirection, x, y);
            foreach (LogicCell edgeCell in edgeCells)
            {
                if (edgeCell.IsHidden ||
                    edgeCell.EffectiveValue != 2 ||
                    edgeCell.AdjacentHiddenNonFlagCellsCount != 3)
                {
                    return false;
                }

                reductionDone = reductionDone || edgeCell.Value != edgeCell.EffectiveValue;
            }

            if (!reductionDone && grid.GetCell(x, y).Value == grid.GetCell(x, y).EffectiveValue) return false;

            //System.Diagnostics.Debug.WriteLine("Found at " + x + "," + y);

            // opens corner cell
            bool returnValue = false;
            if (!grid.GetCell(x + xDirection, y + yDirection).IsFlagged)
            {
                grid.GetCell(x + xDirection, y + yDirection).Open();
                returnValue = true;
            }

            // flags the cells adjacent to 2s if possible
            foreach (LogicCell edgeCell in edgeCells)
            {
                HashSet<LogicCell> cellsToOpen = GetNonOverlapingNonFlaggedHiddenCells(edgeCell, grid.GetCell(x, y));

                if (cellsToOpen.Count == 1)
                {
                    cellsToOpen.Single().Flag();
                    returnValue = true;
                }
            }

            return returnValue;
        }
        
        public static bool PT_Pattern(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 1 || currentCell.AdjacentHiddenCellsCount != 2) return false;

            HashSet<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                if (PrT_Pattern(perpCell, currentCell, grid, x, y)) return true;
            }

            return false;
        }
        private static bool PrT_Pattern((int x, int y) perpCell, LogicCell currentCell, Grid grid, int x, int y)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            int xParallelDirection = yDirection;
            int yParallelDirection = xDirection;

            // the bit in bounds is one more in the direction than H1 as it extends the hole
            if (!grid.IsInBounds(x + xDirection * 3, y + yDirection * 3) ||
                !grid.IsInBounds(x + xParallelDirection * 2, y + yParallelDirection * 2) ||
                !grid.IsInBounds(x - xParallelDirection * 2, y - yParallelDirection * 2) ||
                grid.GetCellTup(perpCell).IsHidden ||
                grid.GetCellTup(perpCell).Value != 1 ||
                grid.GetCellTup(perpCell).EffectiveValue != 1 ||
                grid.GetCellTup(perpCell).AdjacentHiddenCellsCount != 2)
            {
                return false;
            }

            // uses extended wall cells to get the 4 wall cells instead of the normal 2 for hole patterns
            HashSet<(int x, int y)> wallCells = GetExtendedWallCells(perpCell, x, y, grid);
            bool wallCellsValid = wallCells.Count == 4;
            foreach ((int x, int y) wallCell in wallCells)
            {
                if (!grid.GetCellTup(wallCell).IsHidden ||
                    grid.GetCellTup(wallCell).IsFlagged)
                {
                    wallCellsValid = false;
                }
            }

            // only moves on if the 4 wall cells are hidden
            if (!wallCellsValid) return false;

            HashSet<(int x, int y)> outsideWallCells = GetWallCells((x, y), x - xDirection, y - yDirection, grid);
            bool outsideWallCellsValid = outsideWallCells.Count == 2;
            foreach ((int x, int y) cell in outsideWallCells)
            {
                if (grid.GetCellTup(cell).IsHidden ||
                    grid.GetCellTup(cell).Value != 1 ||
                    grid.GetCellTup(cell).EffectiveValue != 1 ||
                    grid.GetCellTup(cell).AdjacentHiddenCellsCount != 2)
                {
                    outsideWallCellsValid = false;
                }
            }

            // only moves on if the two outside wall cells are 1s with effective value 1 and are not adjacent to any other hidden cells except the walls
            if (!outsideWallCellsValid) return false;

            // H1 is already done at this point
            (int x, int y) holeCell = (perpCell.x + xDirection, perpCell.y + yDirection);

            // checks the middle cell is a valid 3
            if (grid.GetCellTup(holeCell).Value != 3 || grid.GetCellTup(holeCell).EffectiveValue != 3) return false;

            (int x, int y) threeHoleCell = (perpCell.x + xDirection, perpCell.y + yDirection);
            (int xDirection, int yDirection) twoHoleCell = (0, 0);
            (int xDirection, int yDirection) fourHoleCell = (0, 0);
            HashSet<(int x, int y)> endHoleCells = GetWallCells(holeCell, perpCell.x, perpCell.y, grid);
            bool foundFour = false;
            bool foundTwo = false;
            
            // find the 4 and 2 required for T-Pattern
            // loop is required because the pattern can be mirrored (2-3-4) or (4-3-2)
            foreach ((int x, int y) endHoleCell in endHoleCells)
            {
                if (grid.GetCellTup(endHoleCell).Value == 4 &&
                    grid.GetCellTup(endHoleCell).EffectiveValue == 4)
                {
                    fourHoleCell.xDirection = endHoleCell.x - threeHoleCell.x;
                    fourHoleCell.yDirection = endHoleCell.y - threeHoleCell.y;
                    foundFour = true;
                }
                else if (grid.GetCellTup(endHoleCell).Value == 2 &&
                    grid.GetCellTup(endHoleCell).EffectiveValue == 2)
                {
                    twoHoleCell.xDirection = endHoleCell.x - threeHoleCell.x;
                    twoHoleCell.yDirection = endHoleCell.y - threeHoleCell.y;
                    foundTwo = true;
                }
            }

            if (!foundFour || !foundTwo) return false;

            bool returnValue = false;
            HashSet<LogicCell> cellsToOpen = new HashSet<LogicCell>();
            cellsToOpen.Add(grid.GetCell(threeHoleCell.x + twoHoleCell.xDirection * 2, threeHoleCell.y + twoHoleCell.yDirection * 2));
            cellsToOpen.Add(grid.GetCell(threeHoleCell.x + twoHoleCell.xDirection * 2 + xDirection, threeHoleCell.y + twoHoleCell.yDirection * 2 + yDirection));
            foreach (LogicCell cell in cellsToOpen)
            {
                if (cell.IsHidden)
                {
                    cell.Open();
                    returnValue = true;
                }
            }

            LogicCell cellToFlag = grid.GetCell(threeHoleCell.x + fourHoleCell.xDirection + xDirection, threeHoleCell.y + fourHoleCell.yDirection + yDirection);
            if (cellToFlag.IsHidden)
            {
                cellToFlag.Flag();
                returnValue = true;
            }

            return returnValue;
        }
    }
}
