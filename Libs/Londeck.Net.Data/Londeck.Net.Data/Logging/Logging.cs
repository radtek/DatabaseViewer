using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Londeck.Net.Logging
{
  public delegate void MessageLoggedDelegate(object sender, LogMessageEventArgs e);

  public class LogFile : IDisposable
  {
    private bool fHeaderWritten = false;

    #region PUBLIC ENUMS

    public enum LogFileNamingType
    {
      Standard,
      Circular,
      DateStamped
    }

    #endregion PUBLIC ENUMS

    #region PRIVATE MEMBERS

    private string _logFilePath = "";
    private string _LogFileName = string.Empty;
    private string _SourceServer = string.Empty;
    private string _DestinationServer = string.Empty;
    private StreamWriter _writer;
    static private bool _FileISDateStamped;
    private bool _disposed = false;
    private bool _appendToFile = false;

    #endregion PRIVATE MEMBERS

    #region PUBLIC PROPERTIES

    private LogFileNamingType _logFileNameType;

    public LogFileNamingType LogFileNameType
    {
      get { return _logFileNameType; }
      set { _logFileNameType = value; }
    }

    /// <summary>
    /// Set to TRUE to append to the current file if it exists (otherwise a new one is created or the existing is overwritten).
    /// </summary>
    public bool AppendToFile
    {
      get { return _appendToFile; }
      set { _appendToFile = value; }
    }

    public string DestinationServer
    {
      get { return _DestinationServer; }
      set { _DestinationServer = value; }
    }

    public string LogFileName
    {
      get
      {
        if (string.IsNullOrEmpty(_LogFileName))
          _LogFileName = "ServiceLog";

        return _LogFileName;
      }
      set { _LogFileName = value; }
    }

    /// <summary>
    /// The path to the logfile.
    /// </summary>
    public string LogFilePath
    {
      get { return _logFilePath; }
      set { _logFilePath = value; }
    }

    public string SourceServer
    {
      get { return _SourceServer; }
      set { _SourceServer = value; }
    }
    #endregion PUBLIC PROPERTIES

    #region PUBLIC EVENTS

    /// <summary>
    /// Raised when a message is logged to the logfile.
    /// </summary>
    public event MessageLoggedDelegate MessageLoggedEvent;

    #endregion PUBLIC EVENTS

    #region CONSTRUCTORS

    public LogFile()
        : this("", false, LogFileNamingType.DateStamped)
    {
    }

    public LogFile(string logFilePath)
        : this(logFilePath, true, LogFileNamingType.DateStamped)
    {

    }

    public LogFile(string logFilePath, bool appendToFile, LogFileNamingType type)
    {
      if(string.IsNullOrEmpty(_LogFileName))
        _LogFileName = "ServiceLog";

      if (Directory.Exists(logFilePath) && LogFileName.Length > 0)
      {
        switch (type)
        {
          case LogFileNamingType.DateStamped:
            if (logFilePath.EndsWith(@"\"))
            {
              logFilePath += LogFileName + DateStamp.GetDateStamp(false) + ".Log";
            }
            else
            {
              logFilePath += @"\" + LogFileName + DateStamp.GetDateStamp(false) + ".Log";
            }
            break;
          default:
            if (logFilePath.EndsWith(@"\"))
            {
              logFilePath += LogFileName + DateStamp.GetDateStamp(false) + ".Log";
            }
            else
            {
              logFilePath += @"\" + LogFileName + DateStamp.GetDateStamp(false) + ".Log";
            }
            break;
        }
      }
      else
      {
        throw new Exception("FilePath or FileName is not valid!!!");
      }
      _logFilePath = logFilePath;
      _appendToFile = appendToFile;
      _logFileNameType = type;
    }

    #endregion CONSTRUCTORS

    #region DISPOSE

    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (this._disposed == false)
      {
        if (disposing)
        {

        }

        //  Dispose managed resources:
        CloseLogFile(true);

      }  //  END IF

      this._disposed = true;

    }  //  END FUNCTION

    ~LogFile()
    {
      Dispose(false);
    }

    #endregion DISPOSE

    #region LOGGING METHODS

    /// <summary>
    /// Opens the logfile or creates a new one.
    /// </summary>
    public void OpenLogFile()
    {
      if (_writer != null)
        return;

      if (string.IsNullOrEmpty(LogFilePath))
      {
        /* Different methods to get the directory of execution.
         *  Console.WriteLine( Assembly.GetEntryAssembly().Location );
         *  Console.WriteLine( new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath );
         *  Console.WriteLine( Assembly.GetEntryAssembly().Location );
         *  Console.WriteLine( Environment.GetCommandLineArgs()[0] );
         *  Console.WriteLine( Process.GetCurrentProcess().MainModule.FileName );
         */

        // LogFilePath = System.Windows.Forms.Application.ExecutablePath + ".log";
        LogFilePath = Environment.CurrentDirectory + ".log";
        LogFilePath = LogFilePath.Replace(".exe", "").Replace(".EXE", "");
      }

      if (string.IsNullOrEmpty(LogFilePath))
        throw (new Exception("Invalid or empty log file path given!!!"));

      //  Determine the log file name per its naming type:
      if (LogFileNameType == LogFileNamingType.Circular)
      {
        LogFilePath = GetCircularFileName(LogFilePath);

        //  Determine if log file exists and needs to be overwritten:
        if (File.Exists(LogFilePath) == true)
        {
          TimeSpan tm = DateTime.Now - File.GetLastWriteTime(LogFilePath);
          if (tm.TotalDays >= 1)
          {
            AppendToFile = false;
          }
        }
      }
      else if (LogFileNameType == LogFileNamingType.DateStamped)
      {
        LogFilePath = GetDateStampedFileName(LogFilePath);
      }
      int retryWriteCount = 0;

      while (retryWriteCount <= 10)
      {
        try
        {
          _writer = new StreamWriter(LogFilePath, AppendToFile);
          retryWriteCount = 100;    //  Write was successful, don't retry.
        }
        catch (System.IO.IOException ex)
        {
          //  If the log is being viewed from a remote server using EditPlus or any other text-editing software that refreshes 
          // constantly, this error will come up:
          if (Regex.IsMatch(ex.Message, "The process cannot access the file .* because it is being used by another process", RegexOptions.IgnoreCase))
          {
            //  Sleep 500 ms and try again:
            retryWriteCount++;
            System.Threading.Thread.Sleep(1000);
          }
          else
          {
            throw (ex);
          }
        }
      }

    }  // END FUNCTION

    /// <summary>
    /// Returns a filename with a _MMDDYYYYHHNNSS date stamp added to it.
    /// </summary>
    /// <param name="fileName">The name and path of the logfile to modify.</param>
    /// <returns>c:\temp\log.log -> c:\temp\log_01152009015500.log</returns>
    public static string GetDateStampedFileName(string fileName)
    {
      string ext = GetFileExtension(fileName);
      if (ext == string.Empty)
        ext = "log";

      fileName = fileName.Replace(ext, "");
      fileName += DateStamp.GetDateStamp(false) + ext;

      _FileISDateStamped = true;
      //fileName += "_" + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + DateTime.Now.Year.ToString("0000") + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00") + ext;
      return fileName;
    }

    /// <summary>
    /// Returns the weekday-named circular filename of the given filename.
    /// </summary>
    /// <param name="fileName">The filename to circularly name.</param>
    /// <returns>c:\temp\log.txt -> c:\temp\log_Monday.txt</returns>
    public static string GetCircularFileName(string fileName)
    {
      string ext = GetFileExtension(fileName);
      fileName = fileName.Replace(ext, "");

      if (string.IsNullOrEmpty(ext) == true)
      {
        ext = ".log";
      }

      //  Append  the weekday name:
      fileName += "_" + DateTime.Now.DayOfWeek.ToString() + ext;
      return fileName;
    }

    /// <summary>
    /// Returns the extension of the given file name and/or path:
    /// </summary>
    /// <param name="fileName">The name or path of the file to evaluate.</param>
    /// <returns>The extension, . included:  .log, .txt, etc...</returns>
    public static string GetFileExtension(string fileName)
    {
      string ext = "";
      string[] fileParts;

      if (fileName.IndexOf(".") >= 0)
      {
        fileParts = fileName.Split('.');
        ext = "." + fileParts[fileParts.GetUpperBound(0)];
      }

      return ext;
    }

    /// <summary>
    /// Closes the file handle and disposes the writer.
    /// </summary>
    public void CloseLogFile(bool _Dispose)
    {
      try
      {
        if (_writer.BaseStream != null)
        {
          _writer.Flush();
          _writer.Close();

          if (_Dispose)
            _writer.Dispose();
        }
      }
      catch (Exception ex)
      {

      }
    }  //  END FUNCTION

    public void WriteBlankLines(int lines)
    {
      if (_writer == null)
        OpenLogFile();

      for (int i = 0; i <= lines - 1; i++)
      {
        _writer.WriteLine("");

        if (MessageLoggedEvent != null)
          MessageLoggedEvent(this, new LogMessageEventArgs(""));
      }
    }

    public void WriteLogHeaderMessage(string SourceServer, string DestinationServer)
    {
      string message;
      message = DateTime.Now.ToString();
      WriteLogMessage(message, false);
      message = string.Format("Source Server: {0}", SourceServer);
      WriteLogMessage(message, false);
      message = string.Format("Destination Server: {0}", DestinationServer);
      WriteLogMessage(message, false);
      fHeaderWritten = true;
      CloseLogFile(false);
    }

    /// <summary>
    ///  Writes a message to the application log file.
    /// </summary>
    /// <param name="message">The message to write to the log file.</param>
    public void WriteLogMessage(string message)
    {
      //_writer = new StreamWriter(LogFilePath, true);
      WriteLogMessage(message, false);
      // CloseLogFile(false);
    }

    /// <summary>
    ///  Writes a message to the application log file.
    /// </summary>
    /// <param name="message">The message to write to the log file.</param>
    /// <param name="omitTimeStamp">If TRUE, then a time stamp is not added to the log file entry.</param>
    public void WriteLogMessage(string message, bool omitTimeStamp)
    {
      if (_writer == null)
      {
        if (!(string.IsNullOrEmpty(LogFileName)))
        {
          _writer = new StreamWriter(LogFilePath, true);
        }
        else if (Path.GetFileName(LogFilePath).Length > 0)
        {
          _writer = new StreamWriter(LogFilePath, true);
        }
        else
        {
          throw new Exception("Filename is invalid or Missing!");
        }
      }
      if (omitTimeStamp == false)
      {
        message = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " : " + message;
      }

      try
      {
        _writer.WriteLine(message);

        if (MessageLoggedEvent != null)
          MessageLoggedEvent(this, new LogMessageEventArgs(message));

        _writer.Flush();
        _writer.Dispose();
        _writer.Close();
      }
      catch
      {
        System.Threading.Thread.Sleep(100);
      }
      CloseLogFile(true);

    }  //  END WriteLogMessage(string message, bool omitTimeStamp)

    #endregion LOGGING METHODS

  }

  public static class DateStamp
  {
    /// <summary>
    /// Returns a DateStamp in the format of:  _mmddyyyyhhnnss
    /// </summary>
    /// <returns></returns>
    public static string GetDateStamp(bool TimeAlso)
    {
      string DateTimeStamp = string.Empty;

      DateTimeStamp = "_" + DateTime.Now.Month.ToString().PadLeft(2, '0')
        + DateTime.Now.Day.ToString().PadLeft(2, '0')
        + DateTime.Now.Year.ToString().PadLeft(4, '0');

      if (TimeAlso)
      {
        DateTimeStamp += 
          DateTime.Now.Hour.ToString().PadLeft(2, '0')
           + DateTime.Now.Minute.ToString().PadLeft(2, '0') 
           + DateTime.Now.Second.ToString().PadLeft(2, '0');
      }
      return DateTimeStamp;
    }
  }

}
