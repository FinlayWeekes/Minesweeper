using Minesweeper;
using Minesweeper.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

class DisplayCell : Cell
{
    public int betchelsGroup
    {
        get
        {
            return betchelsGroup; 
        }
        set
        {
            betchelsGroup = value; 
        }
    }
    private Button btn;
    public Button Btn
    {
        get
        {
            return btn;
        }
    }
    public DisplayCell(Button Btn)
    : base(true)
    {
        this.btn = Btn;
    }

    public override void Flag()
    {
        if (isFlagged)
        {
            btn.BackgroundImage = null;
        }
        else
        {
            btn.BackgroundImage = Resources.FlagIcon;
            // fills the size of the button
            btn.BackgroundImageLayout = ImageLayout.Stretch;
        }

        isFlagged = !isFlagged;
    }
    public override bool Open()
    {
        isHidden = false;
        return false;
    }
    public override void SetMine()
    {
        isMine = true;
        value = -1;
    }
    public override void AddAdjacentCell(Cell cell)
    {
        adjacentCells.Add(cell);
    }

    public void SetValue(int val)
    {
        this.value = val; 
    }
    public void ResetValues()
    {
        isMine = false;
        value = 0;
        isFlagged = false;
        isHidden = true;
        btn.Text = "";
        btn.Enabled = true;
        btn.ForeColor = Color.Black;
        btn.BackgroundImage = null;
    }
}