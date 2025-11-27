using System;
using System.Collections.Generic;
using System.Windows.Forms;

class DisplayCell : Cell
{
    private Button btn;
    public Button Btn
    {
        get
        {
            return btn;
        }
    }
    public DisplayCell(Button Btn)
    : base()
    {
        this.btn = Btn;
    }

    public void IncreaseValue()
    {
        value++;
    }
    public override void Flag()
    {

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
    }
}
