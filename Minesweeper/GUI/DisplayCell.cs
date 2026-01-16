using Minesweeper;
using Minesweeper.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

class DisplayCell : Cell
{
    public int Value
    {
        get 
        { 
            return value; 
        }
        set
        {
            this.value = value;
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
    {
        this.btn = Btn;
    }
    public DisplayCell(int xCoor, int yCoor, bool isHidden, int boardWidth, int boardHeight)
    : base()
    {

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