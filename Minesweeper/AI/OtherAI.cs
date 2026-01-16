using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.Windows.Forms;

namespace Minesweeper.AI
{
    class OtherAI : AI
    {
        public static bool PH1_H3(LogicCell currentCell, Grid grid, int x, int y)
        {
            if (currentCell.Value != 1 && currentCell.AdjacentHiddenCellsCount != 2) return false;

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