using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.AI
{
    class MineCounting
    {
        private HashSet<Frontier> frontiers;
        private Grid grid;
        private int minMinesTotal;
        private int maxMinesTotal;
        private int minMinesOfAllFronts;
        private int maxMinesOfAllFronts;
        private HashSet<LogicCell> leftoverCells;
        private HashSet<int> totalPosMineCounts;
        public MineCounting(Grid grid)
        {
            this.grid = grid;
        }

        public bool Solve()
        {
            //grid.DebugDisplayGrid();

            this.maxMinesTotal = grid.GetMineCount();

            leftoverCells = FindLeftoverHidCells();
            
            minMinesTotal = Math.Max(maxMinesTotal - leftoverCells.Count, 0);

            frontiers = FindFrontiers(grid);

            // if there are no frontiers, then it checks if the mine count is 0 meaning the leftover cells can be opened or, the lefover counts can all be flagged
            if (frontiers.Count == 0)
            {
                if (leftoverCells.Count == maxMinesTotal)
                {
                    return FlagLeftoverCells();
                }

                if (maxMinesTotal == 0)
                {
                    return OpenLeftoverCells();
                }

                return false;
            }

            // if the frontier size is greater than 50, this could take anywhere from 0.1 to 1 seconds on my pc, so if it finds one in the 50-60 range, the board is considered impossible
            // letting the program try to search these grids will cause it to get stuck as they are usualy not solvable anyway
            // the time taken could be minutes for frontiers around 80 and these will usualy not result in any progress
            // it will be both unreasonable and unfun for players to solve frontiers of length greater than 50 if all the other patterns have already been checked
            if (CheckForUnreasonableFrontiers()) return false;

            // fills out the PosMinesForFlag and PosMinesForEmpty array of hashset of int for each frontier+		hiddenCells	{Minesweeper.AI.Frontier.HiddenCell[6]}	Minesweeper.AI.Frontier.HiddenCell[]

            foreach (Frontier frontier in frontiers)
            {
                frontier.StartCounting();

                // checks if the frontier has found any solutions and exectues them
                // returns true if so as solving frontiers is very expesive and 
                if (frontier.ExcecuteFoundValues(grid)) return false;
            }

            // sums up every possible total mine count with the given frontier counts
            totalPosMineCounts = FindTotalPosCounts();

            // finds the sum of each grid's min and max counts
            minMinesOfAllFronts = totalPosMineCounts.Min();
            maxMinesOfAllFronts = totalPosMineCounts.Max();

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

            // checks if the minimum number of mines found is equal to the maximum possible mines,
            // meaning that all leftover cells must be empty, so can be opened
            minMinesOfAllFronts = FindSumOfMinCount();
            if (minMinesOfAllFronts == maxMinesTotal)
            {
                OpenLeftoverCells();

                changed = changed || leftoverCells.Count > 0;
            }

            // can flag all the leftover cells if the maximum posible mines in the front is equal too the maximum number of mines take away the number of leftover cells
            maxMinesOfAllFronts = FindSumOfMaxCount();
            if (maxMinesOfAllFronts == grid.GetMineCount() - leftoverCells.Count)
            {
                FlagLeftoverCells();

                changed = changed || leftoverCells.Count > 0;
            }

            //System.Diagnostics.Debug.WriteLine("done minecounting");
            //grid.DebugDisplayGrid();

            return changed;
        }

        private bool CheckForUnreasonableFrontiers()
        {
            foreach (Frontier frontier in frontiers)
            {
                if (frontier.Size > Program.FrontierSizeCutoff)
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

        private bool FlagLeftoverCells()
        {
            foreach (LogicCell cellToFlag in leftoverCells)
            {
                cellToFlag.Flag();
            }

            return leftoverCells.Count > 0;
        }
        private bool OpenLeftoverCells()
        {
            foreach (LogicCell cellToOpen in leftoverCells)
            {
                if (cellToOpen.IsHidden) cellToOpen.Open();
            }

            return leftoverCells.Count > 0;
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
                        !grid.GetCell(x, y).IsAdjacentToOpen())
                    {
                        cells.Add(grid.GetCell(x, y));
                    }
                }
            }
            return cells;
        }


        // methods used to find the forntiers
        private HashSet<Frontier> FindFrontiers(Grid grid)
        {
            // checked cells to avoid duplicate frontiers
            HashSet<LogicCell> checkedCells = new HashSet<LogicCell>();
            HashSet<Frontier> fronts = new HashSet<Frontier>();

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.GetCell(x, y).IsHidden &&
                        !grid.GetCell(x, y).IsFlagged &&
                        grid.GetCell(x, y).IsAdjacentToOpen() &&
                        !checkedCells.Contains(grid.GetCell(x, y)))
                    {
                        HashSet<LogicCell> frontCells = new HashSet<LogicCell>();
                        GetFrontHiddenCellsAt(grid.GetCell(x, y), frontCells, new HashSet<LogicCell>());

                        if (frontCells.Count > 0)
                        {
                            AddCellsToChecked(checkedCells, frontCells);

                            HashSet<LogicCell> frontierRevealedCells = GetFrontRevCells(frontCells);
                            HashSet<(int value, HashSet<int> adjacentIDs)> frontierRevCellsInfo = new HashSet<(int value, HashSet<int> adjacentIDs)>();

                            foreach (LogicCell cell in frontierRevealedCells)
                            {
                                frontierRevCellsInfo.Add((cell.EffectiveValue, GetAdjacentIDs(frontCells, cell)));
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
        private void AddCellsToChecked(HashSet<LogicCell> checkedCells, HashSet<LogicCell> cellsToAdd)
        {
            foreach (LogicCell cell in cellsToAdd)
            {
                checkedCells.Add(cell);
            }
        }
        private HashSet<int> GetAdjacentIDs(HashSet<LogicCell> frontCells, LogicCell revealedCells)
        {
            HashSet<int> ids = new HashSet<int>();

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
        private HashSet<LogicCell> GetFrontRevCells(HashSet<LogicCell> frontCells)
        {
            HashSet<LogicCell> frontierRevealedCells = new HashSet<LogicCell>();

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
        private void GetFrontHiddenCellsAt(LogicCell cell, HashSet<LogicCell> frontCells, HashSet<LogicCell> checkedCells)
        {
            checkedCells.Add(cell);
            frontCells.Add(cell);

            foreach (LogicCell adjacentCell in cell.AdjacentCells)
            {
                if (!checkedCells.Contains(adjacentCell))
                {
                    if (adjacentCell.IsHidden && !adjacentCell.IsFlagged)
                    {
                        if (adjacentCell.IsAdjacentToOpen())
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
        private void GetFrontHiddenCellsAtEdge(LogicCell cell, HashSet<LogicCell> frontCells, HashSet<LogicCell> checkedCells)
        {
            checkedCells.Add(cell);

            foreach (LogicCell adjacentCell in cell.AdjacentCells)
            {
                if (adjacentCell.IsHidden &&
                    !adjacentCell.IsFlagged &&
                    adjacentCell.IsAdjacentToOpen() &&
                    !checkedCells.Contains(adjacentCell))
                {
                    GetFrontHiddenCellsAt(adjacentCell, frontCells, checkedCells);
                }
            }
        }
    }
}
