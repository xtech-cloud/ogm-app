
using System;
using System.Windows;

namespace OGM
{
    /// <summary>
    ///     主内容
    /// </summary>
    public partial class MainContentControl
    {
        public class MainContentUiBridge : IMainContentUiBridge
        {
            public MainContentControl control { get; set; }

            public void SwitchPage(string _page)
            {
                var obj = AssemblyHelper.ResolveByKey(_page);
                var ctl = obj ?? AssemblyHelper.CreateInternalInstance($"UI.{_page}");
                if (control.SubContent is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                control.SubContent = ctl;
            }
        }

        public static readonly DependencyProperty SubContentProperty = DependencyProperty.Register("SubContent", typeof(object), typeof(MainContentControl));
        public object SubContent
        {
            get
            {
                return GetValue(MainContentControl.SubContentProperty);
            }
            set
            {
                SetValue(MainContentControl.SubContentProperty, value);
            }
        }
        

        public MainContentFacade facade { get; set; }

        public MainContentControl()
        {
            InitializeComponent();

            facade = App.Current.FindResource(MainContentFacade.NAME) as MainContentFacade;
            MainContentUiBridge bridge = new MainContentUiBridge();
            bridge.control = this;
            facade.setUiBridge(bridge);
        }

    }
}
