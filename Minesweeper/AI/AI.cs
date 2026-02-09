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
        // methods that handle general diagonal and perpendicular cells
        protected static HashSet<(int x, int y)> GetDiagonalCellIndexes(Grid grid, int x, int y)
        {
            HashSet<(int x, int y)> diagCells = new HashSet<(int x, int y)>();

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
        protected static HashSet<(int x, int y)> GetPerpendicularCellIndexes(Grid grid, int x, int y)
        {
            HashSet<(int x, int y)> perpCells = new HashSet<(int x, int y)>();

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

        
        // used in hole and triangle patterns
        protected static HashSet<(int x, int y)> GetWallCells((int x, int y) perpCell, int x, int y, Grid grid)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            HashSet<(int x, int y)> wallCells = new HashSet<(int x, int y)>();
            wallCells.Add((perpCell.x + yDirection, perpCell.y + xDirection));
            wallCells.Add((perpCell.x - yDirection, perpCell.y - xDirection));

            HashSet<(int x, int y)> wallCellsToRemove = new HashSet<(int x, int y)>();
            foreach ((int x, int y) cell in wallCells)
            {
                if (!grid.IsInBounds(cell.x, cell.y))
                {
                    wallCellsToRemove.Add(cell);
                }
            }

            foreach ((int x, int y) cell in wallCellsToRemove)
            {
                wallCells.Remove(cell);
            }

            return wallCells;
        }
        protected static HashSet<(int x, int y)> GetExtendedWallCells((int x, int y) perpCell, int x, int y, Grid grid)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            HashSet<(int x, int y)> wallCells = new HashSet<(int x, int y)>();
            wallCells.Add((perpCell.x + yDirection, perpCell.y + xDirection));
            wallCells.Add((perpCell.x - yDirection, perpCell.y - xDirection));
            wallCells.Add((perpCell.x + yDirection * 2, perpCell.y + xDirection * 2));
            wallCells.Add((perpCell.x - yDirection * 2, perpCell.y - xDirection * 2));

            HashSet<(int x, int y)> wallCellsToRemove = new HashSet<(int x, int y)>();
            foreach ((int x, int y) cell in wallCells)
            {
                if (!grid.IsInBounds(cell.x, cell.y))
                {
                    wallCellsToRemove.Add(cell);
                }
            }

            foreach ((int x, int y) cell in wallCellsToRemove)
            {
                wallCells.Remove(cell);
            }

            return wallCells;
        }
        protected static HashSet<(int x, int y)> GetHoleCells((int x, int y) perpCell, int x, int y, Grid grid, HashSet<(int x, int y)> wallCells)
        {
            int xDirection = perpCell.x - x;
            int yDirection = perpCell.y - y;

            if (wallCells.Count == 2 || (wallCells.Count == 1 && grid.GetCell(x, y).AdjacentHiddenCellsCount == 1))
            {
                HashSet<(int x, int y)> holeCells = new HashSet<(int x, int y)>();

                foreach ((int x, int y) wallCell in wallCells)
                {
                    holeCells.Add((xDirection + wallCell.x, yDirection + wallCell.y));
                }
                holeCells.Add((xDirection + perpCell.x, yDirection + perpCell.y));

                return holeCells;
            }

            return new HashSet<(int x, int y)>();
        }


        // used in the corner patterns
        protected static HashSet<LogicCell> GetCornerCells(Grid grid, int xDirection, int yDirection, int x, int y)
        {
            HashSet<LogicCell> cornerCells = new HashSet<LogicCell>();

            cornerCells.Add(grid.GetCell(x + xDirection, y + yDirection));
            cornerCells.Add(grid.GetCell(x - xDirection, y + yDirection));
            cornerCells.Add(grid.GetCell(x + xDirection, y - yDirection));
            cornerCells.Add(grid.GetCell(x, y + yDirection));
            cornerCells.Add(grid.GetCell(x + xDirection, y));

            return cornerCells;
        }
        protected static HashSet<LogicCell> GetEdgeCells(Grid grid, int xDirection, int yDirection, int x, int y)
        {
            HashSet<LogicCell> edgeCells = new HashSet<LogicCell>();

            edgeCells.Add(grid.GetCell(x - xDirection, y));
            edgeCells.Add(grid.GetCell(x, y - yDirection));

            return edgeCells;
        }


        // methods that check if a cell is contained within another
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
        

        // non overlapping cell methods
        protected static HashSet<LogicCell> GetOverlappingHiddenCells(LogicCell cell1, LogicCell cell2)
        {
            HashSet<LogicCell> overlapping = new HashSet<LogicCell>();

            foreach (LogicCell cell in cell1.AdjacentCells)
            {
                if (cell.IsHidden && cell2.AdjacentCells.Contains(cell)) overlapping.Add(cell);
            }

            return overlapping;
        }
        protected static HashSet<LogicCell> GetOverlappingNonFlaggedHiddenCells(LogicCell cell1, LogicCell cell2)
        {
            HashSet<LogicCell> overlapping = new HashSet<LogicCell>();

            foreach (LogicCell cell in cell1.AdjacentCells)
            {
                if (cell.IsHidden && !cell.IsFlagged && cell2.AdjacentCells.Contains(cell)) overlapping.Add(cell);
            }

            return overlapping;
        }


        // gets hidden cells that are adjacent to the main cell but not adjacent to the contained cell
        protected static HashSet<LogicCell> GetNonOverlapingHiddenCells(LogicCell mainCell, LogicCell containedCell)
        {
            HashSet<LogicCell> cells = new HashSet<LogicCell>();

            foreach (LogicCell adjacentCell in mainCell.AdjacentCells)
            {
                if (adjacentCell.IsHidden && !containedCell.AdjacentCells.Contains(adjacentCell)) cells.Add(adjacentCell);
            }

            return cells;
        }
        protected static HashSet<LogicCell> GetNonOverlapingNonFlaggedHiddenCells(LogicCell mainCell, LogicCell containedCell)
        {
            HashSet<LogicCell> cells = new HashSet<LogicCell>();

            foreach (LogicCell adjacentCell in mainCell.AdjacentCells)
            {
                if (adjacentCell.IsHidden && !adjacentCell.IsFlagged && !containedCell.AdjacentCells.Contains(adjacentCell)) cells.Add(adjacentCell);
            }

            return cells;
        }
    }
}