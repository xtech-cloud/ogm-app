
namespace OGM.UI
{
    partial class AppForm
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
            this.scApp = new System.Windows.Forms.SplitContainer();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbLog = new System.Windows.Forms.CheckBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.scApp)).BeginInit();
            this.scApp.Panel1.SuspendLayout();
            this.scApp.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // scApp
            // 
            this.scApp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scApp.Location = new System.Drawing.Point(0, 0);
            this.scApp.Name = "scApp";
            this.scApp.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scApp.Panel1
            // 
            this.scApp.Panel1.Controls.Add(this.panel3);
            this.scApp.Panel1.Controls.Add(this.panel2);
            this.scApp.Panel1.Controls.Add(this.panel1);
            this.scApp.Size = new System.Drawing.Size(2053, 1121);
            this.scApp.SplitterDistance = 684;
            this.scApp.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Location = new System.Drawing.Point(422, 125);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1619, 545);
            this.panel3.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(422, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1619, 105);
            this.panel2.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbLog);
            this.panel1.Controls.Add(this.treeView1);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(412, 667);
            this.panel1.TabIndex = 0;
            // 
            // cbLog
            // 
            this.cbLog.AutoSize = true;
            this.cbLog.Location = new System.Drawing.Point(10, 634);
            this.cbLog.Name = "cbLog";
            this.cbLog.Size = new System.Drawing.Size(59, 19);
            this.cbLog.TabIndex = 1;
            this.cbLog.Text = "日志";
            this.cbLog.UseVisualStyleBackColor = true;
            this.cbLog.CheckedChanged += new System.EventHandler(this.cbLog_CheckedChanged);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(10, 22);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(388, 591);
            this.treeView1.TabIndex = 0;
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2053, 1121);
            this.Controls.Add(this.scApp);
            this.Name = "AppForm";
            this.Text = "AppForm";
            this.scApp.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scApp)).EndInit();
            this.scApp.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scApp;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox cbLog;
    }
}