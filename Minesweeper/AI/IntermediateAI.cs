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
                    CellsContainEachother(currentCell, adjacentCell))
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
            if (currentCell.Value != 1 || currentCell.AdjacentHiddenCellsCount > 2) return false;
            //System.Diagnostics.Debug.WriteLine("checking H1 at " + x + "," + y);

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                if (grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) &&
                    !grid.GetCell(perpCell.x, perpCell.y).IsHidden &&
                    grid.GetCell(perpCell.x, perpCell.y).Value == 1 &&
                    grid.GetCell(perpCell.x, perpCell.y).EffectiveValue == 1 &&
                    grid.GetCell(perpCell.x, perpCell.y).AdjacentHiddenCellsCount >= 2)
                {
                    List<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
                    bool wallCellsHidden = wallCells.Count > 0;
                    foreach ((int x, int y) wallCell in wallCells)
                    {
                        wallCellsHidden = wallCellsHidden && grid.GetCell(wallCell.x, wallCell.y).IsHidden;
                    }

                    if (wallCellsHidden)
                    {
                        List<(int x, int y)> holeCells = GetHoleCells(perpCell, x, y, grid, wallCells);

                        bool hiddenCellExists = false;
                        foreach ((int x, int y) cellToOpen in holeCells)
                        {
                            hiddenCellExists = grid.GetCell(cellToOpen.x, cellToOpen.y).IsHidden || hiddenCellExists;
                        }

                        if (hiddenCellExists)
                        {
                            foreach ((int x, int y) cellToOpen in holeCells)
                            {
                                if (grid.GetCell(cellToOpen.x, cellToOpen.y).IsHidden) grid.GetCell(cellToOpen.x, cellToOpen.y).Open();
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
            if (currentCell.Value != 1 || currentCell.AdjacentHiddenCellsCount > 2) return false;
            //System.Diagnostics.Debug.WriteLine("checking H2 at " + x + "," + y);

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                // the bit in bounds is one more in the directin than H1 as it extends the hole
                if (grid.IsInBounds(x + xDirection * 3, y + yDirection * 3) &&
                    !grid.GetCell(perpCell.x, perpCell.y).IsHidden &&
                    grid.GetCell(perpCell.x, perpCell.y).Value == 1 &&
                    grid.GetCell(perpCell.x, perpCell.y).EffectiveValue == 1 &&
                    grid.GetCell(perpCell.x, perpCell.y).AdjacentHiddenCellsCount >= 2)
                {
                    List<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
                    bool wallCellsHidden = wallCells.Count > 0;
                    foreach ((int x, int y) wallCell in wallCells)
                    {
                        wallCellsHidden = wallCellsHidden && grid.GetCell(wallCell.x, wallCell.y).IsHidden;
                    }

                    if (wallCellsHidden)
                    {
                        List<(int x, int y)> holeCells = GetHoleCells(perpCell, x, y, grid, wallCells);

                        // differs from H1 here as H2 needs these to not be hidden
                        bool allCellsAreOpen = true;
                        foreach ((int x, int y) cellToOpen in holeCells)
                        {
                            allCellsAreOpen = !grid.GetCell(cellToOpen.x, cellToOpen.y).IsHidden && allCellsAreOpen;
                        }

                        if (allCellsAreOpen)
                        {
                            (int x, int y) holeCell = (perpCell.x + xDirection, perpCell.y + yDirection);

                            if (grid.GetCell(holeCell.x, holeCell.y).Value == 1 && grid.GetCell(holeCell.x, holeCell.y).EffectiveValue == 1)
                            {
                                // the end2Cells are not hidden, they are the cells next to the middle cell in the hole and represnt where the walls would be so the GetHoleCells method works
                                List<(int x, int y)> end2Cells = GetWallCells(holeCell, perpCell.x, perpCell.y, grid);
                                List<(int x, int y)> hole2Cells = GetHoleCells(holeCell, perpCell.x, perpCell.y, grid, end2Cells);

                                bool hiddenCellExists = false;
                                foreach ((int x, int y) cellToOpen in hole2Cells)
                                {
                                    hiddenCellExists = grid.GetCell(cellToOpen.x, cellToOpen.y).IsHidden || hiddenCellExists;
                                }

                                if (hiddenCellExists)
                                {
                                    foreach ((int x, int y) cellToOpen in hole2Cells)
                                    {
                                        if (grid.GetCell(cellToOpen.x, cellToOpen.y).IsHidden)
                                        {
                                            grid.GetCell(cellToOpen.x, cellToOpen.y).Open();
                                        }
                                    }

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
            if (currentCell.Value != 1 || currentCell.AdjacentHiddenCellsCount > 2) return false;
            //System.Diagnostics.Debug.WriteLine("checking H3 at " + x + "," + y);

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                int xDirection = perpCell.x - x;
                int yDirection = perpCell.y - y;

                if (grid.IsInBounds(x + xDirection * 2, y + yDirection * 2) &&
                    !grid.GetCell(perpCell.x, perpCell.y).IsHidden &&
                    grid.GetCell(perpCell.x, perpCell.y).Value == 1 &&
                    grid.GetCell(perpCell.x, perpCell.y).EffectiveValue == 1 &&
                    grid.GetCell(perpCell.x, perpCell.y).AdjacentHiddenCellsCount >= 2)
                {
                    List<(int x, int y)> wallCells = GetWallCells(perpCell, x, y, grid);
                    bool wallCellsHidden = wallCells.Count > 0;
                    foreach ((int x, int y) wallCell in wallCells)
                    {
                        wallCellsHidden = wallCellsHidden && grid.GetCell(wallCell.x, wallCell.y).IsHidden;
                    }

                    if (wallCellsHidden)
                    {
                        List<(int x, int y)> holeCells = GetHoleCells(perpCell, x, y, grid, wallCells);

                        // differs from H1 here as H3 needs these to not be hidden
                        bool allCellsAreOpen = true;
                        foreach ((int x, int y) cellToOpen in holeCells)
                        {
                            allCellsAreOpen = !grid.GetCell(cellToOpen.x, cellToOpen.y).IsHidden && allCellsAreOpen;
                        }

                        if (allCellsAreOpen)
                        {
                            // gets the cells outside and checks they are 1s with effective value 1
                            System.Diagnostics.Debug.WriteLine("found H1 at " + x + "," + y);
                            grid.DebugDisplayGrid();
                            List<(int x, int y)> outsideWallCells = new List<(int x, int y)>();
                            outsideWallCells.Add((x + yDirection, y + xDirection));
                            outsideWallCells.Add((x - yDirection, y - xDirection));

                            for (int i = 0; i < outsideWallCells.Count; i++)
                            {
                                if (grid.GetCell(outsideWallCells[i].x, outsideWallCells[i].y).Value != 1 ||
                                    grid.GetCell(outsideWallCells[i].x, outsideWallCells[i].y).EffectiveValue != 1)
                                {
                                    outsideWallCells.RemoveAt(i);
                                    i--;
                                }
                            }

                            foreach ((int x, int y) outsideWallCell in outsideWallCells)
                            {
                                (int x, int y) insideWallCell = (outsideWallCell.x + xDirection * 2, outsideWallCell.y + yDirection * 2);

                                if (grid.GetCell(insideWallCell.x, insideWallCell.y).Value != 1 ||
                                    grid.GetCell(insideWallCell.x, insideWallCell.y).EffectiveValue != 1)
                                {
                                    List<LogicCell> cellsToOpen = GetNonOverlapingHiddenCells(grid.GetCell(insideWallCell.x, insideWallCell.y), grid.GetCell(outsideWallCell.x, outsideWallCell.y));

                                    if (cellsToOpen.Count > 0)
                                    {
                                        foreach (LogicCell cellToOpen in cellsToOpen)
                                        {
                                            cellToOpen.Open();
                                        }

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
