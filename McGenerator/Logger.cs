using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace McGenerator
{
    public enum LogCategory
    {
        lcMessage = 0,
        lcWarning = 1,
        lcError = 2,
    }

    class Logger
    {
        private McGenerator.Form1 form_;

        public Logger(McGenerator.Form1 form)
        {
            form_ = form;
            Log("Created Logger", LogCategory.lcMessage);
        }

        public void Log( string msg, LogCategory lc )
        {
            form_.WriteLogMessage(msg, lc);
        }
    }
}
