using System;
using System.Collections.Generic;
using System.Text;

namespace VTAServiceCheck.EventLog
{

  #region LogMessageEventArgs Class

  public class LogMessageEventArgs : EventArgs
  {
    private string _logMessage;

    public LogMessageEventArgs(string logMessage)
    {

      _logMessage = logMessage;
    }

    public string LogMessage
    {
      get { return _logMessage; }
      set { _logMessage = value; }
    }
  }

  #endregion

}
