using System;

abstract class Cell
{
    protected bool isOnEdge;
    public bool IsOnEdge
    {
        get
        {
            return isOnEdge;
        }
    }
    protected bool isOnCorner;
    public bool IsOnCorner
    {
        get
        {
            return isOnCorner;
        }
    }
    protected int adjacentEdge;
    public int AdjacentEdge
    {
        get
        {
            return adjacentEdge;
        }
    }
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

        // sets adjacent edge which are 1 for top edge, 2 for right edge, 3 for bottom edge, and 4 for left edge
        // the corner edge is the number starting from the top left corner matching the top edge
        if (xCoor == 0)
        {
            this.isOnEdge = true;
            this.adjacentEdge = 4;

            if (yCoor == 0)
            {
                this.adjacentEdge = 1;
                this.isOnCorner = true;
            }
            else if (yCoor == boardHeight - 1)
            {
                this.isOnCorner = true;
            }
        }
        else if (xCoor == boardWidth - 1)
        {
            this.isOnEdge = true;
            this.adjacentEdge = 2;

            if (yCoor == 0)
            {
                isOnCorner = true;
            }
            else if (yCoor == boardHeight - 1)
            {
                adjacentEdge = 3;
                isOnCorner = true;
            }
        }
        else if (yCoor == 0)
        {
            this.isOnEdge = true;
            this.adjacentEdge = 1;
        }
        else if (yCoor == boardHeight - 1)
        {
            this.isOnEdge = true;
            this.adjacentEdge = 3;
        }
    }
    protected Cell()
    {

    }

    public abstract bool Open();
    public abstract void Flag();
    public abstract void SetMine();
}