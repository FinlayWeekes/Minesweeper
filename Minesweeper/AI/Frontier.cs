using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.AI
{
    struct HiddenCell
    {
        public bool IsFlagged;
        public List<int> AdjacentRevCellIndexes;
        public List<int> AdjacentFullCellIndexes;
    }
    struct RevealedCell
    {
        public int EffectiveValue;
    }
    class Frontier
    {
        public RevealedCell[] revealedCells;
        public HiddenCell[] hiddenCells;

        private int[] minMinesForFlag;
        private int[] minMinesForEmpty;

        private int MaxMines;
        private int HiddenCellsCount;

        private int[] hiddenCellIDs;

        private int MinMines;

        public Frontier((int value, List<int> adjacentIDs)[] frontierRevCells, int hiddenMinesCount, int maxMines)
        {
            this.MaxMines = maxMines;
            this.HiddenCellsCount = hiddenMinesCount;

            // hidden cell IDs are used to match each revealed cell to thier adjacent hidden cells
            int indexesFound = 0;
            hiddenCellIDs = new int[hiddenMinesCount]; 
            for (int i = 0; i < hiddenCellIDs.Length; i++)
            {
                hiddenCellIDs[i] = -1;
            }

            // there is a value for each hidden cell based on its index
            // it will store the minimum number of mines required for the condition of the array to be true
            minMinesForFlag = new int[hiddenMinesCount];
            minMinesForEmpty = new int[hiddenMinesCount];
            for (int i = 0; i < hiddenMinesCount; i++)
            { 
                minMinesForFlag[i] = maxMines + 1;
                minMinesForEmpty[i] = maxMines + 1;
            }

            // starts with the cells not being flagged
            hiddenCells = new HiddenCell[HiddenCellsCount];
            for (int i = 0; i < HiddenCellsCount; i++)
            {
                HiddenCell cell = new HiddenCell
                {
                    IsFlagged = false,
                    AdjacentRevCellIndexes = new List<int>(),
                    AdjacentFullCellIndexes = new List<int>()
                };

                hiddenCells[i] = cell;
            }

            // populates revealed cells to be the same as the frontierRevCells
            revealedCells = new RevealedCell[frontierRevCells.Length];
            for (int i = 0; i < frontierRevCells.Length; i++)
            {
                FillAdjacentCells(frontierRevCells[i].adjacentIDs, hiddenCellIDs, i, ref indexesFound);

                RevealedCell cell = new RevealedCell
                {
                    EffectiveValue = frontierRevCells[i].value
                };

                revealedCells[i] = cell;
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
        private void FillAdjacentCells(List<int> adjacentIDs, int[] hiddenCellIDs, int revealedCellIndex, ref int indexesFound)
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
        public bool StartCounting()
        {
            LetCellBeEmpty(0, 0);
            LetCellBeFlagged(0, 0);

            for (int i = 0; i < minMinesForEmpty.Length; i++)
            {
                if (minMinesForEmpty[i] == MaxMines + 1)
                {
                    System.Diagnostics.Debug.WriteLine("cell " + hiddenCellIDs[i] + " can be flagged");
                }
            }
            for (int i = 0; i < minMinesForFlag.Length; i++)
            {
                if (minMinesForFlag[i] == MaxMines + 1)
                {
                    System.Diagnostics.Debug.WriteLine("cell " + hiddenCellIDs[i] + " can be opened");
                }
            }
            System.Diagnostics.Debug.WriteLine("MIN MINES FOR EMPTY");
            for (int i = 0; i < minMinesForEmpty.Length; i++)
            {
                if (minMinesForEmpty[i] != MaxMines + 1 && minMinesForFlag[i] != MaxMines + 1)
                {
                    System.Diagnostics.Debug.WriteLine("cell " + hiddenCellIDs[i] + ": " + minMinesForEmpty[i]);
                }
            }
            System.Diagnostics.Debug.WriteLine("MIN MINES FOR FLAG");
            for (int i = 0; i < minMinesForFlag.Length; i++)
            {
                if (minMinesForEmpty[i] != MaxMines + 1 && minMinesForFlag[i] != MaxMines + 1)
                {
                    System.Diagnostics.Debug.WriteLine("cell " + hiddenCellIDs[i] + ": " + minMinesForFlag[i]);
                }
            }

            return false;
        }
        private int LetCellBeEmpty(int flagsPlaced, int index)
        {
            if (index == hiddenCells.Length) return flagsPlaced;

            if (SkipedNonCompletedCell(hiddenCells[index].AdjacentFullCellIndexes, revealedCells)) return MaxMines + 1;

            int newIndex = index + 1;

            int minMines = Math.Min(LetCellBeEmpty(flagsPlaced, newIndex),
                                    LetCellBeFlagged(flagsPlaced, newIndex));

            minMinesForEmpty[index] = Math.Min(minMinesForEmpty[index], minMines);

            return minMines;
        }
        private int LetCellBeFlagged(int flagsPlaced, int index)
        {
            if (index == hiddenCells.Length) return flagsPlaced;

            List<int> revCellsVisited = new List<int>();
            if (FlagCell(ref flagsPlaced, index, revCellsVisited)) return MaxMines + 1;

            int newIndex = index + 1;

            int minMines = Math.Min(LetCellBeEmpty(flagsPlaced, newIndex),
                                    LetCellBeFlagged(flagsPlaced, newIndex));

            minMinesForFlag[index] = Math.Min(minMinesForFlag[index], minMines);
            
            UndoFlagCell(ref flagsPlaced, index, revCellsVisited);

            return minMines;
        }
        private void UndoFlagCell(ref int flagsPlaced, int index, List<int> revCellsVisited)
        {
            hiddenCells[index].IsFlagged = false;
            flagsPlaced--;
            foreach (int cellIndex in revCellsVisited)
            {
                revealedCells[cellIndex].EffectiveValue++;
            }
        }
        private bool FlagCell(ref int flagsPlaced, int index, List<int> revCellsVisited)
        {
            // returns true if that cell cant be flagged in this situation
            if (MaxMines == flagsPlaced) return true;

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

            return SkipedNonCompletedCell(hiddenCells[index].AdjacentFullCellIndexes, revealedCells);
        }
        private bool SkipedNonCompletedCell(List<int> adjacentFullCellIndexes, RevealedCell[] revealedCells)
        {
            foreach (int fullCellIndex in adjacentFullCellIndexes)
            {
                if (revealedCells[fullCellIndex].EffectiveValue != 0) return true;
            }

            return false;
        }
    }
}
