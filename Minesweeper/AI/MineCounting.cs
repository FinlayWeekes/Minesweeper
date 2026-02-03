using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.AI
{
    class MineCounting
    {
        const int sizeCutoff = 50;

        List<Frontier> frontiers;
        Grid grid;
        int minMinesTotal;
        int maxMinesTotal;
        int minMinesOfAllFronts;
        int maxMinesOfAllFronts;
        HashSet<LogicCell> leftOverCells;
        HashSet<int> totalPosMineCounts;
        public MineCounting(Grid grid)
        {
            this.grid = grid;
        }

        public bool Solve()
        {
            //grid.DebugDisplayGrid();

            this.maxMinesTotal = grid.MineCount;

            leftOverCells = FindLeftoverHidCells();
            
            minMinesTotal = Math.Max(maxMinesTotal - leftOverCells.Count, 0);

            frontiers = FindFrontiers(grid);

            // if there are no frontiers, then there is nothing that can be solved
            if (frontiers.Count == 0) return false;

            // if the frontier size is greater than 50, this could take anywhere from 0.1 to 1 seconds on my pc, so if it finds one in the 50-60 range, the board is considered impossible
            // letting the program try to search these grids will cause it to get stuck as they are usualy not solvable anyway
            // the time taken could be minutes for frontiers around 80 and these will usualy not result in any progress
            // it will be both unreasonable and unfun for players to solve frontiers of length greater than 50 if all the other patterns have already been checked
            if (CheckForUnreasonableFrontiers()) return false;

            // fills out the PosMinesForFlag and PosMinesForEmpty array of hashset of int for each frontier
            foreach (Frontier frontier in frontiers)
            {
                frontier.StartCounting();

                // checks if the frontier has found any solutions and exectues them
                // returns true if so as solving frontiers is very expesive and 
                if (frontier.ExcecuteFoundValues(grid)) return false;
            }

            // finds the sum of each grid's min and max counts
            minMinesOfAllFronts = FindSumOfMinCount();
            maxMinesOfAllFronts = FindSumOfMaxCount();

            // sums up every possible total mine count with the given frontier counts
            totalPosMineCounts = FindTotalPosCounts();

            // removes values below/above the min/max mines
            PruneTotalPosMineCount(minMinesTotal, maxMinesTotal);

            // removes invalid mine counts from each frontier's PosMineCounts
            // invalid counts which are above/below min/max are already removed
            // this takes into account the action combinations of counts and removes any counts which when added with any of the other mine counts from other frontiers are invalid
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

            // removes any values from the posMinesForFlag and posMinesForEmpty which are not in the newly pruned PosMineCounts set 
            foreach (Frontier frontier in frontiers)
            {
                frontier.PruneWithNewPossibleValues();
            }

            // executes those changes by flagging any cell which has no minecount possible for empty and opening any cell which does not have a vlaid minecount for it to be flagged
            bool changed = false;
            foreach (Frontier frontier in frontiers)
            {
                //System.Diagnostics.Debug.WriteLine("");
                //frontier.DebugData();
                changed = frontier.ExcecuteFoundValues(grid) || changed;
            }

            // checks if the minimum number of mines found is equal to the mine count,
            // meaning that all leftover cells must be empty, so can be opened
            if (FindSumOfMinCount() == minMinesTotal)
            { 
                foreach (LogicCell cell in leftOverCells)
                {
                    if (!cell.IsHidden) cell.Open();
                }

                changed = changed || leftOverCells.Count > 0;
            }

            //System.Diagnostics.Debug.WriteLine("done minecounting");
            //grid.DebugDisplayGrid();

            return changed;
        }

        private bool CheckForUnreasonableFrontiers()
        {
            foreach (Frontier frontier in frontiers)
            {
                if (frontier.Size > sizeCutoff)
                {
                    System.Diagnostics.Debug.WriteLine("unreasonable front");
                    return true;
                }
            }
            return false;
        }

        private int FindSumOfMinCount()
        {
            int count = 0;
            foreach (Frontier frontier in frontiers)
            {
                count += frontier.MinMinesInFront;
            }
            return count;
        }
        private int FindSumOfMaxCount()
        {
            int count = 0;
            foreach (Frontier frontier in frontiers)
            {
                count += frontier.MaxMinesInFront;
            }
            return count;
        }

        private void PruneTotalPosMineCount(int min, int max)
        {
            HashSet<int> valsToRemove = new HashSet<int>();
            foreach (int val in totalPosMineCounts)
            {
                if (val < min || val > max) valsToRemove.Add(val);
            }

            foreach (int val in valsToRemove)
            {
                totalPosMineCounts.Remove(val);
            }
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
        private HashSet<LogicCell> FindLeftoverHidCells()
        {
            HashSet<LogicCell> cells = new HashSet<LogicCell>();
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.GetCell(x, y).IsHidden &&
                        !grid.GetCell(x, y).IsFlagged &&
                        !CellIsAdjacentToOpen(grid.GetCell(x, y)))
                    {
                        cells.Add(grid.GetCell(x, y));
                    }
                }
            }
            return cells;
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

                            //System.Diagnostics.Debug.WriteLine("FOUND FRONT AT " + x + "," + y);
                            //System.Diagnostics.Debug.WriteLine("COUNT: " + frontCells.Count);


                            fronts.Add(new Frontier(frontierRevCellsInfo, frontCells.Count, maxMinesTotal));
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
