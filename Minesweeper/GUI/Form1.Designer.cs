namespace Minesweeper
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.beginnerButton = new System.Windows.Forms.Button();
            this.customButton = new System.Windows.Forms.Button();
            this.amateurButton = new System.Windows.Forms.Button();
            this.intermediateButton = new System.Windows.Forms.Button();
            this.expertButton = new System.Windows.Forms.Button();
            this.buttonMaster = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Impact", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(107, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(547, 117);
            this.label1.TabIndex = 0;
            this.label1.Text = "MINESWEPER";
            // 
            // beginnerButton
            // 
            this.beginnerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.beginnerButton.Location = new System.Drawing.Point(28, 165);
            this.beginnerButton.Name = "beginnerButton";
            this.beginnerButton.Size = new System.Drawing.Size(126, 55);
            this.beginnerButton.TabIndex = 1;
            this.beginnerButton.Text = "Beginner";
            this.beginnerButton.UseVisualStyleBackColor = true;
            this.beginnerButton.Click += new System.EventHandler(this.beginnerButton_Click);
            // 
            // customButton
            // 
            this.customButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customButton.Location = new System.Drawing.Point(325, 240);
            this.customButton.Name = "customButton";
            this.customButton.Size = new System.Drawing.Size(125, 52);
            this.customButton.TabIndex = 2;
            this.customButton.Text = "Custom";
            this.customButton.UseVisualStyleBackColor = true;
            this.customButton.Click += new System.EventHandler(this.customButton_Click);
            // 
            // amateurButton
            // 
            this.amateurButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.amateurButton.Location = new System.Drawing.Point(181, 165);
            this.amateurButton.Name = "amateurButton";
            this.amateurButton.Size = new System.Drawing.Size(126, 55);
            this.amateurButton.TabIndex = 4;
            this.amateurButton.Text = "Amateur";
            this.amateurButton.UseVisualStyleBackColor = true;
            this.amateurButton.Click += new System.EventHandler(this.amateurButton_Click);
            // 
            // intermediateButton
            // 
            this.intermediateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.intermediateButton.Location = new System.Drawing.Point(324, 165);
            this.intermediateButton.Name = "intermediateButton";
            this.intermediateButton.Size = new System.Drawing.Size(126, 55);
            this.intermediateButton.TabIndex = 5;
            this.intermediateButton.Text = "Intermediate";
            this.intermediateButton.UseVisualStyleBackColor = true;
            this.intermediateButton.Click += new System.EventHandler(this.intermediateButton_Click);
            // 
            // expertButton
            // 
            this.expertButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.expertButton.Location = new System.Drawing.Point(477, 165);
            this.expertButton.Name = "expertButton";
            this.expertButton.Size = new System.Drawing.Size(126, 55);
            this.expertButton.TabIndex = 6;
            this.expertButton.Text = "Expert";
            this.expertButton.UseVisualStyleBackColor = true;
            this.expertButton.Click += new System.EventHandler(this.expertButton_Click);
            // 
            // buttonMaster
            // 
            this.buttonMaster.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMaster.Location = new System.Drawing.Point(632, 165);
            this.buttonMaster.Name = "buttonMaster";
            this.buttonMaster.Size = new System.Drawing.Size(126, 55);
            this.buttonMaster.TabIndex = 7;
            this.buttonMaster.Text = "Master";
            this.buttonMaster.UseVisualStyleBackColor = true;
            this.buttonMaster.Click += new System.EventHandler(this.buttonMaster_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 450);
            this.Controls.Add(this.buttonMaster);
            this.Controls.Add(this.expertButton);
            this.Controls.Add(this.intermediateButton);
            this.Controls.Add(this.amateurButton);
            this.Controls.Add(this.customButton);
            this.Controls.Add(this.beginnerButton);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button beginnerButton;
        private System.Windows.Forms.Button customButton;
        private System.Windows.Forms.Button amateurButton;
        private System.Windows.Forms.Button intermediateButton;
        private System.Windows.Forms.Button expertButton;
        private System.Windows.Forms.Button buttonMaster;
    }
}

