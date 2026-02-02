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
        int leftoverHidCellCount;
        int minMinesTotal;
        int maxMinesTotal;
        int minMinesOfAllFronts;
        int maxMinesOfAllFronts;
        HashSet<int> totalPosMineCounts;
        public MineCounting(Grid grid)
        {
            this.grid = grid;
        }

        public bool Solve()
        {
            grid.DebugDisplayGrid();

            this.maxMinesTotal = grid.MineCount;

            leftoverHidCellCount = FindLeftoverHidCells();
            minMinesTotal = Math.Min(maxMinesTotal - leftoverHidCellCount, 0);

            frontiers = FindFrontiers(grid);

            if (frontiers.Count == 0) return false;

            // fills out the PosMinesForFlag and PosMinesForEmpty array of hashset of int for each frontier
            // also sums up their min and max number of mines in the same loop
            minMinesOfAllFronts = 0;
            maxMinesOfAllFronts = 0;
            foreach (Frontier frontier in frontiers)
            {
                frontier.StartCounting();
                minMinesOfAllFronts += frontier.MinMinesInFront;
                maxMinesOfAllFronts += frontier.MaxMinesInFront;
            }

            // sums up every possible total mine count with the given frontier counts
            totalPosMineCounts = FindTotalPosCounts();

            foreach (Frontier frontier in frontiers)
            {
                int minCountOtherFronts = minMinesOfAllFronts - frontier.MinMinesInFront;
                int maxCountOtherFronts = maxMinesOfAllFronts - frontier.MaxMinesInFront;

                HashSet<int> validCounts = new HashSet<int>();

                foreach (int val in frontier.PosMineCounts)
                {
                    if (MineCountIsValid(val, frontier, minCountOtherFronts, maxCountOtherFronts)) validCounts.Add(val);
                }

                frontier.PosMineCounts = validCounts;
            }

            foreach (Frontier frontier in frontiers)
            {
                frontier.PruneWithNewPossibleValues();
            }

            bool changed = false;
            foreach (Frontier frontier in frontiers)
            {
                System.Diagnostics.Debug.WriteLine("");
                frontier.DebugData();
                changed = frontier.ExcecuteFoundValues(grid) || changed;
            }

            System.Diagnostics.Debug.WriteLine("done minecounting");
            grid.DebugDisplayGrid();

            return changed;
        }

        private bool MineCountIsValid(int count, Frontier frontier, int minCountOtherFronts, int maxCountOtherFronts)
        {
            foreach (int possibleCount in totalPosMineCounts)
            {
                int remaningMines = possibleCount - count;

                if (remaningMines >= minCountOtherFronts && remaningMines <= maxCountOtherFronts)
                {
                    return true; 
                }
            }

            return false;
        }

        private HashSet<int> FindTotalPosCounts()
        {
            HashSet<int> prevMineCounts = new HashSet<int>();
            prevMineCounts.Add(0);

            foreach (Frontier frontier in frontiers)
            {
                HashSet<int> nextSet = new HashSet<int>();

                foreach (int prevCount in prevMineCounts)
                {
                    foreach (int thisCount in frontier.PosMineCounts)
                    {
                        nextSet.Add(prevCount + thisCount);
                    }
                }

                prevMineCounts = nextSet;
            }

            return prevMineCounts;
        }
        private int FindLeftoverHidCells()
        {
            int cellCount = 0;
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.GetCell(x, y).IsHidden &&
                        !grid.GetCell(x, y).IsFlagged &&
                        !CellIsAdjacentToOpen(grid.GetCell(x, y)))
                    {
                        leftoverHidCellCount++;
                    }
                }
            }
            return cellCount;
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


                            fronts.Add(new Frontier(frontierRevCellsInfo, frontCells.Count, maxMinesTotal, minMinesTotal));
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
