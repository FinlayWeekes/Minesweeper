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
        public int AdjacentHiddenNonFlagCellsCount
        {
            get
            {
                return adjacentHiddenCellsCount - value + effectiveValue;
            }
        }

        // used by the grid for generating cells
        public LogicCell(bool isHidden, int id)
        : base(isHidden)
        {
            adjacentHiddenCellsCount = 0;
            this.id = id;
        }
        public void SetValues()
        {
            effectiveValue = value;

            canBeChecked = !isHidden;
            if (canBeChecked) canBeChecked = value != 0;
        }
        public override void SetMine()
        {
            isMine = true;

            foreach (LogicCell cell in adjacentCells)
            {
                if (!cell.IsMine) cell.value++;
            }
        }
        public override void AddAdjacentCell(Cell cell)
        {
            if (cell.IsMine && !this.isMine) this.value++;

            adjacentCells.Add(cell);
            if (cell.IsHidden)
            {
                adjacentHiddenCellsCount++;
            }
        }

        // helper methods used by the Open and Flag methods
        private void LowerEffectiveValue()
        {
            effectiveValue--;
        }
        public void LowerAdjacentHiddenCellsCount()
        {
            adjacentHiddenCellsCount--;
        }

        // methods used by solver/AIs
        public override bool Open()
        {
            if (isMine)
            {
                System.Diagnostics.Debug.WriteLine("---------CLICKED ON A MINE---------");
            }
            else if (!isHidden)
            {
                System.Diagnostics.Debug.WriteLine("---------OPENED NON HIDDEN CELL---------");
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
        public override void Flag()
        {
            if (isFlagged)
            {
                System.Diagnostics.Debug.WriteLine("cell was flagged twice");
            }
            if (!isHidden)
            {
                System.Diagnostics.Debug.WriteLine("opened cell was flagged twice");
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
        public void Complete()
        {
            canBeChecked = false;
        }


        public bool IsAdjacentToOpen()
        {
            foreach (LogicCell adjacentCell in adjacentCells)
            {
                if (!adjacentCell.IsHidden) return true;
            }
            return false;
        }
        public bool IsAdjacentToClear()
        {
            foreach (LogicCell adjacentCell in adjacentCells)
            {
                if (adjacentCell.Value == 0 && !adjacentCell.IsMine) return true;
            }
            return false;
        }
    }
}