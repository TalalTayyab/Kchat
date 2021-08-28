using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kchat.UI.Framework
{
    public class LoggingService
    {
        public event EventHandler<string> OnNewLog;

        public void AddLog(string logMessage)
        {
            OnNewLog?.Invoke(this, $"{DateTimeOffset.Now.ToString("dd/MM/yyyy HH:mm:ss")} {logMessage}");
        }
    }
}
