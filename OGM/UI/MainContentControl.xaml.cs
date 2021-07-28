
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

            public void SwitchPage(object _page)
            {
                control.SubContent = _page;
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
