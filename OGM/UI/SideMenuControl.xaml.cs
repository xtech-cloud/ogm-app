using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace OGM
{
    public partial class SideMenuControl
    {
        public class SideMenuUiBridge : ISideMenuUiBridge
        {
            public SideMenuControl control { get; set; }
           
        }

        public SideMenuFacade facade { get; set; }

        private List<TabItem> TabItems_ = new List<TabItem>();
        private List<TabItem> TabItems
        {
            get { return TabItems_; }
            set { TabItems_ = value; }
        }


        public SideMenuControl()
        {
            InitializeComponent();

            facade = App.Current.FindResource(SideMenuFacade.NAME) as SideMenuFacade;
            SideMenuUiBridge bridge = new SideMenuUiBridge();
            bridge.control = this;
            facade.setUiBridge(bridge);
        }
        
        private ICommand _switchItemCmd = null;
        public ICommand SwitchItemCmd
        {
            get
            {
                if (_switchItemCmd == null)
                {
                    _switchItemCmd = new RelayCommand(
                        this.SwitchItemCmd_CanExecute,
                        this.SwitchItemCmd_Execute);
                }
                return _switchItemCmd;
            }
        }

        private void SwitchItemCmd_Execute(FunctionEventArgs<object> _args)
        {
            string name = (_args.Info as SideMenuItem)?.Name;
            ISideMenuViewBridge bridge = facade.getViewBridge() as ISideMenuViewBridge;
            bridge.OnTabActivated(name);
            _args.Handled = true;
        }
        public bool SwitchItemCmd_CanExecute(FunctionEventArgs<object> _args)
        {
            return true;
        }
    }
}
