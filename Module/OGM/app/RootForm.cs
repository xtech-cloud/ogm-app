using System.Windows.Forms;

namespace app
{
     public class RootForm: Form
    {
        public RootForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void SetMainPanel(object _panel)
        {
            ContainerControl panel = _panel as ContainerControl;
            this.Controls.Add(panel);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RootForm
            // 
            this.ClientSize = new System.Drawing.Size(1052, 798);
            this.Name = "RootForm";
            this.ResumeLayout(false);

        }
    }

}
