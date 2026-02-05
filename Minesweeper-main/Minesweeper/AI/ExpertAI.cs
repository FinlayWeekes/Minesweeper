using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows.Forms;

namespace Minesweeper.AI
{
    class ExpertAI : AI
    {
        public static bool P1_1CRx(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 1) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (!adjacentCell.IsHidden &&
                    adjacentCell.EffectiveValue == 1 &&
                    NonFlagCellsContainEachother(currentCell, adjacentCell) &&
                    (currentCell.Value != 1 || adjacentCell.Value != 1))
                {
                    List<LogicCell> cellsToOpen = GetNonOverlapingNonFlaggedHiddenCells(currentCell, adjacentCell);

                    if (cellsToOpen.Count > 1)
                    {
                        foreach (LogicCell cell in cellsToOpen)
                        {
                            if (cell.IsHidden) cell.Open();
                        }

                        return true;
                    }
                }
            }

            return false;
        }
        public static bool P2_1CRx(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue < 2) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (!adjacentCell.IsHidden &&
                    adjacentCell.EffectiveValue == 1 &&
                    (adjacentCell.Value != adjacentCell.EffectiveValue || currentCell.Value != currentCell.EffectiveValue))
                {
                    List<LogicCell> overlappingCells = GetOverlappingNonFlaggedHiddenCells(currentCell, adjacentCell);

                    if (overlappingCells.Count > 0)
                    {
                        List<LogicCell> mineCells = GetNonOverlapingNonFlaggedHiddenCells(currentCell, adjacentCell);

                        // the -1 represents the mine in the overlapping cells due to the effective 1
                        if (mineCells.Count == currentCell.EffectiveValue - 1)
                        {
                            List<LogicCell> cellsToOpen = GetNonOverlapingNonFlaggedHiddenCells(adjacentCell, currentCell);

                            if (cellsToOpen.Count > 1)
                            {
                                foreach (LogicCell cellToOpen in cellsToOpen)
                                {
                                    if (cellToOpen.IsHidden) cellToOpen.Open();
                                }

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
        public static bool P2_1Rx(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue <= 2) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (!adjacentCell.IsHidden &&
                    adjacentCell.EffectiveValue == 1 &&
                    (adjacentCell.Value != adjacentCell.EffectiveValue || currentCell.Value != currentCell.EffectiveValue))
                {
                    List<LogicCell> overlappingCells = GetOverlappingNonFlaggedHiddenCells(currentCell, adjacentCell);

                    if (overlappingCells.Count > 0)
                    {
                        List<LogicCell> cellsToFlag = GetNonOverlapingNonFlaggedHiddenCells(currentCell, adjacentCell);

                        // the -1 represents the mine in the overlapping cells due to the effective 1
                        if (cellsToFlag.Count == currentCell.EffectiveValue - 1)
                        {
                            foreach (LogicCell cellToFlag in cellsToFlag)
                            {
                                cellToFlag.Flag();
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public static bool PH1R(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 1 || currentCell.AdjacentHiddenCellsCount - currentCell.Value + currentCell.EffectiveValue != 2) return false;

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                if (grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) &&
                    !grid.GetCellTup(perpCell).IsHidden &&
                    grid.GetCellTup(perpCell).EffectiveValue == 1 &&
                    grid.GetCellTup(perpCell).AdjacentHiddenCellsCount >= 2 &&
                    (grid.GetCellTup(perpCell).Value != grid.GetCellTup(perpCell).EffectiveValue || currentCell.EffectiveValue != currentCell.Value))
                {
                    List<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
                    bool wallCellsHidden = wallCells.Count == 2;
                    foreach ((int x, int y) wallCell in wallCells)
                    {
                        wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden && !grid.GetCellTup(wallCell).IsFlagged;
                    }

                    // valid if all the wall cells are hidden and not flagged
                    if (wallCellsHidden)
                    {
                        List<(int x, int y)> holeCells = GetHoleCells(perpCell, x, y, grid, wallCells);

                        bool hiddenCellExists = false;
                        foreach ((int x, int y) cellToOpen in holeCells)
                        {
                            hiddenCellExists = (grid.GetCellTup(cellToOpen).IsHidden && !grid.GetCellTup(cellToOpen).IsFlagged) || hiddenCellExists;
                        }

                        // valid if there is at least one hole cell that is hidden and not flagged
                        if (hiddenCellExists)
                        {
                            foreach ((int x, int y) cellToOpen in holeCells)
                            {
                                if (grid.GetCellTup(cellToOpen).IsHidden && !grid.GetCellTup(cellToOpen).IsFlagged) grid.GetCellTup(cellToOpen).Open();
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public static bool PH2R(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 1 || currentCell.AdjacentHiddenNonFlagCellsCount != 2) return false; 

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                // the bit in bounds is one more in the direction than H1 as it extends the hole
                if (grid.IsInBounds(x + xDirection * 3, y + yDirection * 3) &&
                    !grid.GetCellTup(perpCell).IsHidden &&
                    grid.GetCellTup(perpCell).EffectiveValue == 1 &&
                    grid.GetCellTup(perpCell).AdjacentHiddenCellsCount >= 2)
                {
                    List<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
                    bool wallCellsHidden = wallCells.Count == 2;
                    foreach ((int x, int y) wallCell in wallCells)
                    {
                        wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden && !grid.GetCellTup(wallCell).IsFlagged;
                    }

                    // all the wall cells need to be hidden and not flagged
                    if (wallCellsHidden)
                    {
                        List<(int x, int y)> hole1Cells = GetHoleCells(perpCell, x, y, grid, wallCells);

                        // differs from H1 here as H2 needs these to not be hidden
                        bool allCellsAreOpen = true;
                        foreach ((int x, int y) hole1Cell in hole1Cells)
                        {
                            allCellsAreOpen = !grid.GetCellTup(hole1Cell).IsHidden && allCellsAreOpen;
                        }

                        if (allCellsAreOpen)
                        {
                            (int x, int y) holeCell = (perpCell.x + xDirection, perpCell.y + yDirection);

                            if (grid.GetCellTup(holeCell).EffectiveValue == 1 &&
                                (grid.GetCellTup(holeCell).EffectiveValue != grid.GetCellTup(holeCell).Value || grid.GetCellTup(perpCell).EffectiveValue != grid.GetCellTup(perpCell).Value || currentCell.EffectiveValue != currentCell.Value))
                            {
                                // the end2Cells are not hidden, they are the cells next to the middle cell in the hole and represnt where the walls would be so the GetHoleCells method works
                                List<(int x, int y)> end2Cells = GetWallCells(holeCell, perpCell.x, perpCell.y, grid);
                                List<(int x, int y)> hole2Cells = GetHoleCells(holeCell, perpCell.x, perpCell.y, grid, end2Cells);

                                bool hiddenCellExists = false;
                                foreach ((int x, int y) cellToOpen in hole2Cells)
                                {
                                    hiddenCellExists = (grid.GetCellTup(cellToOpen).IsHidden && !grid.GetCellTup(cellToOpen).IsFlagged) || hiddenCellExists;
                                }

                                // needs at leat one of the hole 2 cells to be hidden and not flagged
                                if (hiddenCellExists)
                                {
                                    foreach ((int x, int y) cellToOpen in hole2Cells)
                                    {
                                        if (grid.GetCellTup(cellToOpen).IsHidden && !grid.GetCellTup(cellToOpen).IsFlagged)
                                        {
                                            grid.GetCellTup(cellToOpen).Open();
                                        }
                                    }
                                    //System.Diagnostics.Debug.WriteLine("found H2 at " + x + "," + y);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
        public static bool PH3R(LogicCell currentCell, Grid grid, int x, int y)
        {
            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                if (grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) &&
                    !grid.GetCellTup(perpCell).IsHidden)
                {
                    (int x, int y) holeCell1 = (perpCell.x + xDirection, perpCell.y + yDirection);

                    if (!grid.GetCellTup(holeCell1).IsHidden)
                    {
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
                                grid.GetCellTup(insideWallCells[i]).EffectiveValue == 1 &&
                                grid.GetCellTup(outsideWallCells[i]).EffectiveValue == 1 &&
                                NonFlagCellsContainEachother(grid.GetCellTup(insideWallCells[i]), grid.GetCellTup(outsideWallCells[i])) &&
                                (grid.GetCellTup(insideWallCells[i]).Value != 1 || grid.GetCellTup(outsideWallCells[i]).Value != 1))
                            {
                                List<LogicCell> cellsToOpen = GetNonOverlapingNonFlaggedHiddenCells(grid.GetCellTup(insideWallCells[i]), grid.GetCellTup(outsideWallCells[i]));

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

            return false;
        }
        public static bool P1_1T(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 1 || currentCell.EffectiveValue != 1 || currentCell.AdjacentHiddenCellsCount != 3) return false;

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                if (grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) &&
                    !grid.GetCellTup(perpCell).IsHidden &&
                    grid.GetCellTup(perpCell).Value == 1 &&
                    grid.GetCellTup(perpCell).EffectiveValue == 1 &&
                    grid.GetCellTup(perpCell).AdjacentHiddenCellsCount >= 4 &&
                    CellsContainEachother(grid.GetCellTup(perpCell), currentCell))
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
                        List<(int x, int y)> holeCells = GetHoleCells(perpCell, x, y, grid, wallCells);

                        bool hiddenCellExists = false;
                        foreach ((int x, int y) cellToOpen in holeCells)
                        {
                            hiddenCellExists = grid.GetCellTup(cellToOpen).IsHidden || hiddenCellExists;
                        }

                        if (hiddenCellExists)
                        {
                            foreach ((int x, int y) cellToOpen in holeCells)
                            {
                                if (grid.GetCellTup(cellToOpen).IsHidden) grid.GetCellTup(cellToOpen).Open();
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public static bool P2_2T(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 2 || currentCell.EffectiveValue != 2 || currentCell.AdjacentHiddenCellsCount != 3) return false;

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                if (grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) &&
                    !grid.GetCellTup(perpCell).IsHidden &&
                    grid.GetCellTup(perpCell).Value == 2 &&
                    grid.GetCellTup(perpCell).EffectiveValue == 2 &&
                    grid.GetCellTup(perpCell).AdjacentHiddenCellsCount >= 4 &&
                    CellsContainEachother(grid.GetCellTup(perpCell), currentCell))
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
                        List<LogicCell> holeCells = GetNonOverlapingHiddenCells(grid.GetCellTup(perpCell), currentCell);

                        foreach (LogicCell cellToOpen in holeCells)
                        {
                            if (cellToOpen.IsHidden) cellToOpen.Open();
                        }

                        return true;
                    }
                }
            }

            return false;
        }
        public static bool P3_2T(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 3 || currentCell.EffectiveValue != 3 || currentCell.AdjacentHiddenCellsCount != 4) return false;

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                if (grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) &&
                    !grid.GetCellTup(perpCell).IsHidden &&
                    grid.GetCellTup(perpCell).Value == 2 &&
                    grid.GetCellTup(perpCell).EffectiveValue == 2 &&
                    grid.GetCellTup(perpCell).AdjacentHiddenCellsCount >= 4)
                {
                    List<LogicCell> cellToFlag = GetNonOverlapingHiddenCells(currentCell, grid.GetCellTup(perpCell));

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
                            List<LogicCell> holeCells = GetNonOverlapingHiddenCells(grid.GetCellTup(perpCell), currentCell);

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
    }
}