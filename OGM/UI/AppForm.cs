using System;
using System.Windows.Forms;
using XTC.oelMVCS;

namespace OGM.UI
{
    public partial class AppForm : Form
    {
        public AppView view;
        public AppForm()
        {
            InitializeComponent();
        }

        private void cbLog_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            view.onLogCheckedChanged(cb.Checked);
        }
    }
}
