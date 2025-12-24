namespace Minesweeper
{
    partial class Stats
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
            this.Stats3BV = new System.Windows.Forms.Label();
            this.StatsTime = new System.Windows.Forms.Label();
            this.StatsClicks = new System.Windows.Forms.Label();
            this.StatsRate = new System.Windows.Forms.Label();
            this.StatsRPQ = new System.Windows.Forms.Label();
            this.StatsIOS = new System.Windows.Forms.Label();
            this.StatsSize = new System.Windows.Forms.Label();
            this.StatsDifficulty = new System.Windows.Forms.Label();
            this.StatsMineCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Stats3BV
            // 
            this.Stats3BV.AutoSize = true;
            this.Stats3BV.Location = new System.Drawing.Point(16, 57);
            this.Stats3BV.Name = "Stats3BV";
            this.Stats3BV.Size = new System.Drawing.Size(33, 13);
            this.Stats3BV.TabIndex = 0;
            this.Stats3BV.Text = "3BV: ";
            // 
            // StatsTime
            // 
            this.StatsTime.AutoSize = true;
            this.StatsTime.Location = new System.Drawing.Point(16, 72);
            this.StatsTime.Name = "StatsTime";
            this.StatsTime.Size = new System.Drawing.Size(36, 13);
            this.StatsTime.TabIndex = 1;
            this.StatsTime.Text = "Time: ";
            // 
            // StatsClicks
            // 
            this.StatsClicks.AutoSize = true;
            this.StatsClicks.Location = new System.Drawing.Point(16, 87);
            this.StatsClicks.Name = "StatsClicks";
            this.StatsClicks.Size = new System.Drawing.Size(41, 13);
            this.StatsClicks.TabIndex = 2;
            this.StatsClicks.Text = "Clicks: ";
            // 
            // StatsRate
            // 
            this.StatsRate.AutoSize = true;
            this.StatsRate.Location = new System.Drawing.Point(16, 102);
            this.StatsRate.Name = "StatsRate";
            this.StatsRate.Size = new System.Drawing.Size(43, 13);
            this.StatsRate.TabIndex = 3;
            this.StatsRate.Text = "3BV/s: ";
            // 
            // StatsRPQ
            // 
            this.StatsRPQ.AutoSize = true;
            this.StatsRPQ.Location = new System.Drawing.Point(16, 117);
            this.StatsRPQ.Name = "StatsRPQ";
            this.StatsRPQ.Size = new System.Drawing.Size(36, 13);
            this.StatsRPQ.TabIndex = 4;
            this.StatsRPQ.Text = "RPQ: ";
            // 
            // StatsIOS
            // 
            this.StatsIOS.AutoSize = true;
            this.StatsIOS.Location = new System.Drawing.Point(16, 132);
            this.StatsIOS.Name = "StatsIOS";
            this.StatsIOS.Size = new System.Drawing.Size(31, 13);
            this.StatsIOS.TabIndex = 5;
            this.StatsIOS.Text = "IOS: ";
            // 
            // StatsSize
            // 
            this.StatsSize.AutoSize = true;
            this.StatsSize.Location = new System.Drawing.Point(16, 27);
            this.StatsSize.Name = "StatsSize";
            this.StatsSize.Size = new System.Drawing.Size(33, 13);
            this.StatsSize.TabIndex = 6;
            this.StatsSize.Text = "Size: ";
            // 
            // StatsDifficulty
            // 
            this.StatsDifficulty.AutoSize = true;
            this.StatsDifficulty.Location = new System.Drawing.Point(16, 12);
            this.StatsDifficulty.Name = "StatsDifficulty";
            this.StatsDifficulty.Size = new System.Drawing.Size(53, 13);
            this.StatsDifficulty.TabIndex = 7;
            this.StatsDifficulty.Text = "Difficulty: ";
            // 
            // StatsMineCount
            // 
            this.StatsMineCount.AutoSize = true;
            this.StatsMineCount.Location = new System.Drawing.Point(16, 42);
            this.StatsMineCount.Name = "StatsMineCount";
            this.StatsMineCount.Size = new System.Drawing.Size(67, 13);
            this.StatsMineCount.TabIndex = 8;
            this.StatsMineCount.Text = "Mine Count: ";
            // 
            // Stats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(180, 161);
            this.Controls.Add(this.StatsMineCount);
            this.Controls.Add(this.StatsDifficulty);
            this.Controls.Add(this.StatsSize);
            this.Controls.Add(this.StatsIOS);
            this.Controls.Add(this.StatsRPQ);
            this.Controls.Add(this.StatsRate);
            this.Controls.Add(this.StatsClicks);
            this.Controls.Add(this.StatsTime);
            this.Controls.Add(this.Stats3BV);
            this.Name = "Stats";
            this.Text = "Stats";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Stats3BV;
        private System.Windows.Forms.Label StatsTime;
        private System.Windows.Forms.Label StatsClicks;
        private System.Windows.Forms.Label StatsRate;
        private System.Windows.Forms.Label StatsRPQ;
        private System.Windows.Forms.Label StatsIOS;
        private System.Windows.Forms.Label StatsSize;
        private System.Windows.Forms.Label StatsDifficulty;
        private System.Windows.Forms.Label StatsMineCount;
    }
}