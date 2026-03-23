using System;
using System.Collections.Generic;

public abstract class Cell
{
    public int Value
    {
        get
        {
            return value;
        }
    }
    protected int value;
    public bool IsHidden
    {
        get
        {
            return isHidden;
        }
    }
    protected bool isHidden;
    public bool IsMine
    {
        get
        {
            return isMine;
        }
    }
    protected bool isMine;
    public bool IsFlagged
    {
        get
        {
            return isFlagged;
        }
    }
    protected bool isFlagged;
    public List<Cell> AdjacentCells
    {
        get
        {
            return adjacentCells;
        }
    }
    protected List<Cell> adjacentCells;
    
    protected Cell(bool isHidden)
    {
        this.adjacentCells = new List<Cell>();
        this.isHidden = isHidden;
        this.isMine = false;
        this.value = 0;
        this.isFlagged = false;
    }

    public abstract bool Open();
    public abstract void Flag();
    public abstract void SetMine();
    public abstract void AddAdjacentCell(Cell cell);
}