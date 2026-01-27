using System;
using System.Collections.Generic;

public abstract class Cell
{
    protected int value;
    public int Value
    {
        get
        {
            return value;
        }
    }
    protected bool isHidden;
    public bool IsHidden
    {
        get
        {
            return isHidden;
        }
    }
    protected bool isMine;
    public bool IsMine
    {
        get
        {
            return isMine;
        }
    }
    protected bool isFlagged;
    public bool IsFlagged
    {
        get
        {
            return isFlagged;
        }
    }
    protected List<Cell> adjacentCells;
    public List<Cell> AdjacentCells
    {
        get
        {
            return adjacentCells;
        }
    }

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