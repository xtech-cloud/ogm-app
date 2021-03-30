using System;
using System.Windows.Forms;
using XTC.oelMVCS;

namespace OGM
{
    public class RootForm: Form
    {
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

        public void SetMainForm(Form _form)
        {
            this.Controls.Add(_form);
        }
    }

    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logger logger = new ConsoleLogger();
            Config config = new Config();
            Framework framework = new Framework();
            framework.setConfig(config);
            framework.setLogger(logger);
            framework.Initialize();
            AppView appView = new AppView();
            framework.getStaticPipe().RegisterView(AppView.NAME, appView);
            framework.Setup();

            ModuleManager mgr = new ModuleManager();
            mgr.Parse();
            RootForm rootForm = new RootForm();

            Application.Run(rootForm);

            framework.Dismantle();
            framework.Release();
        }
    }
}
