using System;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Minesweeper.AI
{
    public class LogicCell : Cell
    {
        public bool CanBeChecked
        {
            get
            {
                return canBeChecked;
            }
        }
        private bool canBeChecked;
        public List<LogicCell> AdjacentCells
        {
            get
            {
                return adjacentCells;
            }
        }
        private List<LogicCell> adjacentCells;
        public int EffectiveValue
        {
            get
            {
                return effectiveValue;
            }
        }
        private int effectiveValue;
        public int Id
        {
            get
            {
                return id;
            }
        }
        private int id;
        public int AdjacentHiddenCellsCount
        {
            get
            {
                return adjacentHiddenCellsCount;
            }
        }
        private int adjacentHiddenCellsCount;
        
        public LogicCell(int x, int y, bool isHidden, int boardWidth, int boardHeight, int id)
        : base(x, y, isHidden, boardWidth, boardHeight)
        {
            adjacentCells = new List<LogicCell>();
            adjacentHiddenCellsCount = 0;
            this.id = id;
        }
        public LogicCell(LogicCell cell, bool addAdjacent)
        {
            adjacentCells = new List<LogicCell>();

            if (addAdjacent)
            {
                foreach (LogicCell adjacentCell in cell.AdjacentCells)
                {
                    this.AdjacentCells.Add(new LogicCell(adjacentCell, false));
                }
            }

            this.effectiveValue = cell.EffectiveValue;
            this.isHidden = cell.IsHidden;
            this.isFlagged = cell.IsFlagged;
            this.id = cell.Id;
            this.adjacentHiddenCellsCount = cell.AdjacentHiddenCellsCount;
        }

        public int[] GetDirection(Grid grid, LogicCell cell, int x, int y)
        {
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    if (xOffset != 0 || yOffset != 0)
                    {
                        if (grid.GetCell(x + xOffset, y + yOffset) == cell)
                        {
                            int[] directions = { xOffset, yOffset };
                            return directions;
                        }
                    }
                }
            }

            return null;
        }
        public void SetValues()
        {
            effectiveValue = value;

            canBeChecked = !isHidden;
            if (canBeChecked) canBeChecked = value != 0;
        }
        public void AddAdjacentCell(LogicCell cell)
        {
            if (cell.IsMine && !this.isMine) this.value++;

            adjacentCells.Add(cell);
            if (cell.IsHidden)
            {
                adjacentHiddenCellsCount++;
            }
        }
        public override void Flag()
        {
            if (isFlagged)
            {
                System.Diagnostics.Debug.WriteLine("cell was flagged twice");
                return;
            }

            isFlagged = true;
            Complete();

            foreach (LogicCell cell in adjacentCells)
            {
                if (!cell.IsMine)
                {
                    cell.LowerEffectiveValue();
                }
            }
        }
        public void SetFlagTrue()
        {
            isFlagged = true;
        }
        private void LowerEffectiveValue()
        {
            effectiveValue--;
        }
        public override bool Open()
        {
            if (isMine)
            {
                System.Diagnostics.Debug.WriteLine("---------CLICKED ON A MINE------");
            }
            else if (!isHidden)
            {
                System.Diagnostics.Debug.WriteLine("OPENED NON HIDDEN CELL");
            }

            isHidden = false;
            canBeChecked = value != 0;
            foreach (LogicCell cell in adjacentCells)
            {
                cell.LowerAdjacentHiddenCellsCount();
            }

            if (value > 0) return false;

            foreach (LogicCell cell in adjacentCells)
            {
                if (cell.isHidden) cell.Open();
            }

            return false;
        }
        public void LowerAdjacentHiddenCellsCount()
        {
            adjacentHiddenCellsCount--;
        }
        public void Complete()
        {
            canBeChecked = false;
        }
        public override void SetMine()
        {
            isMine = true;

            foreach (LogicCell cell in adjacentCells)
            {
                if (!cell.IsMine) cell.value++;
            }
        }
    }
}