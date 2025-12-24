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
        public bool P1_1CR(LogicCell currentCell, Grid grid, int x, int y)
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
        public bool P2_1R(LogicCell currentCell, Grid grid, int x, int y)
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
        public bool P2_1C(LogicCell currentCell, Grid grid, int x, int y)
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
        public bool P2_1CR(LogicCell currentCell, Grid grid, int x, int y)
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
                            if (cellsToFlag[index].IsFlagged)
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
        public bool PH1_H3(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 1) return false;

            int unflaggedCellsCount = 0;
            foreach (Cell adjacentCell in currentCell.AdjacentCells)
            {
                if (adjacentCell.IsHidden && !adjacentCell.IsFlagged) unflaggedCellsCount++;
            }
            if (unflaggedCellsCount != 2) return false;

            List<(int x, int y)> perpCellPoints = GetPerpendicularCellIndexes(grid, x, y);
            foreach ((int x, int y) perpCell in perpCellPoints)
            {
                if (!grid.GetCell(perpCell.x, perpCell.y).IsHidden &&
                    grid.GetCell(perpCell.x, perpCell.y).Value == 1 &&
                    grid.GetCell(perpCell.x, perpCell.y).EffectiveValue == 1)
                {
                    // splits into two sections to make it easier to handle if the hole is horisontal or vertical
                    if (perpCell.x - x == 0)
                    {
                        int yOpen = (perpCell.y - y) * 2 + y;

                        if (yOpen >= 0 &&
                            yOpen < grid.Height &&
                            x + 1 < grid.Width &&
                            grid.GetCell(x + 1, perpCell.y).IsHidden &&
                            x - 1 >= 0 &&
                            grid.GetCell(x - 1, perpCell.y).IsHidden)
                        {
                            // Opens H1 cells
                            bool changed = false;
                            for (int xOfset = -1; xOfset <= 1; xOfset++)
                            {
                                int xOpen = x + xOfset;

                                if (grid.GetCell(xOpen, yOpen).IsHidden)
                                {
                                    grid.GetCell(xOpen, yOpen).Open();
                                    changed = true;
                                }
                            }

                            // starts H2
                            // sets yOpen to the 
                            if (grid.GetCell(x, yOpen).Value == 1)
                            {
                                yOpen = (yOpen - perpCell.y) * 2 + perpCell.y;
                                if (yOpen >= 0 &&
                                    yOpen < grid.Height)
                                {
                                    for (int xOfset = -1; xOfset <= 1; xOfset++)
                                    {
                                        int xOpen = x + xOfset;

                                        if (grid.GetCell(xOpen, yOpen).IsHidden &&
                                            !grid.GetCell(xOpen, yOpen).IsFlagged)
                                        {
                                            grid.GetCell(xOpen, yOpen).Open();
                                            changed = true;
                                        }
                                    }
                                }
                            }

                            // starts H3
                            // sets yCell to the same point as checked in H1
                            int yCell = (perpCell.y - y) * 2 + y;
                            for (int xOfset = -1; xOfset <= 1; xOfset += 1)
                            {
                                int xCell = x + xOfset;
                                if (grid.GetCell(xCell, yCell).Value == 1)
                                {

                                    if (grid.GetCell(xCell, y).Value == 1 && CellsContainEachother(grid.GetCell(xCell, yCell), grid.GetCell(xCell, y)))
                                    {
                                        List<LogicCell> cellsToOpen = GetNonOverlapingHiddenCells(grid.GetCell(xCell, yCell), grid.GetCell(xCell, y));

                                        foreach (LogicCell cell in cellsToOpen)
                                        {
                                            if (cell.IsHidden)
                                            {
                                                cell.Open();
                                                changed = true;
                                            }
                                        }

                                    }
                                }
                            }

                            return changed;
                        }
                    }
                    else
                    {
                        // sets xOpen to the space 1 away from the current cell in the direction of the perp cell
                        int xOpen = (perpCell.x - x) * 2 + x;

                        if (xOpen >= 0 && 
                            xOpen < grid.Width &&
                            y + 1 < grid.Height &&
                            grid.GetCell(perpCell.x, y + 1).IsHidden &&
                            y - 1 >= 0 &&
                            grid.GetCell(perpCell.x, y - 1).IsHidden)
                        {
                            bool changed = false;
                            for (int yOfset = -1; yOfset <= 1; yOfset++)
                            {
                                int yOpen = y + yOfset;

                                if (grid.GetCell(xOpen, yOpen).IsHidden)
                                {
                                    grid.GetCell(xOpen, yOpen).Open();
                                    changed = true;
                                }
                            }

                            // starts H2
                            // sets xOpen to the space 1 away from the pervious location in the direction of the perp cell
                            if (grid.GetCell(xOpen, y).Value == 1)
                            {
                                xOpen = (xOpen - perpCell.x) * 2 + perpCell.x;
                                if (xOpen >= 0 &&
                                    xOpen < grid.Width)
                                {
                                    for (int yOfset = -1; yOfset <= 1; yOfset++)
                                    {
                                        int yOpen = y + yOfset;

                                        if (grid.GetCell(xOpen, yOpen).IsHidden &&
                                            !grid.GetCell(xOpen, yOpen).IsFlagged)
                                        {
                                            grid.GetCell(xOpen, yOpen).Open();
                                            changed = true;
                                        }
                                    }
                                }
                            }

                            // starts H3
                            // sets xCell to the same point as checked in H1
                            int xCell = (perpCell.x - x) * 2 + x;
                            for (int yOfset = -1; yOfset <= 1; yOfset += 1)
                            {
                                int yCell = y + yOfset;
                                if (grid.GetCell(xCell, yCell).Value == 1)
                                {
                                    if (grid.GetCell(x, yCell).Value == 1 && CellsContainEachother(grid.GetCell(xCell, yCell), grid.GetCell(x, yCell)))
                                    {
                                        List<LogicCell> cellsToOpen = GetNonOverlapingHiddenCells(grid.GetCell(xCell, yCell), grid.GetCell(x, yCell));

                                        foreach (LogicCell cell in cellsToOpen)
                                        {
                                            if (cell.IsHidden)
                                            {
                                                cell.Open();
                                                changed = true;
                                            }
                                        }

                                    }
                                }
                            }

                            return changed;
                        }
                    }
                }
            }

            return false;
        }
    }
}