namespace Minesweeper.GUI
{
    partial class CustomForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomForm));
            this.title = new System.Windows.Forms.Label();
            this.heightTextBox = new System.Windows.Forms.TextBox();
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.mineCountTextBox = new System.Windows.Forms.TextBox();
            this.boardPropertiesLabel = new System.Windows.Forms.Label();
            this.generateButton = new System.Windows.Forms.Button();
            this.loadLabel = new System.Windows.Forms.Label();
            this.difTextBox = new System.Windows.Forms.TextBox();
            this.loadButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.beginnerPatList = new System.Windows.Forms.ListView();
            this.checkIcons = new System.Windows.Forms.ImageList(this.components);
            this.intermediatePatList = new System.Windows.Forms.ListView();
            this.amateurPatList = new System.Windows.Forms.ListView();
            this.expertPatList = new System.Windows.Forms.ListView();
            this.masterPatList = new System.Windows.Forms.ListView();
            this.beginnerLabel = new System.Windows.Forms.Label();
            this.amateurLabel = new System.Windows.Forms.Label();
            this.intermediateLabel = new System.Windows.Forms.Label();
            this.expertLabel = new System.Windows.Forms.Label();
            this.masterLabel = new System.Windows.Forms.Label();
            this.debugLabel = new System.Windows.Forms.Label();
            this.menuButton = new System.Windows.Forms.Button();
            this.mineDensityTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // title
            // 
            this.title.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Impact", 48F);
            this.title.Location = new System.Drawing.Point(98, 9);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(617, 80);
            this.title.TabIndex = 0;
            this.title.Text = "CUSTOM MINESWEEPER";
            // 
            // heightTextBox
            // 
            this.heightTextBox.Location = new System.Drawing.Point(191, 93);
            this.heightTextBox.Name = "heightTextBox";
            this.heightTextBox.Size = new System.Drawing.Size(100, 20);
            this.heightTextBox.TabIndex = 1;
            // 
            // widthTextBox
            // 
            this.widthTextBox.Location = new System.Drawing.Point(191, 119);
            this.widthTextBox.Name = "widthTextBox";
            this.widthTextBox.Size = new System.Drawing.Size(100, 20);
            this.widthTextBox.TabIndex = 2;
            // 
            // mineCountTextBox
            // 
            this.mineCountTextBox.Location = new System.Drawing.Point(191, 142);
            this.mineCountTextBox.Name = "mineCountTextBox";
            this.mineCountTextBox.Size = new System.Drawing.Size(100, 20);
            this.mineCountTextBox.TabIndex = 3;
            // 
            // boardPropertiesLabel
            // 
            this.boardPropertiesLabel.AutoSize = true;
            this.boardPropertiesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.boardPropertiesLabel.Location = new System.Drawing.Point(12, 92);
            this.boardPropertiesLabel.Name = "boardPropertiesLabel";
            this.boardPropertiesLabel.Size = new System.Drawing.Size(173, 96);
            this.boardPropertiesLabel.TabIndex = 4;
            this.boardPropertiesLabel.Text = "Enter Height:\r\nEnter Width:\r\nEnter Mine Count:\r\nEnter Mine Density:";
            this.boardPropertiesLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // generateButton
            // 
            this.generateButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.generateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generateButton.Location = new System.Drawing.Point(328, 88);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(151, 43);
            this.generateButton.TabIndex = 7;
            this.generateButton.Text = "Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // loadLabel
            // 
            this.loadLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loadLabel.AutoSize = true;
            this.loadLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadLabel.Location = new System.Drawing.Point(548, 88);
            this.loadLabel.Name = "loadLabel";
            this.loadLabel.Size = new System.Drawing.Size(208, 24);
            this.loadLabel.TabIndex = 8;
            this.loadLabel.Text = "Enter Name of Difficulty:\r\n";
            this.loadLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // difTextBox
            // 
            this.difTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.difTextBox.Location = new System.Drawing.Point(593, 114);
            this.difTextBox.Name = "difTextBox";
            this.difTextBox.Size = new System.Drawing.Size(127, 20);
            this.difTextBox.TabIndex = 9;
            // 
            // loadButton
            // 
            this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loadButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadButton.Location = new System.Drawing.Point(556, 137);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(76, 27);
            this.loadButton.TabIndex = 10;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Location = new System.Drawing.Point(675, 136);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(76, 27);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // beginnerPatList
            // 
            this.beginnerPatList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.beginnerPatList.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.beginnerPatList.FullRowSelect = true;
            this.beginnerPatList.HideSelection = false;
            this.beginnerPatList.Location = new System.Drawing.Point(29, 222);
            this.beginnerPatList.Name = "beginnerPatList";
            this.beginnerPatList.Size = new System.Drawing.Size(121, 216);
            this.beginnerPatList.StateImageList = this.checkIcons;
            this.beginnerPatList.TabIndex = 12;
            this.beginnerPatList.UseCompatibleStateImageBehavior = false;
            this.beginnerPatList.View = System.Windows.Forms.View.List;
            // 
            // checkIcons
            // 
            this.checkIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("checkIcons.ImageStream")));
            this.checkIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.checkIcons.Images.SetKeyName(0, "square-image.png");
            this.checkIcons.Images.SetKeyName(1, "square-image (1).png");
            this.checkIcons.Images.SetKeyName(2, "square-image (2).png");
            // 
            // intermediatePatList
            // 
            this.intermediatePatList.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.intermediatePatList.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.intermediatePatList.FullRowSelect = true;
            this.intermediatePatList.HideSelection = false;
            this.intermediatePatList.Location = new System.Drawing.Point(342, 222);
            this.intermediatePatList.Name = "intermediatePatList";
            this.intermediatePatList.Size = new System.Drawing.Size(121, 216);
            this.intermediatePatList.StateImageList = this.checkIcons;
            this.intermediatePatList.TabIndex = 13;
            this.intermediatePatList.UseCompatibleStateImageBehavior = false;
            this.intermediatePatList.View = System.Windows.Forms.View.List;
            // 
            // amateurPatList
            // 
            this.amateurPatList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.amateurPatList.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.amateurPatList.FullRowSelect = true;
            this.amateurPatList.HideSelection = false;
            this.amateurPatList.Location = new System.Drawing.Point(187, 222);
            this.amateurPatList.Name = "amateurPatList";
            this.amateurPatList.Size = new System.Drawing.Size(121, 216);
            this.amateurPatList.StateImageList = this.checkIcons;
            this.amateurPatList.TabIndex = 14;
            this.amateurPatList.UseCompatibleStateImageBehavior = false;
            this.amateurPatList.View = System.Windows.Forms.View.List;
            // 
            // expertPatList
            // 
            this.expertPatList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.expertPatList.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.expertPatList.FullRowSelect = true;
            this.expertPatList.HideSelection = false;
            this.expertPatList.Location = new System.Drawing.Point(506, 222);
            this.expertPatList.Name = "expertPatList";
            this.expertPatList.Size = new System.Drawing.Size(121, 216);
            this.expertPatList.StateImageList = this.checkIcons;
            this.expertPatList.TabIndex = 15;
            this.expertPatList.UseCompatibleStateImageBehavior = false;
            this.expertPatList.View = System.Windows.Forms.View.List;
            // 
            // masterPatList
            // 
            this.masterPatList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.masterPatList.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.masterPatList.FullRowSelect = true;
            this.masterPatList.HideSelection = false;
            this.masterPatList.Location = new System.Drawing.Point(663, 222);
            this.masterPatList.Name = "masterPatList";
            this.masterPatList.Size = new System.Drawing.Size(121, 216);
            this.masterPatList.StateImageList = this.checkIcons;
            this.masterPatList.TabIndex = 16;
            this.masterPatList.UseCompatibleStateImageBehavior = false;
            this.masterPatList.View = System.Windows.Forms.View.List;
            // 
            // beginnerLabel
            // 
            this.beginnerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.beginnerLabel.AutoSize = true;
            this.beginnerLabel.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.beginnerLabel.Location = new System.Drawing.Point(26, 183);
            this.beginnerLabel.Name = "beginnerLabel";
            this.beginnerLabel.Size = new System.Drawing.Size(127, 36);
            this.beginnerLabel.TabIndex = 18;
            this.beginnerLabel.Text = "BEGINNER";
            // 
            // amateurLabel
            // 
            this.amateurLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.amateurLabel.AutoSize = true;
            this.amateurLabel.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.amateurLabel.Location = new System.Drawing.Point(188, 183);
            this.amateurLabel.Name = "amateurLabel";
            this.amateurLabel.Size = new System.Drawing.Size(119, 36);
            this.amateurLabel.TabIndex = 19;
            this.amateurLabel.Text = "AMATEUR";
            // 
            // intermediateLabel
            // 
            this.intermediateLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.intermediateLabel.AutoSize = true;
            this.intermediateLabel.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.intermediateLabel.Location = new System.Drawing.Point(322, 183);
            this.intermediateLabel.Name = "intermediateLabel";
            this.intermediateLabel.Size = new System.Drawing.Size(174, 36);
            this.intermediateLabel.TabIndex = 20;
            this.intermediateLabel.Text = "INTERMEDIATE";
            // 
            // expertLabel
            // 
            this.expertLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.expertLabel.AutoSize = true;
            this.expertLabel.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.expertLabel.Location = new System.Drawing.Point(519, 183);
            this.expertLabel.Name = "expertLabel";
            this.expertLabel.Size = new System.Drawing.Size(97, 36);
            this.expertLabel.TabIndex = 21;
            this.expertLabel.Text = "EXPERT";
            // 
            // masterLabel
            // 
            this.masterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.masterLabel.AutoSize = true;
            this.masterLabel.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.masterLabel.Location = new System.Drawing.Point(672, 183);
            this.masterLabel.Name = "masterLabel";
            this.masterLabel.Size = new System.Drawing.Size(106, 36);
            this.masterLabel.TabIndex = 22;
            this.masterLabel.Text = "MASTER";
            // 
            // debugLabel
            // 
            this.debugLabel.AutoSize = true;
            this.debugLabel.ForeColor = System.Drawing.Color.Red;
            this.debugLabel.Location = new System.Drawing.Point(325, 151);
            this.debugLabel.Name = "debugLabel";
            this.debugLabel.Size = new System.Drawing.Size(0, 13);
            this.debugLabel.TabIndex = 23;
            // 
            // menuButton
            // 
            this.menuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.menuButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.menuButton.Location = new System.Drawing.Point(731, 22);
            this.menuButton.Name = "menuButton";
            this.menuButton.Size = new System.Drawing.Size(47, 23);
            this.menuButton.TabIndex = 24;
            this.menuButton.Text = "Menu";
            this.menuButton.UseVisualStyleBackColor = true;
            this.menuButton.Click += new System.EventHandler(this.menuButton_Click);
            // 
            // mineDensityTextBox
            // 
            this.mineDensityTextBox.Location = new System.Drawing.Point(191, 168);
            this.mineDensityTextBox.Name = "mineDensityTextBox";
            this.mineDensityTextBox.Size = new System.Drawing.Size(100, 20);
            this.mineDensityTextBox.TabIndex = 25;
            // 
            // CustomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mineDensityTextBox);
            this.Controls.Add(this.menuButton);
            this.Controls.Add(this.debugLabel);
            this.Controls.Add(this.masterLabel);
            this.Controls.Add(this.expertLabel);
            this.Controls.Add(this.intermediateLabel);
            this.Controls.Add(this.amateurLabel);
            this.Controls.Add(this.beginnerLabel);
            this.Controls.Add(this.masterPatList);
            this.Controls.Add(this.expertPatList);
            this.Controls.Add(this.amateurPatList);
            this.Controls.Add(this.intermediatePatList);
            this.Controls.Add(this.beginnerPatList);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.difTextBox);
            this.Controls.Add(this.loadLabel);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.boardPropertiesLabel);
            this.Controls.Add(this.mineCountTextBox);
            this.Controls.Add(this.widthTextBox);
            this.Controls.Add(this.heightTextBox);
            this.Controls.Add(this.title);
            this.Name = "CustomForm";
            this.Text = "CustomForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label title;
        private System.Windows.Forms.TextBox heightTextBox;
        private System.Windows.Forms.TextBox widthTextBox;
        private System.Windows.Forms.TextBox mineCountTextBox;
        private System.Windows.Forms.Label boardPropertiesLabel;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.Label loadLabel;
        private System.Windows.Forms.TextBox difTextBox;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ListView beginnerPatList;
        private System.Windows.Forms.ImageList checkIcons;
        private System.Windows.Forms.ListView intermediatePatList;
        private System.Windows.Forms.ListView amateurPatList;
        private System.Windows.Forms.ListView expertPatList;
        private System.Windows.Forms.ListView masterPatList;
        private System.Windows.Forms.Label beginnerLabel;
        private System.Windows.Forms.Label amateurLabel;
        private System.Windows.Forms.Label intermediateLabel;
        private System.Windows.Forms.Label expertLabel;
        private System.Windows.Forms.Label masterLabel;
        private System.Windows.Forms.Label debugLabel;
        private System.Windows.Forms.Button menuButton;
        private System.Windows.Forms.TextBox mineDensityTextBox;
    }
}