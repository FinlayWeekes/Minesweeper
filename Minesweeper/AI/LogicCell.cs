using System;
using System.Data;
using System.Collections.Generic;
using System.IO;

class LogicCell : Cell
{
    private bool canBeChecked;
    public bool CanBeChecked
    {
        get
        {
            return canBeChecked;
        }
    }
    private List<LogicCell> adjacentCells;
    public List<LogicCell> AdjacentCells
    {
        get
        {
            return adjacentCells;
        }
    }
    private int effectiveValue;
    public int EffectiveValue
    {
        get
        {
            return effectiveValue;
        }
    }
    private int id;
    public int Id
    {
        get
        {
            return id;
        }
    }
    private int adjacentHiddenCellsCount;
    public int AdjacentHiddenCellsCount
    {
        get
        {
            return adjacentHiddenCellsCount;
        }
    }
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
                    if (grid.GetLogicCell(x + xOffset, y + yOffset) == cell)
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
    public void LowerEfectiveValue()
    {
        effectiveValue--;
    }
    private void LowerEffectiveValue()
    {
        effectiveValue--;
    }
    public override bool Open()
    {
        if (isMine)
        {
            Console.WriteLine("clicked on mine");
            Console.ReadLine();
        }

        isHidden = false;
        canBeChecked = value != 0;
        foreach (LogicCell cell in adjacentCells)
        {
            cell.LowerAdjacentHiddenCellsCount();
        }

        if (value > 0) return false;
        if (isMine) return true;

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