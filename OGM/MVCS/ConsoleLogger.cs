using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using XTC.oelMVCS;

namespace OGM
{
    public class ConsoleLogger : Logger
    {
        public delegate void AppendDelegate(string _message, Color _color);
        public AppendDelegate appendDelegate;

        private List<KeyValuePair<string, Color>> cache = new List<KeyValuePair<string, Color>>();
        protected override void trace(string _categoray, string _message)
        {
            this.appendTextColorful(string.Format("TRACE | {0} > {1}", _categoray, _message), Color.FromRgb(119, 136, 153));
        }

        protected override void debug(string _categoray, string _message)
        {
            this.appendTextColorful(string.Format("DEBUG | {0} > {1}", _categoray, _message), Color.FromRgb(65, 105, 225));
        }

        protected override void info(string _categoray, string _message)
        {
            this.appendTextColorful(string.Format("INFO | {0} > {1}", _categoray, _message), Color.FromRgb(50, 205, 50));
        }

        protected override void warning(string _categoray, string _message)
        {
            this.appendTextColorful(string.Format("WARN | {0} > {1}", _categoray, _message), Color.FromRgb(255, 165, 0));
        }

        protected override void error(string _categoray, string _message)
        {
            this.appendTextColorful(string.Format("ERROR | {0} > {1}", _categoray, _message), Color.FromRgb(255, 0, 0));
        }

        protected override void exception(Exception _exception)
        {
            this.appendTextColorful(string.Format("EXCEPT | > {0}", _exception.ToString()), Color.FromRgb(138, 43, 226));
        }

        private void appendTextColorful(string addtext, Color color)
        {
            if (null == appendDelegate)
            {
                cache.Add(new KeyValuePair<string, Color>(addtext, color));
                return;
            }

            if (cache.Count > 0)
            {
                foreach (var msg in cache)
                {
                    appendDelegate(msg.Key, msg.Value);
                }
                cache.Clear();
            }
            appendDelegate(addtext, color);
        }
    }//class
}//namespace
