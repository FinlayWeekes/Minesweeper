using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.AI
{
    class Frontier
    {
        private struct HiddenCell
        {
            public bool IsFlagged;
            public HashSet<int> AdjacentRevCellIndexes;
            public HashSet<int> AdjacentFullCellIndexes;
        }
        private struct RevealedCell
        {
            public int EffectiveValue;
        }

        private RevealedCell[] revealedCells;
        private HiddenCell[] hiddenCells;

        private HashSet<int>[] posMinesForFlag;
        private HashSet<int>[] posMinesForEmpty;

        public HashSet<int> PosMineCounts;

        public int Size
        {
            get
            {
                return hiddenCellIDs.Length;
            }
        }

        public int MinMinesInFront
        { 
            get
            {
                return minMinesInFront;
            }
        }
        private int minMinesInFront;
        public int MaxMinesInFront
        {
            get
            {
                return maxMinesInFront;
            }
        }
        private int maxMinesInFront;

        private int maxMines;

        private int[] hiddenCellIDs;

        public Frontier(HashSet<(int value, HashSet<int> adjacentIDs)> frontierRevCells, int hiddenCellsCount, int maxMines)
        {
            this.maxMines = Math.Min(maxMines, hiddenCellsCount);

            // hidden cell IDs are used to match each revealed cell to thier adjacent hidden cells
            int indexesFound = 0;
            hiddenCellIDs = new int[hiddenCellsCount]; 
            for (int i = 0; i < hiddenCellIDs.Length; i++)
            {
                hiddenCellIDs[i] = -1;
            }

            // there is a value for each hidden cell based on its index
            // it will store the minimum number of mines required for the condition of the array to be true
            posMinesForFlag = new HashSet<int>[hiddenCellsCount];
            posMinesForEmpty = new HashSet<int>[hiddenCellsCount];
            for (int i = 0; i < hiddenCellsCount; i++)
            {
                posMinesForFlag[i] = new HashSet<int>();
                posMinesForEmpty[i] = new HashSet<int>();
            }

            // starts with the cells not being flagged
            hiddenCells = new HiddenCell[hiddenCellsCount];
            for (int i = 0; i < hiddenCellsCount; i++)
            {
                HiddenCell cell = new HiddenCell
                {
                    IsFlagged = false,
                    AdjacentRevCellIndexes = new HashSet<int>(),
                    AdjacentFullCellIndexes = new HashSet<int>()
                };

                hiddenCells[i] = cell;
            }

            // populates revealed cells to be the same as the frontierRevCells
            revealedCells = new RevealedCell[frontierRevCells.Count];
            int index = 0;
            foreach ((int value, HashSet<int> adjacentIDs) frontRevCell in frontierRevCells)
            {
                // updates the hidden cells adjacent revealed cells
                FillAdjacentCells(frontRevCell.adjacentIDs, hiddenCellIDs, index, ref indexesFound);

                RevealedCell cell = new RevealedCell
                {
                    EffectiveValue = frontRevCell.value
                };

                revealedCells[index] = cell;
                index++;
            }

            // fills the AdjacentFullCellIndexes list for each hidden cell which is a list of revealed cells which need to be completed by teh time it reaches this cel;
            // so that moving on from an impossible situation where a value is not fully met is not possible
            for (int i = 0; i < revealedCells.Length; i++)
            {
                int maxValue = -1;
                for (int hidIndex = 0; hidIndex < hiddenCells.Length; hidIndex++)
                {
                    if (hiddenCells[hidIndex].AdjacentRevCellIndexes.Contains(i)) maxValue = hidIndex;
                }

                hiddenCells[maxValue].AdjacentFullCellIndexes.Add(i);
            }

        }
        private void FillAdjacentCells(HashSet<int> adjacentIDs, int[] hiddenCellIDs, int revealedCellIndex, ref int indexesFound)
        {
            foreach (int id in adjacentIDs)
            {
                int adjacentHiddenCellIndex = Array.IndexOf(hiddenCellIDs, id);
                
                if (adjacentHiddenCellIndex == -1)
                {
                    hiddenCellIDs[indexesFound] = id;
                    adjacentHiddenCellIndex = indexesFound;
                    indexesFound++;
                }

                hiddenCells[adjacentHiddenCellIndex].AdjacentRevCellIndexes.Add(revealedCellIndex);
            }
        }
        
        
        // Methods used for the actual mine counting
        public void StartCounting()
        {
            PosMineCounts = LetCellBeEmpty(0, 0);
            PosMineCounts.UnionWith(LetCellBeFlagged(0, 0));

            maxMinesInFront = PosMineCounts.Max();
            minMinesInFront = PosMineCounts.Min();
        }
        private HashSet<int> LetCellBeEmpty(int flagsPlaced, int index)
        {
            // this set will represnt the number of mines in each valid arrangement where this cell is empty (no duplicates)
            HashSet<int> posNumberOfFlags = new HashSet<int>();

            // base case which is run when the end of the frontier has been reached
            if (index == hiddenCells.Length)
            {
                // only time when posNumberOfFlags is added to
                // because the mine count is only confirmed when the method reaches the end
                posNumberOfFlags.Add(flagsPlaced);
                return posNumberOfFlags;
            }

            // returns true if the current cell is the last cell adjacent to any cell which has an effective value greater than 0
            // prunes invalid configuarations
            if (SkipedNonCompletedCell(hiddenCells[index].AdjacentFullCellIndexes, revealedCells)) return posNumberOfFlags;

            // moves onto the next cell in the frontier
            int newIndex = index + 1;

            // calls itself and the flag method
            // basically tests the two possible situations where the next cell in the frontier is either contains a mine, or is empty
            // UnionWith is an efficient way of combining two has sets into one and removing duplicats at the same time
            posNumberOfFlags.UnionWith(LetCellBeEmpty(flagsPlaced, newIndex));
            posNumberOfFlags.UnionWith(LetCellBeFlagged(flagsPlaced, newIndex));

            // an array of hashsets the size of the number of hidden cells which keeps track of the number of mines in each possible arrangement where this cell is empty
            posMinesForEmpty[index].UnionWith(posNumberOfFlags);

            // the method can be simplified down to "find the possible number of mines in each valid arrangement where this cell is empty from the current state of the frontier"
            return posNumberOfFlags;
        }
        private HashSet<int> LetCellBeFlagged(int flagsPlaced, int index)
        {
            // the method is pretty similar to LetCellBeEmpty except it flags a cell in the process
            HashSet<int> posNumberOfFlags = new HashSet<int>();
            
            if (index == hiddenCells.Length)
            {
                posNumberOfFlags.Add(flagsPlaced);
                return posNumberOfFlags;
            }

            HashSet<int> revCellsVisited = new HashSet<int>();

            // FlagCell returns true if:
            // an adjacent revealed cell to this cell already has an effective value of 0
            // or if the max mine count has been reached
            // or if this is the last hidden cell in the frontier that an adjacent revealed cell is adjacent to has an effective value not equal to 0 after flagging (SkipedNonCompletedCell)
            if (FlagCell(ref flagsPlaced, index, revCellsVisited)) return posNumberOfFlags;

            int newIndex = index + 1;

            posNumberOfFlags.UnionWith(LetCellBeEmpty(flagsPlaced, newIndex));
            posNumberOfFlags.UnionWith(LetCellBeFlagged(flagsPlaced, newIndex));

            posMinesForFlag[index].UnionWith(posNumberOfFlags);
            
            // the cells are managed as an array of structs so they are passed by reference, not value
            // this means the cell needs to be unflagged before returning to take it back to its previous state
            UndoFlagCell(ref flagsPlaced, index, revCellsVisited);

            return posNumberOfFlags;
        }
        private void UndoFlagCell(ref int flagsPlaced, int index, HashSet<int> revCellsVisited)
        {
            hiddenCells[index].IsFlagged = false;
            flagsPlaced--;
            foreach (int cellIndex in revCellsVisited)
            {
                revealedCells[cellIndex].EffectiveValue++;
            }
        }
        private bool FlagCell(ref int flagsPlaced, int index, HashSet<int> revCellsVisited)
        {
            // returns true if that cell cant be flagged in this situation
            if (maxMines == flagsPlaced) return true;

            hiddenCells[index].IsFlagged = true;
            flagsPlaced++;

            foreach (int cellIndex in hiddenCells[index].AdjacentRevCellIndexes)
            {
                revealedCells[cellIndex].EffectiveValue--;
                revCellsVisited.Add(cellIndex);

                if (revealedCells[cellIndex].EffectiveValue == -1)
                {
                    UndoFlagCell(ref flagsPlaced, index, revCellsVisited);
                    return true;
                }
            }

            if  (SkipedNonCompletedCell(hiddenCells[index].AdjacentFullCellIndexes, revealedCells))
            {
                UndoFlagCell(ref flagsPlaced, index, revCellsVisited);
                return true;
            }

            return false;
        }
        private bool SkipedNonCompletedCell(HashSet<int> adjacentFullCellIndexes, RevealedCell[] revealedCells)
        {
            foreach (int fullCellIndex in adjacentFullCellIndexes)
            {
                if (revealedCells[fullCellIndex].EffectiveValue != 0) return true;
            }

            return false;
        }
        
        public void PruneWithNewPossibleValues()
        {
            maxMinesInFront = -1;
            minMinesInFront = maxMines + 1;
            foreach (HashSet<int> set in posMinesForEmpty)
            {
                // needs the extra hashset because removing items in a foreach loop of the set you are removing from does not work
                HashSet<int> valsToRemove = new HashSet<int>();

                foreach (int val in set)
                {
                    if (!PosMineCounts.Contains(val)) valsToRemove.Add(val);
                }

                foreach (int val in valsToRemove)
                {
                    set.Remove(val);
                }

                maxMinesInFront = Math.Max(maxMinesInFront, PosMineCounts.Max());
                minMinesInFront = Math.Min(minMinesInFront, PosMineCounts.Min());
            }
            foreach (HashSet<int> set in posMinesForFlag)
            {
                HashSet<int> valsToRemove = new HashSet<int>();

                foreach (int val in set)
                {
                    if (!PosMineCounts.Contains(val)) valsToRemove.Add(val);
                }

                foreach (int val in valsToRemove)
                {
                    set.Remove(val);
                }
            }
        }
            
        public bool ExcecuteFoundValues(Grid grid)
        {
            bool changed = CheckForOpenCells(grid);
            changed = CheckForFlaggedCells(grid) || changed;

            return changed;
        }
        public bool CheckForOpenCells(Grid grid)
        {
            bool changed = false;
            // opens all cells which cannot be flagged
            for (int i = 0; i < posMinesForFlag.Length; i++)
            {
                if (posMinesForFlag[i].Count == 0)
                {
                    changed = true;
                    LogicCell cellToOpen = grid.GetCellTup(IDToPoint(hiddenCellIDs[i], grid.Width, grid.Height));

                    if (cellToOpen.IsHidden) cellToOpen.Open();
                }
            }
            return changed;
        }
        public bool CheckForFlaggedCells(Grid grid)
        {
            bool changed = false;
            // opens all cells which cannot be opened
            for (int i = 0; i < posMinesForEmpty.Length; i++)
            {
                if (posMinesForEmpty[i].Count == 0)
                {
                    changed = true;
                    LogicCell cellToFlag = grid.GetCellTup(IDToPoint(hiddenCellIDs[i], grid.Width, grid.Height));

                    if (!cellToFlag.IsFlagged) cellToFlag.Flag();
                }
            }
            return changed;
        }

        public (int x, int y) IDToPoint(int id, int width, int height)
        {
            return (id % width, id / width);
        }

        public void DebugData()
        {
            for (int i = 0; i < posMinesForEmpty.Length; i++)
            {
                if (posMinesForEmpty[i].Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("cell " + hiddenCellIDs[i] + ": can be flagged");
                }
            }
            for (int i = 0; i < posMinesForEmpty.Length; i++)
            {
                if (posMinesForFlag[i].Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("cell " + hiddenCellIDs[i] + ": can be opened");
                }
            }
            System.Diagnostics.Debug.WriteLine("MINE AMOUNTS IN FRONTIER FOR EMPTY CELL");
            for (int i = 0; i < posMinesForEmpty.Length; i++)
            {
                if (posMinesForEmpty[i].Count != 0 && posMinesForFlag[i].Count != 0)
                {
                    System.Diagnostics.Debug.Write("cell " + hiddenCellIDs[i] + ": ");
                    foreach (int val in posMinesForEmpty[i])
                    {
                        System.Diagnostics.Debug.Write(val + ", ");
                    }
                    System.Diagnostics.Debug.WriteLine("");
                }
            }
            System.Diagnostics.Debug.WriteLine("MINE AMOUNTS IN FRONTIER FOR FLAGGED CELL");
            for (int i = 0; i < posMinesForFlag.Length; i++)
            {
                if (posMinesForEmpty[i].Count != 0 && posMinesForFlag[i].Count != 0)
                {
                    System.Diagnostics.Debug.Write("cell " + hiddenCellIDs[i] + ": ");
                    foreach (int val in posMinesForFlag[i])
                    {
                        System.Diagnostics.Debug.Write(val + ", ");
                    }
                    System.Diagnostics.Debug.WriteLine("");
                }
            }
        }
    }
}
