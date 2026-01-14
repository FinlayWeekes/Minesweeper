using System;

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


    protected Cell(int xCoor, int yCoor, bool isHidden, int boardWidth, int boardHeight)
    {
        this.isHidden = isHidden;
        this.isMine = false;
        this.value = 0;
        this.isFlagged = false;
    }
    protected Cell()
    {

    }

    public abstract bool Open();
    public abstract void Flag();
    public abstract void SetMine();
}