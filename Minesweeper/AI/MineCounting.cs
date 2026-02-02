using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.AI
{
    class MineCounting
    {
        List<Frontier> frontiers;
        Grid grid;
        public MineCounting(Grid grid)
        {
            this.grid = grid;
        }

        public bool Solve()
        {
            grid.DebugDisplayGrid();

            this.frontiers = FindFrontiers(grid);
            if (frontiers.Count == 0) return false;

            foreach (Frontier frontier in frontiers)
            {
                frontier.StartCounting();
            }

            return false;
        }





        // methods used to find the forntiers
        private List<Frontier> FindFrontiers(Grid grid)
        {
            // checked cells to avoid duplicate frontiers
            List<LogicCell> checkedCells = new List<LogicCell>();
            List<Frontier> fronts = new List<Frontier>();

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.GetCell(x, y).IsHidden &&
                        !grid.GetCell(x, y).IsFlagged &&
                        CellIsAdjacentToOpen(grid.GetCell(x, y)) &&
                        !checkedCells.Contains(grid.GetCell(x, y)))
                    {
                        List<LogicCell> frontCells = new List<LogicCell>();
                        GetFrontHiddenCellsAt(grid.GetCell(x, y), frontCells, new List<LogicCell>());

                        if (frontCells.Count > 0)
                        {
                            AddCellsToChecked(checkedCells, frontCells);

                            List<LogicCell> frontierRevealedCells = GetFrontRevCells(frontCells);
                            (int value, List<int> adjacentIDs)[] frontierRevCellsInfo = new (int value, List<int> adjacentIDs)[frontierRevealedCells.Count];

                            for (int i = 0; i < frontierRevealedCells.Count; i++)
                            {
                                frontierRevCellsInfo[i].value = frontierRevealedCells[i].EffectiveValue;
                                frontierRevCellsInfo[i].adjacentIDs = GetAdjacentIDs(frontCells, frontierRevealedCells[i]);
                            }

                            System.Diagnostics.Debug.WriteLine("FOUND FRONT AT " + x + "," + y);
                            System.Diagnostics.Debug.WriteLine("COUNT: " + frontCells.Count);


                            fronts.Add(new Frontier(frontierRevCellsInfo, frontCells.Count, grid.MineCount));
                        }
                    }
                }
            }

            return fronts;
        }
        private void AddCellsToChecked(List<LogicCell> checkedCells, List<LogicCell> cellsToAdd)
        {
            foreach (LogicCell cell in cellsToAdd)
            {
                checkedCells.Add(cell);
            }
        }
        private List<int> GetAdjacentIDs(List<LogicCell> frontCells, LogicCell revealedCells)
        {
            List<int> ids = new List<int>();

            foreach (LogicCell adjacentCell in revealedCells.AdjacentCells)
            {
                if (adjacentCell.IsHidden &&
                    frontCells.Contains(adjacentCell))
                {
                    ids.Add(adjacentCell.Id);
                }
            }

            return ids;
        }
        private List<LogicCell> GetFrontRevCells(List<LogicCell> frontCells)
        {
            List<LogicCell> frontierRevealedCells = new List<LogicCell>();

            foreach (LogicCell frontCell in frontCells)
            {
                foreach (LogicCell adjacentCell in frontCell.AdjacentCells)
                {
                    if (!adjacentCell.IsHidden &&
                        !frontierRevealedCells.Contains(adjacentCell))
                    {
                        frontierRevealedCells.Add(adjacentCell);
                    }
                }
            }

            return frontierRevealedCells;
        }
        private void GetFrontHiddenCellsAt(LogicCell cell, List<LogicCell> frontCells, List<LogicCell> checkedCells)
        {
            checkedCells.Add(cell);
            if (!cell.IsFlagged) frontCells.Add(cell);

            foreach (LogicCell adjacentCell in cell.AdjacentCells)
            {
                if (!checkedCells.Contains(adjacentCell))
                {
                    if (adjacentCell.IsHidden)
                    {
                        if (CellIsAdjacentToOpen(adjacentCell))
                        {
                            GetFrontHiddenCellsAt(adjacentCell, frontCells, checkedCells);
                        }
                    }
                    else
                    {
                        GetFrontHiddenCellsAtEdge(adjacentCell, frontCells, checkedCells);
                    }
                }
            }
        }
        private void GetFrontHiddenCellsAtEdge(LogicCell cell, List<LogicCell> frontCells, List<LogicCell> checkedCells)
        {
            checkedCells.Add(cell);

            foreach (LogicCell adjacentCell in cell.AdjacentCells)
            {
                if (adjacentCell.IsHidden &&
                    CellIsAdjacentToOpen(adjacentCell) &&
                    !checkedCells.Contains(adjacentCell))
                {
                    GetFrontHiddenCellsAt(adjacentCell, frontCells, checkedCells);
                }
            }
        }
        private bool CellIsAdjacentToOpen(LogicCell cell)
        {
            foreach (LogicCell adjacentCell in cell.AdjacentCells)
            {
                if (!adjacentCell.IsHidden) return true;
            }
            return false;
        }
    }
}
