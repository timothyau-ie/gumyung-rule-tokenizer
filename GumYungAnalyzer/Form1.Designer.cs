namespace GumYungAnalyzer
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
            this.components = new System.ComponentModel.Container();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnFF = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnSkip = new System.Windows.Forms.Button();
            this.tbGraph = new GumYungAnalyzer.SyncTextBox();
            this.tbNames = new GumYungAnalyzer.SyncTextBox();
            this.lbNames = new System.Windows.Forms.ListBox();
            this.lbGraph = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(12, 54);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(220, 308);
            this.listBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(67, 45);
            this.button1.TabIndex = 1;
            this.button1.Text = "Next >";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnFF
            // 
            this.btnFF.Enabled = false;
            this.btnFF.Location = new System.Drawing.Point(85, 3);
            this.btnFF.Name = "btnFF";
            this.btnFF.Size = new System.Drawing.Size(43, 45);
            this.btnFF.TabIndex = 1;
            this.btnFF.Text = ">>";
            this.btnFF.UseVisualStyleBackColor = true;
            this.btnFF.Click += new System.EventHandler(this.btnFF_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(134, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(43, 45);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "||";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnSkip
            // 
            this.btnSkip.Enabled = false;
            this.btnSkip.Location = new System.Drawing.Point(183, 3);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(43, 45);
            this.btnSkip.TabIndex = 1;
            this.btnSkip.Text = ">|";
            this.btnSkip.UseVisualStyleBackColor = true;
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // tbGraph
            // 
            this.tbGraph.Buddy = this.tbNames;
            this.tbGraph.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbGraph.Location = new System.Drawing.Point(422, 275);
            this.tbGraph.Multiline = true;
            this.tbGraph.Name = "tbGraph";
            this.tbGraph.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbGraph.Size = new System.Drawing.Size(744, 87);
            this.tbGraph.TabIndex = 3;
            // 
            // tbNames
            // 
            this.tbNames.Buddy = this.tbGraph;
            this.tbNames.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNames.Location = new System.Drawing.Point(248, 275);
            this.tbNames.Multiline = true;
            this.tbNames.Name = "tbNames";
            this.tbNames.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbNames.Size = new System.Drawing.Size(168, 87);
            this.tbNames.TabIndex = 3;
            // 
            // lbNames
            // 
            this.lbNames.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNames.FormattingEnabled = true;
            this.lbNames.ItemHeight = 15;
            this.lbNames.Location = new System.Drawing.Point(248, 3);
            this.lbNames.Name = "lbNames";
            this.lbNames.Size = new System.Drawing.Size(168, 259);
            this.lbNames.TabIndex = 0;
            // 
            // lbGraph
            // 
            this.lbGraph.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGraph.FormattingEnabled = true;
            this.lbGraph.ItemHeight = 15;
            this.lbGraph.Location = new System.Drawing.Point(422, 3);
            this.lbGraph.Name = "lbGraph";
            this.lbGraph.Size = new System.Drawing.Size(744, 259);
            this.lbGraph.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 374);
            this.Controls.Add(this.tbGraph);
            this.Controls.Add(this.tbNames);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnSkip);
            this.Controls.Add(this.btnFF);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lbGraph);
            this.Controls.Add(this.lbNames);
            this.Controls.Add(this.listBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private SyncTextBox tbNames;
        private SyncTextBox tbGraph;
        private System.Windows.Forms.Button btnFF;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnSkip;
        private System.Windows.Forms.ListBox lbNames;
        private System.Windows.Forms.ListBox lbGraph;
    }
}