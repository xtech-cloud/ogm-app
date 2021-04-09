
namespace OGM
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
            this.tcPages = new System.Windows.Forms.TabControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbLog = new System.Windows.Forms.CheckBox();
            this.tvPages = new System.Windows.Forms.TreeView();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.scApp)).BeginInit();
            this.scApp.Panel1.SuspendLayout();
            this.scApp.Panel2.SuspendLayout();
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
            this.scApp.Panel1.Controls.Add(this.tcPages);
            this.scApp.Panel1.Controls.Add(this.panel1);
            // 
            // scApp.Panel2
            // 
            this.scApp.Panel2.Controls.Add(this.rtbLog);
            this.scApp.Size = new System.Drawing.Size(1008, 729);
            this.scApp.SplitterDistance = 444;
            this.scApp.SplitterWidth = 5;
            this.scApp.TabIndex = 0;
            // 
            // tcPages
            // 
            this.tcPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcPages.ItemSize = new System.Drawing.Size(0, 1);
            this.tcPages.Location = new System.Drawing.Point(266, 12);
            this.tcPages.Name = "tcPages";
            this.tcPages.SelectedIndex = 0;
            this.tcPages.Size = new System.Drawing.Size(730, 393);
            this.tcPages.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tcPages.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.cbLog);
            this.panel1.Controls.Add(this.tvPages);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(257, 438);
            this.panel1.TabIndex = 0;
            // 
            // cbLog
            // 
            this.cbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbLog.AutoSize = true;
            this.cbLog.Location = new System.Drawing.Point(9, 408);
            this.cbLog.Name = "cbLog";
            this.cbLog.Size = new System.Drawing.Size(51, 21);
            this.cbLog.TabIndex = 2;
            this.cbLog.Text = "日志";
            this.cbLog.UseVisualStyleBackColor = true;
            this.cbLog.CheckedChanged += new System.EventHandler(this.cbLog_CheckedChanged);
            // 
            // tvPages
            // 
            this.tvPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvPages.Location = new System.Drawing.Point(9, 12);
            this.tvPages.Name = "tvPages";
            this.tvPages.PathSeparator = "/";
            this.tvPages.Size = new System.Drawing.Size(237, 390);
            this.tvPages.TabIndex = 0;
            this.tvPages.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvPages_AfterSelect);
            // 
            // rtbLog
            // 
            this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLog.Location = new System.Drawing.Point(4, 4);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(1001, 259);
            this.rtbLog.TabIndex = 0;
            this.rtbLog.Text = "";
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.scApp);
            this.Name = "AppForm";
            this.Text = "XTC聚合中台";
            this.scApp.Panel1.ResumeLayout(false);
            this.scApp.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scApp)).EndInit();
            this.scApp.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scApp;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TreeView tvPages;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.CheckBox cbLog;
        private System.Windows.Forms.TabControl tcPages;
    }
}