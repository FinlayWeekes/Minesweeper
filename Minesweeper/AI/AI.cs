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
        protected List<LogicCell> GetDiagonalCells(Grid grid, int x, int y)
        {
            List<LogicCell> diagCells = new List<LogicCell>();
            List<(int x, int y)> indexes = this.GetDiagonalCellIndexes(grid, x, y);

            foreach ((int x, int y) point in indexes)
            {
                diagCells.Add(grid.GetCell(point.x, point.y));
            }

            return diagCells;
        }
        protected List<LogicCell> GetPerpendicularCells(Grid grid, int x, int y)
        {
            List<LogicCell> perpCells = new List<LogicCell>();
            List<(int x, int y)> indexes = this.GetPerpendicularCellIndexes(grid, x, y);

            foreach ((int x, int y) point in indexes)
            {
                perpCells.Add(grid.GetCell(point.x, point.y));
            }

            return perpCells;
        }
        protected List<(int x, int y)> GetDiagonalCellIndexes(Grid grid, int x, int y)
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
        protected List<(int x, int y)> GetPerpendicularCellIndexes(Grid grid, int x, int y)
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
        protected bool CellsContainEachother(LogicCell mainCell, LogicCell containedCell)
        {
            foreach (LogicCell cell in containedCell.AdjacentCells)
            {
                if (cell.IsHidden && !mainCell.AdjacentCells.Contains(cell)) return false;
            }

            return true;
        }
        
        // gets hidden cells that are adjacent to the main cell but not adjacent to the contained cell
        protected List<LogicCell> GetNonOverlapingHiddenCells(LogicCell mainCell, LogicCell containedCell)
        {
            List<LogicCell> cells = new List<LogicCell>();

            foreach (LogicCell adjacentCell in mainCell.AdjacentCells)
            {
                if (adjacentCell.IsHidden && !containedCell.AdjacentCells.Contains(adjacentCell)) cells.Add(adjacentCell);
            }


            return cells;
        }
    }
}