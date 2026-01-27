using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
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
                if (!adjacentCell.IsHidden &&
                    adjacentCell.EffectiveValue == 1 &&
                    (currentCell.Value != 1 || adjacentCell.Value != 1) &&
                    NonFlagCellsContainEachother(currentCell, adjacentCell))
                {
                    List<LogicCell> cells = GetNonOverlapingHiddenCells(currentCell, adjacentCell);
                    int unflaggedCells = 0;
                    int index = -1;

                    for (int i = 0; i < cells.Count; i++)
                    {
                        if (!cells[i].IsFlagged)
                        {
                            unflaggedCells++;
                            index = i;
                        }
                    }

                    if (unflaggedCells == 1)
                    {
                        cells[index].Open();
                        return true;
                    }
                }
            }

            return false;
        }
        public static bool P2_1R(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 2) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (!adjacentCell.IsHidden &&
                    adjacentCell.EffectiveValue == 1 &&
                    (currentCell.Value != 2 || adjacentCell.Value != 1))
                {
                    List<LogicCell> cells = GetNonOverlapingHiddenCells(currentCell, adjacentCell);
                    int unflaggedCells = 0;
                    int index = -1;

                    for (int i = 0; i < cells.Count; i++)
                    {
                        if (!cells[i].IsFlagged)
                        {
                            unflaggedCells++;
                            index = i;
                        }
                    }

                    if (unflaggedCells == 1)
                    {
                        cells[index].Flag();
                        return true;
                    }
                }
            }

            return false;
        }
        public static bool P2_1C(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 2 || currentCell.EffectiveValue != 2) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (!adjacentCell.IsHidden &&
                    adjacentCell.Value == 1 &&
                    adjacentCell.EffectiveValue == 1)
                {
                    List<LogicCell> cells = GetNonOverlapingHiddenCells(currentCell, adjacentCell);

                    if (cells.Count == 1)
                    {
                        List<LogicCell> cellToOpen = GetNonOverlapingHiddenCells(adjacentCell, currentCell);

                        if (cellToOpen.Count == 1)
                        {
                            cellToOpen[0].Open();
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public static bool P2_1CR(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.EffectiveValue != 2) return false;

            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (!adjacentCell.IsHidden &&
                    adjacentCell.EffectiveValue == 1 &&
                    (currentCell.Value != 2 || adjacentCell.Value != 1))
                {
                    List<LogicCell> cellsToFlag = GetNonOverlapingHiddenCells(currentCell, adjacentCell);

                    if (cellsToFlag.Count == currentCell.Value - 1)
                    {
                        int unflaggedCells = 0;
                        int index = 0;

                        while (unflaggedCells <= 1 && index < cellsToFlag.Count)
                        {
                            if (!cellsToFlag[index].IsFlagged)
                            {
                                unflaggedCells++;
                            }
                            index++;
                        }

                        if (unflaggedCells == 1)
                        {
                            List<LogicCell> cellToOpen = GetNonOverlapingHiddenCells(adjacentCell, currentCell);

                            if (cellToOpen.Count == 1 && !cellToOpen[0].IsFlagged)
                            {
                                cellToOpen[0].Open();
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
        public static bool PH1(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 1 || currentCell.AdjacentHiddenCellsCount != 2) return false;
            //System.Diagnostics.Debug.WriteLine("checking H1 at " + x + "," + y);

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                if (grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) &&
                    !grid.GetCellTup(perpCell).IsHidden &&
                    grid.GetCellTup(perpCell).Value == 1 &&
                    grid.GetCellTup(perpCell).EffectiveValue == 1 &&
                    grid.GetCellTup(perpCell).AdjacentHiddenCellsCount >= 2)
                {
                    List<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
                    bool wallCellsHidden = wallCells.Count == 2;
                    foreach ((int x, int y) wallCell in wallCells)
                    {
                        wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden;
                    }

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
        public static bool PH2(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 1 || currentCell.AdjacentHiddenCellsCount != 2) return false;
            //System.Diagnostics.Debug.WriteLine("checking H2 at " + x + "," + y);

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
                    grid.GetCellTup(perpCell).AdjacentHiddenCellsCount >= 2)
                {
                    List<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
                    bool wallCellsHidden = wallCells.Count == 2;
                    foreach ((int x, int y) wallCell in wallCells)
                    {
                        wallCellsHidden = wallCellsHidden && grid.GetCellTup(wallCell).IsHidden;
                    }

                    if (wallCellsHidden)
                    {
                        List<(int x, int y)> holeCells = GetHoleCells(perpCell, x, y, grid, wallCells);

                        // differs from H1 here as H2 needs these to not be hidden
                        bool allCellsAreOpen = true;
                        foreach ((int x, int y) cellToOpen in holeCells)
                        {
                            allCellsAreOpen = !grid.GetCellTup(cellToOpen).IsHidden && allCellsAreOpen;
                        }

                        if (allCellsAreOpen)
                        {
                            (int x, int y) holeCell = (perpCell.x + xDirection, perpCell.y + yDirection);

                            if (grid.GetCellTup(holeCell).Value == 1 && grid.GetCellTup(holeCell).EffectiveValue == 1)
                            {
                                // the end2Cells are not hidden, they are the cells next to the middle cell in the hole and represnt where the walls would be so the GetHoleCells method works
                                List<(int x, int y)> end2Cells = GetWallCells(holeCell, perpCell.x, perpCell.y, grid);
                                List<(int x, int y)> hole2Cells = GetHoleCells(holeCell, perpCell.x, perpCell.y, grid, end2Cells);

                                bool hiddenCellExists = false;
                                foreach ((int x, int y) cellToOpen in hole2Cells)
                                {
                                    hiddenCellExists = grid.GetCellTup(cellToOpen).IsHidden || hiddenCellExists;
                                }

                                if (hiddenCellExists)
                                {
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
                            }
                        }
                    }
                }
            }

            return false;
        }
        public static bool PH3(LogicCell currentCell, Grid grid, int x, int y)
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
                                grid.GetCellTup(insideWallCells[i]).Value == 1 &&
                                grid.GetCellTup(outsideWallCells[i]).Value == 1 &&
                                grid.GetCellTup(insideWallCells[i]).EffectiveValue == 1 &&
                                grid.GetCellTup(outsideWallCells[i]).EffectiveValue == 1 && 
                                CellsContainEachother(grid.GetCellTup(insideWallCells[i]), grid.GetCellTup(outsideWallCells[i])))
                            {
                                List<LogicCell> cellsToOpen = GetNonOverlapingHiddenCells(grid.GetCellTup(insideWallCells[i]), grid.GetCellTup(outsideWallCells[i]));

                                returnValue = cellsToOpen.Count > 0 || returnValue;

                                foreach (LogicCell cell in cellsToOpen)
                                {
                                    if (cell.IsHidden) cell.Open();
                                }
                            }
                        }

                        if (returnValue) return returnValue;
                    }
                }
            }

            return false;
        }
    }
}
