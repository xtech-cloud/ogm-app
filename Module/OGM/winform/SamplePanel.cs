using System;
using System.Windows.Forms;
using XTC.oelMVCS;

namespace OGM.Module.Sample
{
    public partial class SamplePanel : UserControl
    {
        public class SampleUiBridge : ISampleUiBridge
        {
            public SamplePanel panel { get; set; }

            public object getRootPanel()
            {
                return panel;
            }

            public void refreshErrorTip(string _tip)
            {
                panel.lTip.Text = _tip;
            }
        }

        public  SampleFacade facade { get; set; }

        public SamplePanel()
        {
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Location = new System.Drawing.Point(1, -3);
            this.Name = "panel1";
            this.Size = new System.Drawing.Size(1050, 801);
            this.TabIndex = 0;

            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            ISampleViewBridge bridge = facade.getViewBridge() as ISampleViewBridge;
            bridge.onLoginSubmit(tbUsername.Text, tbPassword.Text);
        }

    }
}
