using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace OGM
{
    public partial class AppForm : Form
    {
        private Dictionary<string, TabPage> pages = new Dictionary<string, TabPage>();

        public AppForm()
        {
            this.FormClosed += this.onFormClosed;
            InitializeComponent();
            this.scApp.Panel2Collapsed = true;
        }

        public RichTextBox getLoggerUi()
        {
            return this.rtbLog;
        }

        public void AddPath(string _path, object _page)
        {
            ContainerControl page = _page as ContainerControl;
            //this.Controls.Add(panel);
            string[] sections = _path.Split("/");
            var nodes = this.tvPages.Nodes;
            foreach (string section in sections)
            {
                if (string.IsNullOrEmpty(section))
                    continue;
                var found = nodes.Find(section, false);
                if (found.Length == 0)
                {
                    TreeNode newNode = new TreeNode();
                    newNode.Name = section;
                    newNode.Text = section;
                    nodes.Add(newNode);
                    found = new TreeNode[] { newNode };
                }
                nodes = found[0].Nodes;
            }
            TabPage tabPage = new TabPage();
            tabPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            tabPage.Location = new System.Drawing.Point(10, 10);
            tabPage.Name = _path;
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(760, 660);
            tabPage.TabIndex = 0;
            tabPage.Text = _path;
            tabPage.UseVisualStyleBackColor = true;
            tabPage.Controls.Add(page);
            this.tcPages.Controls.Add(tabPage);
            this.pages[_path] = tabPage;
            this.tvPages.ExpandAll();
        }

        private void cbLog_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            this.scApp.Panel2Collapsed = !cb.Checked;
        }

        private void tvPages_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node.Nodes.Count > 0)
                return;
            string fullpath = "/" + node.FullPath;
            TabPage page;
            if (!this.pages.TryGetValue(fullpath, out page))
                return;
            this.tcPages.SelectedTab = page;
        }

        private void onFormClosed(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
