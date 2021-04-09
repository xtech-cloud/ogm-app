using System;
using System.Drawing;
using XTC.oelMVCS;

namespace OGM
{
    public class ConsoleLogger : Logger
    {
        public System.Windows.Forms.RichTextBox rtbLog { get; set; }
        protected override void trace(string _categoray, string _message)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("TRACE | {0} > {1}", _categoray, _message), Color.Gray);
        }

        protected override void debug(string _categoray, string _message)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("DEBUG | {0} > {1}", _categoray, _message), Color.Blue);
        }

        protected override void info(string _categoray, string _message)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("INFO | {0} > {1}", _categoray, _message), Color.Green);
        }

        protected override void warning(string _categoray, string _message)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("WARN | {0} > {1}", _categoray, _message), Color.Orange);
        }

        protected override void error(string _categoray, string _message)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("ERROR | {0} > {1}", _categoray, _message), Color.Red);
        }

        protected override void exception(Exception _exception)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("EXCEPT | > {0}", _exception.ToString()), Color.Purple);
        }

        private void appendTextColorful(string addtext, Color color)
        {
            addtext += Environment.NewLine;
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color;
            rtbLog.AppendText(addtext);
        }
    }//class
}//namespace
