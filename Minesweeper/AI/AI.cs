using System;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Data;
using System.Collections;

namespace Minesweeper.AI
{
    abstract class AI
    {
        protected static List<LogicCell> GetDiagonalCells(Grid grid, int x, int y)
        {
            List<LogicCell> diagCells = new List<LogicCell>();
            List<(int x, int y)> indexes = GetDiagonalCellIndexes(grid, x, y);

            foreach ((int x, int y) point in indexes)
            {
                diagCells.Add(grid.GetCell(point.x, point.y));
            }

            return diagCells;
        }
        protected static List<LogicCell> GetPerpendicularCells(Grid grid, int x, int y)
        {
            List<LogicCell> perpCells = new List<LogicCell>();
            List<(int x, int y)> indexes = GetPerpendicularCellIndexes(grid, x, y);

            foreach ((int x, int y) point in indexes)
            {
                perpCells.Add(grid.GetCell(point.x, point.y));
            }

            return perpCells;
        }
        protected static List<(int x, int y)> GetDiagonalCellIndexes(Grid grid, int x, int y)
        {
            List<(int x, int y)> diagCells = new List<(int x, int y)>();

            for (int xOfset = -1; xOfset <= 1; xOfset += 2)
            {
                if (x + xOfset >= 0 &&
                    x + xOfset < grid.Width)
                {
                    for (int yOfset = -1; yOfset <= 1; yOfset += 2)
                    {
                        if (y + yOfset >= 0 &&
                            y + yOfset < grid.Height)
                        {
                            diagCells.Add((x + xOfset, y + yOfset));
                        }
                    }
                }
            }

            return diagCells;
        }
        protected static List<(int x, int y)> GetPerpendicularCellIndexes(Grid grid, int x, int y)
        {
            List<(int x, int y)> perpCells = new List<(int x, int y)>();

            for (int xOfset = -1; xOfset <= 1; xOfset++)
            {
                if (x + xOfset >= 0 &&
                    x + xOfset < grid.Width)
                {
                    if (xOfset == 0)
                    {
                        for (int yOfset = -1; yOfset <= 1; yOfset += 2)
                        {
                            if (y + yOfset >= 0 && 
                                y + yOfset < grid.Height)
                            {
                                perpCells.Add((x, y + yOfset));
                            }
                        }
                    }
                    else
                    {
                        perpCells.Add((x + xOfset, y));
                    }
                }
            }

            return perpCells;
        }

        protected static int FlaggedCellsCount(List<LogicCell> cells)
        {
            int count = 0;

            foreach (LogicCell cell in cells)
            {
                if (cell.IsFlagged) count++;
            }

            return count;
        }

        protected static List<(int x, int y)> GetWallCells((int x, int y) perpCell, int x, int y, Grid grid)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            List<(int x, int y)> wallCells = new List<(int x, int y)>();
            (int x, int y) hiddenCell1 = (perpCell.x + yDirection, perpCell.y + xDirection);
            (int x, int y) hiddenCell2 = (perpCell.x - yDirection, perpCell.y - xDirection);

            if (grid.IsInBounds(hiddenCell1.x, hiddenCell1.y)) wallCells.Add(hiddenCell1);
            if (grid.IsInBounds(hiddenCell2.x, hiddenCell2.y)) wallCells.Add(hiddenCell2);

            return wallCells;
        }
        protected static List<(int x, int y)> GetHoleCells((int x, int y) perpCell, int x, int y, Grid grid, List<(int x, int y)> wallCells)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            if (wallCells.Count == 2 || (wallCells.Count == 1 && grid.GetCell(x, y).AdjacentHiddenCellsCount == 1))
            {
                List<(int x, int y)> holeCells = new List<(int x, int y)>();

                foreach ((int x, int y) wallCell in wallCells)
                {
                    holeCells.Add((xDirection + wallCell.x, yDirection + wallCell.y));
                }
                holeCells.Add((xDirection + perpCell.x, yDirection + perpCell.y));

                return holeCells;
            }

            return new List<(int x, int y)>();
        }

        protected static List<LogicCell> GetOverlappingHiddenCells(LogicCell cell1, LogicCell cell2)
        {
            List<LogicCell> overlapping = new List<LogicCell>();

            foreach (LogicCell cell in cell1.AdjacentCells)
            {
                if (cell.IsHidden && cell2.AdjacentCells.Contains(cell)) overlapping.Add(cell);
            }

            return overlapping;
        }
        protected static List<LogicCell> GetOverlappingNonFlaggedHiddenCells(LogicCell cell1, LogicCell cell2)
        {
            List<LogicCell> overlapping = new List<LogicCell>();

            foreach (LogicCell cell in cell1.AdjacentCells)
            {
                if (cell.IsHidden && !cell.IsFlagged && cell2.AdjacentCells.Contains(cell)) overlapping.Add(cell);
            }

            return overlapping;
        }

        protected static bool NonFlagCellsContainEachother(LogicCell mainCell, LogicCell containedCell)
        {
            foreach (LogicCell cell in containedCell.AdjacentCells)
            {
                if (cell.IsHidden && !cell.IsFlagged && !mainCell.AdjacentCells.Contains(cell)) return false;
            }

            return true;
        }
        protected static bool CellsContainEachother(LogicCell mainCell, LogicCell containedCell)
        {
            foreach (LogicCell cell in containedCell.AdjacentCells)
            {
                if (cell.IsHidden && !mainCell.AdjacentCells.Contains(cell)) return false;
            }

            return true;
        }
        
        // gets hidden cells that are adjacent to the main cell but not adjacent to the contained cell
        protected static List<LogicCell> GetNonOverlapingHiddenCells(LogicCell mainCell, LogicCell containedCell)
        {
            List<LogicCell> cells = new List<LogicCell>();

            foreach (LogicCell adjacentCell in mainCell.AdjacentCells)
            {
                if (adjacentCell.IsHidden && !containedCell.AdjacentCells.Contains(adjacentCell)) cells.Add(adjacentCell);
            }

            return cells;
        }
        protected static List<LogicCell> GetNonOverlapingNonFlaggedHiddenCells(LogicCell mainCell, LogicCell containedCell)
        {
            List<LogicCell> cells = new List<LogicCell>();

            foreach (LogicCell adjacentCell in mainCell.AdjacentCells)
            {
                if (adjacentCell.IsHidden && !adjacentCell.IsFlagged && !containedCell.AdjacentCells.Contains(adjacentCell)) cells.Add(adjacentCell);
            }

            return cells;
        }
    }
}