using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

namespace Minesweeper.AI
{
    class MasterAI : AI
    {
        public static bool P1_1TR(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 1 || currentCell.AdjacentHiddenNonFlagCellsCount != 3) return false;

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                if (grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) &&
                    !grid.GetCellTup(perpCell).IsHidden &&
                    grid.GetCellTup(perpCell).EffectiveValue == 1 &&
                    grid.GetCellTup(perpCell).AdjacentHiddenNonFlagCellsCount >= 4 &&
                    NonFlagCellsContainEachother(grid.GetCellTup(perpCell), currentCell) &&
                    (currentCell.EffectiveValue != currentCell.Value || grid.GetCellTup(perpCell).EffectiveValue != grid.GetCellTup(perpCell).Value))
                {
                    List<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
                    bool wallCellsHidden = wallCells.Count == 2;
                    foreach ((int x, int y) wallCell in wallCells)
                    {
                        wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden;
                    }

                    // the extra cell differenciating this from the Hs is already checked by making sure the current cell is adjacent to 3 hidden cells
                    // and that it is contained in the perp cell
                    if (wallCellsHidden)
                    {
                        // no need to check if theer is a hidden non-flagged cell in the hole as this has already been checked by the previous conditions about hidden non flag adjacent cells
                        List<LogicCell> holeCells = GetNonOverlapingNonFlaggedHiddenCells(grid.GetCellTup(perpCell), currentCell);

                        foreach (LogicCell cellToOpen in holeCells)
                        {
                            if (cellToOpen.IsHidden && !cellToOpen.IsFlagged) cellToOpen.Open();
                        }

                        return true;
                    }
                }
            }

            return false;
        }
        public static bool P2_2TR(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 2 || currentCell.AdjacentHiddenNonFlagCellsCount != 3) return false;

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                if (grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) &&
                    !grid.GetCellTup(perpCell).IsHidden &&
                    grid.GetCellTup(perpCell).EffectiveValue == 2 &&
                    grid.GetCellTup(perpCell).AdjacentHiddenNonFlagCellsCount >= 4 &&
                    NonFlagCellsContainEachother(grid.GetCellTup(perpCell), currentCell) &&
                    (currentCell.EffectiveValue != currentCell.Value || grid.GetCellTup(perpCell).EffectiveValue != grid.GetCellTup(perpCell).Value))
                {
                    List<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
                    bool wallCellsHidden = wallCells.Count == 2;
                    foreach ((int x, int y) wallCell in wallCells)
                    {
                        wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden;
                    }

                    // the extra cell differenciating this from the Hs is already checked by making sure the current cell is adjacent to 3 hidden cells
                    // and that it is contained in the perp cell
                    if (wallCellsHidden)
                    {
                        // no need to check if theer is a hidden non-flagged cell in the hole as this has already been checked by the previous conditions about hidden non flag adjacent cells
                        List<LogicCell> holeCells = GetNonOverlapingNonFlaggedHiddenCells(grid.GetCellTup(perpCell), currentCell);

                        foreach (LogicCell cellToOpen in holeCells)
                        {
                            if (cellToOpen.IsHidden && !cellToOpen.IsFlagged) cellToOpen.Open();
                        }

                        return true;
                    }
                }
            }

            return false;
        }
        public static bool P3_2TR(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 3 || currentCell.AdjacentHiddenNonFlagCellsCount != 4) return false;

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                if (grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) &&
                    !grid.GetCellTup(perpCell).IsHidden &&
                    grid.GetCellTup(perpCell).EffectiveValue == 2 &&
                    grid.GetCellTup(perpCell).AdjacentHiddenNonFlagCellsCount >= 4 &&
                    (currentCell.EffectiveValue != currentCell.Value || grid.GetCellTup(perpCell).EffectiveValue != grid.GetCellTup(perpCell).Value))
                {
                    List<LogicCell> cellToFlag = GetNonOverlapingNonFlaggedHiddenCells(currentCell, grid.GetCellTup(perpCell));

                    if (cellToFlag.Count == 1)
                    {
                        cellToFlag[0].Flag();

                        List<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
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
                            List<LogicCell> holeCells = GetNonOverlapingNonFlaggedHiddenCells(grid.GetCellTup(perpCell), currentCell);

                            foreach (LogicCell cellToOpen in holeCells)
                            {
                                if (cellToOpen.IsHidden) cellToOpen.Open();
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
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

            List<(int x, int y)> diagCells = GetDiagonalCellIndexes(grid, x, y);

            foreach ((int x, int y) cornerCell in diagCells)
            {
                int xDirection = cornerCell.x - x;
                int yDirection = cornerCell.y - y;

                if (H1_3_1VC(xDirection, yDirection, grid, x, y)) return true;
            }

            return false;
        }
        private static bool H1_3_1VC(int xDirection, int yDirection, Grid grid, int x, int y)
        {
            // checks every corner cell is hidden
            List<LogicCell> cornerCells = GetCornerCellIndexes(grid, xDirection, yDirection, x, y);
            foreach (LogicCell cornerCell in cornerCells)
            {
                if (!cornerCell.IsHidden) return false;
            }

            // checks the perp 1s
            List<LogicCell> edgeCells = GetEdgeCellIndexes(grid, xDirection, yDirection, x, y);
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
                List<LogicCell> cellsToOpen = GetNonOverlapingHiddenCells(edgeCell, grid.GetCell(x, y));

                if (cellsToOpen.Count == 1)
                {
                    cellsToOpen[0].Open();
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

            List<(int x, int y)> diagCells = GetDiagonalCellIndexes(grid, x, y);

            foreach ((int x, int y) cornerCell in diagCells)
            {
                int xDirection = cornerCell.x - x;
                int yDirection = cornerCell.y - y;

                if (H1_3_1VCR(xDirection, yDirection, grid, x, y)) return true;
            }

            return false;
        }
        private static bool H1_3_1VCR(int xDirection, int yDirection, Grid grid, int x, int y)
        {
            // checks every corner cell is hidden
            List<LogicCell> cornerCells = GetCornerCellIndexes(grid, xDirection, yDirection, x, y);
            foreach (LogicCell cornerCell in cornerCells)
            {
                if (!cornerCell.IsHidden) return false;
            }

            // checks the perp 1s
            bool reductionDone = false;
            List<LogicCell> edgeCells = GetEdgeCellIndexes(grid, xDirection, yDirection, x, y);
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
                List<LogicCell> cellsToOpen = GetNonOverlapingNonFlaggedHiddenCells(edgeCell, grid.GetCell(x, y));

                if (cellsToOpen.Count == 1)
                {
                    cellsToOpen[0].Open();
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

            List<(int x, int y)> diagCells = GetDiagonalCellIndexes(grid, x, y);

            foreach ((int x, int y) cornerCell in diagCells)
            {
                int xDirection = cornerCell.x - x;
                int yDirection = cornerCell.y - y;

                if (H2_2_2VC(xDirection, yDirection, grid, x, y)) return true;
            }

            return false;
        }
        private static bool H2_2_2VC(int xDirection, int yDirection, Grid grid, int x, int y)
        {
            // checks every corner cell is hidden
            List<LogicCell> cornerCells = GetCornerCellIndexes(grid, xDirection, yDirection, x, y);
            foreach (LogicCell cornerCell in cornerCells)
            {
                if (!cornerCell.IsHidden) return false;
            }

            // checks the perp 2s
            List<LogicCell> edgeCells = GetEdgeCellIndexes(grid, xDirection, yDirection, x, y);
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
                List<LogicCell> cellsToOpen = GetNonOverlapingHiddenCells(edgeCell, grid.GetCell(x, y));

                if (cellsToOpen.Count == 1)
                {
                    cellsToOpen[0].Flag();
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

            List<(int x, int y)> diagCells = GetDiagonalCellIndexes(grid, x, y);

            foreach ((int x, int y) cornerCell in diagCells)
            {
                int xDirection = cornerCell.x - x;
                int yDirection = cornerCell.y - y;

                if (H2_2_2VCR(xDirection, yDirection, grid, x, y)) return true;
            }

            return false;
        }
        private static bool H2_2_2VCR(int xDirection, int yDirection, Grid grid, int x, int y)
        {
            // checks every corner cell is hidden
            List<LogicCell> cornerCells = GetCornerCellIndexes(grid, xDirection, yDirection, x, y);
            foreach (LogicCell cornerCell in cornerCells)
            {
                if (!cornerCell.IsHidden) return false;
            }

            // checks the perp 2s
            bool reductionDone = false;
            List<LogicCell> edgeCells = GetEdgeCellIndexes(grid, xDirection, yDirection, x, y);
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

            System.Diagnostics.Debug.WriteLine("Found at " + x + "," + y);

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
                List<LogicCell> cellsToOpen = GetNonOverlapingNonFlaggedHiddenCells(edgeCell, grid.GetCell(x, y));

                if (cellsToOpen.Count == 1)
                {
                    cellsToOpen[0].Flag();
                    returnValue = true;
                }
            }

            return returnValue;
        }
        public static bool PT_Pattern(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 1 || currentCell.AdjacentHiddenCellsCount != 2) return false;

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                // the bit in bounds is one more in the directin than H1 as it extends the hole
                if (grid.IsInBounds(x + xDirection * 3, y + yDirection * 3) &&
                    !grid.GetCellTup(perpCell).IsHidden &&
                    grid.GetCellTup(perpCell).Value == 1 &&
                    grid.GetCellTup(perpCell).EffectiveValue == 1 &&
                    grid.GetCellTup(perpCell).AdjacentHiddenCellsCount == 2)
                {
                    List<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
                    bool wallCellsHidden = wallCells.Count == 2;
                    foreach ((int x, int y) wallCell in wallCells)
                    {
                        wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden;
                    }

                    if (wallCellsHidden)
                    {
                        // H1 is already done at this point
                        (int x, int y) holeCell = (perpCell.x + xDirection, perpCell.y + yDirection);

                        if (grid.GetCellTup(holeCell).Value == 3 && grid.GetCellTup(holeCell).EffectiveValue == 3)
                        {
                            System.Diagnostics.Debug.WriteLine("found valid 3");
                            (int x, int y) threeHoleCell = (perpCell.x + xDirection, perpCell.y + yDirection);
                            (int xDirection, int yDirection) twoHoleCell = (0, 0);
                            (int xDirection, int yDirection) fourHoleCell = (0, 0);
                            List<(int x, int y)> endHoleCells = GetWallCells(holeCell, perpCell.x, perpCell.y, grid);
                            bool foundFour = false;
                            bool foundTwo = false;

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

                            if (foundFour && foundTwo)
                            {
                                bool returnValue = false;
                                List<LogicCell> cellsToOpen = new List<LogicCell>();
                                cellsToOpen.Add(grid.GetCell(threeHoleCell.x + twoHoleCell.xDirection * 2, threeHoleCell.y + twoHoleCell.yDirection * 2));
                                System.Diagnostics.Debug.WriteLine("trying to open {0},{1}", threeHoleCell.x + twoHoleCell.xDirection * 2, threeHoleCell.y + twoHoleCell.yDirection * 2);
                                cellsToOpen.Add(grid.GetCell(threeHoleCell.x + twoHoleCell.xDirection * 2 + xDirection, threeHoleCell.y + twoHoleCell.yDirection * 2 + yDirection));
                                System.Diagnostics.Debug.WriteLine("trying to open {0},{1}", threeHoleCell.x + twoHoleCell.xDirection * 2 + xDirection, threeHoleCell.y + twoHoleCell.yDirection * 2 + yDirection);
                                foreach (LogicCell cell in cellsToOpen)
                                {
                                    if (cell.IsHidden)
                                    {
                                        cell.Open();
                                        returnValue = true;
                                    }
                                }

                                LogicCell cellToFlag = grid.GetCell(threeHoleCell.x + fourHoleCell.xDirection + xDirection, threeHoleCell.y + fourHoleCell.yDirection + yDirection);
                                System.Diagnostics.Debug.WriteLine("trying to flag {0},{1}", threeHoleCell.x + fourHoleCell.xDirection + xDirection, threeHoleCell.y + fourHoleCell.yDirection + yDirection);
                                if (cellToFlag.IsHidden)
                                {
                                    cellToFlag.Flag();
                                    returnValue = true;
                                }


                                return returnValue;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
