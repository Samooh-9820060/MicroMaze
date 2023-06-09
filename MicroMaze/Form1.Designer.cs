namespace MicroMaze
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
            this.panelSettings = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mazeHeight = new System.Windows.Forms.TextBox();
            this.mazeWidth = new System.Windows.Forms.TextBox();
            this.cboxMazeTypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNewMaze = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.panelMaze = new System.Windows.Forms.Panel();
            this.cboxSolveMethod = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panelSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSettings
            // 
            this.panelSettings.Controls.Add(this.cboxSolveMethod);
            this.panelSettings.Controls.Add(this.label4);
            this.panelSettings.Controls.Add(this.label3);
            this.panelSettings.Controls.Add(this.label2);
            this.panelSettings.Controls.Add(this.mazeHeight);
            this.panelSettings.Controls.Add(this.mazeWidth);
            this.panelSettings.Controls.Add(this.cboxMazeTypes);
            this.panelSettings.Controls.Add(this.label1);
            this.panelSettings.Controls.Add(this.btnNewMaze);
            this.panelSettings.Controls.Add(this.btnExit);
            this.panelSettings.Controls.Add(this.btnReset);
            this.panelSettings.Controls.Add(this.btnStart);
            this.panelSettings.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSettings.Location = new System.Drawing.Point(0, 0);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(271, 1033);
            this.panelSettings.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "Maze Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(72, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "X";
            // 
            // mazeHeight
            // 
            this.mazeHeight.Location = new System.Drawing.Point(93, 98);
            this.mazeHeight.Name = "mazeHeight";
            this.mazeHeight.Size = new System.Drawing.Size(54, 22);
            this.mazeHeight.TabIndex = 8;
            // 
            // mazeWidth
            // 
            this.mazeWidth.Location = new System.Drawing.Point(12, 98);
            this.mazeWidth.Name = "mazeWidth";
            this.mazeWidth.Size = new System.Drawing.Size(54, 22);
            this.mazeWidth.TabIndex = 7;
            // 
            // cboxMazeTypes
            // 
            this.cboxMazeTypes.FormattingEnabled = true;
            this.cboxMazeTypes.Location = new System.Drawing.Point(12, 41);
            this.cboxMazeTypes.Name = "cboxMazeTypes";
            this.cboxMazeTypes.Size = new System.Drawing.Size(168, 24);
            this.cboxMazeTypes.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Maze Type";
            // 
            // btnNewMaze
            // 
            this.btnNewMaze.Location = new System.Drawing.Point(12, 274);
            this.btnNewMaze.Name = "btnNewMaze";
            this.btnNewMaze.Size = new System.Drawing.Size(168, 35);
            this.btnNewMaze.TabIndex = 3;
            this.btnNewMaze.Text = "New Maze";
            this.btnNewMaze.UseVisualStyleBackColor = true;
            this.btnNewMaze.Click += new System.EventHandler(this.btnNewMaze_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(12, 315);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(168, 35);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(12, 233);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(168, 35);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 192);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(168, 35);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // panelMaze
            // 
            this.panelMaze.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMaze.Location = new System.Drawing.Point(271, 0);
            this.panelMaze.Name = "panelMaze";
            this.panelMaze.Size = new System.Drawing.Size(1631, 1033);
            this.panelMaze.TabIndex = 1;
            // 
            // cboxSolveMethod
            // 
            this.cboxSolveMethod.FormattingEnabled = true;
            this.cboxSolveMethod.Location = new System.Drawing.Point(12, 155);
            this.cboxSolveMethod.Name = "cboxSolveMethod";
            this.cboxSolveMethod.Size = new System.Drawing.Size(168, 24);
            this.cboxSolveMethod.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Solve Method";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.panelMaze);
            this.Controls.Add(this.panelSettings);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelSettings.ResumeLayout(false);
            this.panelSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSettings;
        private System.Windows.Forms.Panel panelMaze;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnNewMaze;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboxMazeTypes;
        private System.Windows.Forms.TextBox mazeHeight;
        private System.Windows.Forms.TextBox mazeWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboxSolveMethod;
        private System.Windows.Forms.Label label4;
    }
}

