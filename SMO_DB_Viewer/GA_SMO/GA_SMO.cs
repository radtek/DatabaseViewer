using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.MessageBox;

using Microsoft.SqlServer.Replication;

//using MaintServerDAL.SQLServerObjects;
//using MaintServerDAL.BusinessObjects;

namespace GA_SMO
{
   public class SMOFunctions
   {
      private string _FileName = string.Empty;
      public string _LoggingFile;
      StreamWriter _LogFile;
      private bool _Replicated = false;

      public string FileName
      {
         set { _FileName = value; }
         get { return _FileName; }
      }

      public string LoggingFile
      {
         get { return _LoggingFile; }
         set { _LoggingFile = value; }
      }

      public bool Replicated
      {
         set { _Replicated = value; }
         get { return _Replicated; }
      }

      public SMOFunctions()
      {
      }

      public SMOFunctions(string LoggingFile)
      {
         _LogFile = new StreamWriter(LoggingFile);
//         _LogFile.LogFilePath = LoggingFile;
      }

      #region "Jobs"
      public string GetJobScript(Server JobServer, string JobName)
      {
         Job _Job = JobServer.JobServer.Jobs[JobName];
         return ScriptObject<Job>(_Job, null);
      }

      public List<Job> ListSourceServerJobs(Server SourceServer)
      {
         List<Job> _Jobs = new List<Job>();
         foreach (Job _Job in SourceServer.JobServer.Jobs)
         {
            _Jobs.Add(_Job);
         }
         return _Jobs;
      }

      public void CreateJobsOnDestination(Server SourceServer, Server DestServer)
      {
         if (DestServer.Status != ServerStatus.Online)
         {
            throw new Exception("Unable to connect to " + DestServer.Name);
            return;
         }

         if (SourceServer.Status != ServerStatus.Online)
         {
            throw new Exception("Unable to connect to " + SourceServer.Name);
            return;
         }

         // Get the collection of source server Jobs
         // List<Job> Jobs = this.ListSourceServerJobs(SourceServer);

         foreach (Job _Job in SourceServer.JobServer.Jobs)
         {
            if (!(_Job.Category.Contains("REPL-LogReader")
                 || _Job.Category.Contains("REPL-Distribution")
                 || _Job.Category.Contains("REPL-QueueReader")))
            {
               try
               {
                  DestServer.ConnectionContext.Connect();
                  if (DestServer.JobServer.Jobs.Contains(_Job.Name))
                  {
                     _LogFile.WriteLine(string.Format("Dropping {0} from {1}.", _Job.Name, DestServer.Name));
                     DestServer.JobServer.Jobs[_Job.Name].Drop();
                  }

                  _Job.IsEnabled = false;
                  _LogFile.WriteLine(string.Format("Creating {0} from {1}.", _Job.Name, SourceServer.Name));
                  DestServer.Databases["MSDB"].ExecuteNonQuery(ScriptObject<Job>(_Job, null));
               }
               catch (Exception ex)
               {
                  throw (ex);
               }
               DestServer.ConnectionContext.Disconnect();
            }
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="Script"></param>
      /// <param name="JobName"></param>
      /// <param name="DestinationServer"></param>
      public void CreateSpecificJobOnDestination(string Script, string JobName, string DestinationServer)
      {
         try
         {
            Server destserv = new Server(DestinationServer);
            if (destserv.JobServer.Jobs.Contains(JobName))
               destserv.JobServer.Jobs[JobName].Drop();

            destserv.Databases["MSDB"].ExecuteNonQuery(Script);
         }
         catch (Exception ex)
         {
            throw ex;
         }
      }

      public void ScriptJobs(Server JobServer, string ScriptFile)
      {
         SMOWrapper smoWrapper = new SMOWrapper();
         ScriptingOptions oScriptingOptions;

         oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         oScriptingOptions.ScriptDrops = false;
         oScriptingOptions.IncludeDatabaseContext = true;

         oScriptingOptions.DriForeignKeys = true;
         oScriptingOptions.SchemaQualify = true;
         oScriptingOptions.SchemaQualifyForeignKeysReferences = true;
         oScriptingOptions.ScriptSchema = true;

         //Since Alerts and Notifications are dependencies we script these first.
         ScriptJobAlerts(JobServer, oScriptingOptions, ScriptFile);

         // Script Notifications 
         ScriptNotifications(JobServer,oScriptingOptions, ScriptFile);

         foreach (Job _Job in JobServer.JobServer.Jobs)
         {
               string JobScript = smoWrapper.ScriptObject<Job, ScriptingOptions, int>(_Job, oScriptingOptions, 2);
               ScriptWriter(ScriptFile, JobScript);
         }
      }

      public void ScriptViews(string SQLServer, string SQLDatabase, string ScriptFile)
      {
         Server oServer = new Server(SQLServer);

         SMOWrapper smoWrapper = new SMOWrapper();
         ScriptingOptions oScriptingOptions;

         oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         oScriptingOptions.ScriptDrops = false;
         oScriptingOptions.IncludeDatabaseContext = true;

         oScriptingOptions.DriForeignKeys = true;
         oScriptingOptions.SchemaQualify = true;
         oScriptingOptions.SchemaQualifyForeignKeysReferences = true;
         oScriptingOptions.ScriptSchema = true;

         foreach (View _view in oServer.Databases[SQLDatabase].Views)
         {
            try
            {
               if (!_view.IsSystemObject)
               {
                  string TriggerScript = smoWrapper.ScriptObject<View, ScriptingOptions, int>(_view, oScriptingOptions, 2);
                  ScriptWriter(ScriptFile, TriggerScript);
               }
            }
            catch (Exception Ex)
            {
               TextWriter oErrorWriter = new StreamWriter(@"c:\DBScripts\Error\Error.txt");
               string UDFError = string.Format("{0} - {1} " + Ex.Message, SQLServer, SQLDatabase);
               oErrorWriter.Write(UDFError);
               oErrorWriter.Close();
            }

         }
      }

      private void ScriptJobAlerts(Server _JobServer, ScriptingOptions so, string ScriptFile)
      {
         SMOWrapper smoWrapper = new SMOWrapper();
         string _ScriptFile = ScriptFile.Replace("Jobs", "Alerts");
         if (!(System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(_ScriptFile))))
         {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_ScriptFile));
         }

         foreach (Alert _Alert in _JobServer.JobServer.Alerts)
         {
            string AlertScript = smoWrapper.ScriptObject<Job, ScriptingOptions, int>(_Alert, so, 2);
            ScriptWriter(_ScriptFile, AlertScript);
            
         }
      }

      private void ScriptNotifications(Server _JobServer, ScriptingOptions so, string ScriptFile)
      {
         SMOWrapper smoWrapper = new SMOWrapper();
         string _ScriptFile = ScriptFile.Replace("Jobs", "Operators");

         if (!(System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(_ScriptFile))))
         {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_ScriptFile));
         }
         
         foreach (Operator _Operator in _JobServer.JobServer.Operators)
         {
            string OperatorScript = smoWrapper.ScriptObject<Job, ScriptingOptions, int>(_Operator, so, 2);
            ScriptWriter(_ScriptFile, OperatorScript);

         }
      }

      #endregion

      #region "Stored Procedures"
      public void ScriptStoredProcedures(string SQLServer, string SQLDatabase, string ScriptFile)
      {
         Server oServer = new Server(SQLServer);
         Database _DB = null;

         SMOWrapper smoWrapper = new SMOWrapper();
         ScriptingOptions oScriptingOptions = new ScriptingOptions();

         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         oScriptingOptions.ScriptDrops = false;
         oScriptingOptions.IncludeDatabaseContext = true;

         oScriptingOptions.DriForeignKeys = true;
         oScriptingOptions.SchemaQualify = true;
         oScriptingOptions.SchemaQualifyForeignKeysReferences = true;
         oScriptingOptions.ScriptSchema = true;

         _DB = oServer.Databases[SQLDatabase];
         foreach (StoredProcedure _sp in _DB.StoredProcedures)
         {
            try
            {
               if (!_sp.IsSystemObject)
               {
                  string TriggerScript = smoWrapper.ScriptObject<StoredProcedure, ScriptingOptions, int>(_sp, oScriptingOptions, 2);
                  ScriptWriter(ScriptFile, TriggerScript);
               }
            }
            catch (Exception Ex)
            {
               TextWriter oErrorWriter = new StreamWriter(@"c:\DBScripts\Error\Error.txt");
               string UDFError = string.Format("{0} - {1} " + Ex.Message, SQLServer, SQLDatabase);
               oErrorWriter.Write(UDFError);
               oErrorWriter.Close();
            }

         }
      }

      #endregion

      #region "Triggers"
      public void ScriptTrigger( string SQLServer, string SQLDatabase, string ScriptFile )
      {
         // http://rajganesh-mountbatton.blogspot.com/2008/03/using-smo-to-generate-script-of.html
         Server oServer = new Server(SQLServer);
         Database _DB = null; 
         TextWriter oTextWriter = null;

         SMOWrapper smoWrapper = new SMOWrapper();
         ScriptingOptions oScriptingOptions;

         oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         oScriptingOptions.ScriptDrops = false;
         oScriptingOptions.IncludeDatabaseContext = true;

         oScriptingOptions.DriForeignKeys = true;
         oScriptingOptions.SchemaQualify = true;
         oScriptingOptions.SchemaQualifyForeignKeysReferences = true;
         oScriptingOptions.ScriptSchema = true;

         try
         {
            _DB = oServer.Databases[SQLDatabase];
            //oTextWriter = new StreamWriter(@"C:\DBScripts\DbScripts1.sql");
            //if (oTextWriter != null)
            //{
            foreach (Table _Table in _DB.Tables)
            {
               TriggerCollection oTriggerCollect = _Table.Triggers;
               if (oTriggerCollect.Count > 0)
               {
                  //oSBText.Append("\n" + "--" + _DB.Name + "\n" + "--" + _Table.Name);
                  foreach (Trigger oTrg in oTriggerCollect)
                  {
                     try
                     {
                        string TriggerScript = smoWrapper.ScriptObject<Trigger, ScriptingOptions, int>(oTrg, oScriptingOptions, 2);
                        ScriptWriter(ScriptFile, TriggerScript);
                     }

                     catch (Exception Ex)
                     {
                        TextWriter oErrorWriter = new StreamWriter(@"c:\DBScripts\Error\Error.txt");
                        oErrorWriter.Write(Ex.Message);
                        oErrorWriter.Close();
                     }
                  }
               }
            }

         }  // End
         catch (Exception ex)
         {
            throw ex;
         }
         
      }

      private void ScriptWriter(string ScriptFile, string Script_Text)
      {
         //ScriptFile = ScriptFile.Replace(@"\\", @"\");

         // This text is added only once to the file.
         if (!File.Exists(ScriptFile))
         {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(ScriptFile))
            {
               sw.Write(Script_Text);
            }
         }
         else
         {
            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(ScriptFile))
            {
               sw.Write(Script_Text);
            }
         }

      }
      #endregion

      #region "Indexes"
      public string Script_Indexes(Server SQLServer, string SQLDatabase, string Table, string ScriptFile)
      {
        // http://rajganesh-mountbatton.blogspot.com/2008/03/using-smo-to-generate-script-of.html
        // Server oServer = new Server(SQLServer);

        SQLServer.ConnectionContext.Connect();
        Database _DB = null;

        SMOWrapper smoWrapper = new SMOWrapper();
        ScriptingOptions oScriptingOptions;

        oScriptingOptions = new ScriptingOptions();
        oScriptingOptions.IncludeDatabaseContext = true;
        oScriptingOptions.IncludeIfNotExists = false;
        oScriptingOptions.ScriptDrops = false;
        // oScriptingOptions.

        StringBuilder SWScript = new StringBuilder();
        try
        {
          _DB = SQLServer.Databases[SQLDatabase];
          //oTextWriter = new StreamWriter(@"C:\DBScripts\DbScripts1.sql");
          //if (oTextWriter != null)
          //{
          foreach (Index _IDX in _DB.Tables[Table].Indexes)
          {
            try
            {
              string IndexScript = smoWrapper.ScriptObject<Index, ScriptingOptions, int>(_IDX, oScriptingOptions, 2);

              if (!(string.IsNullOrEmpty(ScriptFile)))
              {
                ScriptWriter(ScriptFile, IndexScript);
              }
              else
              {
                SWScript.AppendLine(IndexScript);
              }
            }

            catch (Exception Ex)
            {
              TextWriter oErrorWriter = new StreamWriter(@"c:\DBScripts\Error\Error.txt");
              oErrorWriter.Write(Ex.Message);
              oErrorWriter.Close();
            }
          }
        }
        catch (Exception ex)
        {
          throw ex;
        }
        return SWScript.ToString();
      }
#endregion

      #region "Users"
      public void Script_Users(string SQLServer, string SQLDatabase, string ScriptFile)
      {
         Server oServer = new Server(SQLServer);
         Database _DB = null;
         TextWriter oTextWriter = null;

         SMOWrapper smoWrapper = new SMOWrapper();
         ScriptingOptions oScriptingOptions = new ScriptingOptions();

         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         oScriptingOptions.ScriptDrops = false;
         oScriptingOptions.IncludeDatabaseContext = true;

         oScriptingOptions.DriForeignKeys = true;
         oScriptingOptions.SchemaQualify = true;
         oScriptingOptions.SchemaQualifyForeignKeysReferences = true;
         oScriptingOptions.ScriptSchema = true;

         RemoveScriptFileIfExists(ScriptFile);
         
         _DB = oServer.Databases[SQLDatabase];
         foreach (User _User in _DB.Users)
         {
            try
            {
               if (!_User.IsSystemObject)
               {
                  string UserScript = smoWrapper.ScriptObject<User, ScriptingOptions, int>
                     (_User, oScriptingOptions, 2);
                  ScriptWriter(ScriptFile, UserScript);
               }
            }
            catch (Exception Ex)
            {
               TextWriter oErrorWriter = new StreamWriter(@"c:\DBScripts\Error\Error.txt");
               string UDFError = string.Format("{0} - {1} " + Ex.Message, SQLServer, SQLDatabase);
               oErrorWriter.Write(UDFError);
               oErrorWriter.Close();
            }
         }
      }

#endregion

      #region "Logins"
      public void Script_Logins(string SQLServer, string SQLDatabase, string ScriptFile)
      { 
         Server oServer = new Server(SQLServer);
         Database _DB = null;

         SMOWrapper smoWrapper = new SMOWrapper();
         ScriptingOptions oScriptingOptions = new ScriptingOptions();

         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         oScriptingOptions.ScriptDrops = false;
         oScriptingOptions.IncludeDatabaseContext = true;

         oScriptingOptions.DriForeignKeys = true;
         oScriptingOptions.SchemaQualify = true;
         oScriptingOptions.SchemaQualifyForeignKeysReferences = true;
         oScriptingOptions.ScriptSchema = true;

         RemoveScriptFileIfExists(ScriptFile);
         
         _DB = oServer.Databases[SQLDatabase];
         foreach (Login _login in oServer.Logins)
         {
            try
            {
               if (!_login.IsSystemObject)
               {
                  string UserScript = smoWrapper.ScriptObject<Login, ScriptingOptions, int>
                     (_login, oScriptingOptions, 2);
                  ScriptWriter(ScriptFile, UserScript);
               }
            }
            catch (Exception Ex)
            {
               throw Ex;

               //TextWriter oErrorWriter = new StreamWriter(@"c:\DBScripts\Error\Error.txt");
               //string UDFError = string.Format("{0} - {1} " + Ex.Message, SQLServer, SQLDatabase);
               //oErrorWriter.Write(UDFError);
               //oErrorWriter.Close();
            }
         }

      }
      #endregion

      #region "User Defined Functions"
      public void ScriptUDFs(string SQLServer, string SQLDatabase, string ScriptFile)
      {
         // http://rajganesh-mountbatton.blogspot.com/2008/03/using-smo-to-generate-script-of.html
         Server oServer = new Server(SQLServer);
         Database _DB = null;
         TextWriter oTextWriter = null;

         SMOWrapper smoWrapper = new SMOWrapper();
         ScriptingOptions oScriptingOptions = new ScriptingOptions();

         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         oScriptingOptions.ScriptDrops = false;
         oScriptingOptions.IncludeDatabaseContext = true;

         oScriptingOptions.DriForeignKeys = true;
         oScriptingOptions.SchemaQualify = true;
         oScriptingOptions.SchemaQualifyForeignKeysReferences = true;
         oScriptingOptions.ScriptSchema = true;

         RemoveScriptFileIfExists(ScriptFile);
         
         _DB = oServer.Databases[SQLDatabase];
         foreach (UserDefinedFunction _udf in _DB.UserDefinedFunctions)
         {
            try
            {
               if (!_udf.IsSystemObject)
               {
                  string TriggerScript = smoWrapper.ScriptObject<UserDefinedFunction, ScriptingOptions, int>(_udf, oScriptingOptions, 2);
                  ScriptWriter(ScriptFile, TriggerScript);
               }
            }

            catch (Exception Ex)
            {
               TextWriter oErrorWriter = new StreamWriter(@"c:\DBScripts\Error\Error.txt");
               string UDFError = string.Format("{0} - {1} " + Ex.Message, SQLServer, SQLDatabase);
               oErrorWriter.Write(UDFError);
               oErrorWriter.Close();
            }
         }
      }
      #endregion

      #region "JobSchedule"

      private List<JobSchedule> SourceServerJobSchedules(Job SourceServerJob)
      {
         List<JobSchedule> _Schedule = new List<JobSchedule>();
         foreach (JobSchedule JSchedule in SourceServerJob.JobSchedules)
         {
            _Schedule.Add(JSchedule);
         }
         return _Schedule;
      }

      public void CreateJobScheduleOnDestination(Server SourceServer, Server DestServer)
      {
         foreach (Job _Job in SourceServer.JobServer.Jobs)
         {
            List<JobSchedule> _Schedule = SourceServerJobSchedules(_Job);
            if (DestServer.JobServer.Jobs.Contains(_Job.Name))
            {
               Guid DestJobID = DestServer.JobServer.Jobs[_Job.Name].JobID;
               foreach (JobSchedule _JobSched in _Schedule)
               {
                  _JobSched.ScheduleUid = DestJobID;
                  _JobSched.IsEnabled = false;

                  StringCollection strcoll = _JobSched.Script();
                  string CreateJobScheduleScript = "";

                  foreach (string JobScheduleString in strcoll)
                  {
                     CreateJobScheduleScript += JobScheduleString;
                  }
                  DestServer.Databases["MSDB"].ExecuteNonQuery(CreateJobScheduleScript);
               }
            }
         }
      }

      #endregion

      #region "Foreign Keys"

      public void ScriptForeignKeys(List<TableAndFKey> TblFkey, string SQLSrvrInstance)
      {
         ServerConnection SQLSrvrConnection = new ServerConnection(SQLSrvrInstance);
         System.Collections.Specialized.StringCollection script = new StringCollection();

         // Recreate connection if necessary
         if (SQLSrvrInstance == null)
         {
            SQLSrvrConnection = new ServerConnection();
         }

         // Fill in necessary information
         SQLSrvrConnection.ServerInstance = SQLSrvrInstance;

         // Setup capture and execute to be able to display script
         SQLSrvrConnection.SqlExecutionModes = SqlExecutionModes.ExecuteAndCaptureSql;

         // Use Windows authentication
         SQLSrvrConnection.LoginSecure = true;

         // Go ahead and connect
         SQLSrvrConnection.Connect();
         Server srv = new Server(SQLSrvrConnection);

         ScriptingOptions oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = true;

         /*
         //oScriptingOptions.AppendToFile = true;
         //oScriptingOptions.DriForeignKeys = true;
         //oScriptingOptions.SchemaQualify = true;
         //oScriptingOptions.SchemaQualifyForeignKeysReferences = true;
         //oScriptingOptions.ScriptSchema = true;
          */

         Londeck.Net.Logging.LogFile _LogFile = new Londeck.Net.Logging.LogFile(@"C:\UnTrustedFKeys\June-UnTrustedFKeys.sql", true, 0);

         foreach (TableAndFKey _FKeyTbl in TblFkey)
         {
            // ***** Make sure that the Database is correct in srv.Databases[""]
            ForeignKey fk = srv.Databases[_FKeyTbl.DBName].Tables[_FKeyTbl.TableName, "dbo"].ForeignKeys[_FKeyTbl.FKeyName];
            script = fk.Script(oScriptingOptions);
            foreach (String str in script)
            {
               _LogFile.WriteLogMessage(str);
            }
            _LogFile.WriteBlankLines(1);
         }
         _LogFile.CloseLogFile(true);
      }

      public string ScriptUnTrustedKeys(List<TableAndFKey> TblFkey, string SQLSrvrInstance)
      {
         ServerConnection SQLSrvrConnection = new ServerConnection(SQLSrvrInstance);
         System.Collections.Specialized.StringCollection script = new StringCollection();

         // Recreate connection if necessary
         if (SQLSrvrInstance == null)
         {
            SQLSrvrConnection = new ServerConnection();
         }

         if (string.IsNullOrEmpty(this.FileName))
         {
            System.Windows.Forms.MessageBox.Show("No file name give for the output script.", "Script Foreign Keys.", System.Windows.Forms.MessageBoxButtons.OK);
         }

         // Fill in necessary information
         SQLSrvrConnection.ServerInstance = SQLSrvrInstance;

         // Setup capture and execute to be able to display script
         SQLSrvrConnection.SqlExecutionModes = SqlExecutionModes.ExecuteAndCaptureSql;

         // Use Windows authentication
         SQLSrvrConnection.LoginSecure = true;

         // Go ahead and connect
         SQLSrvrConnection.Connect();
         Server srv = new Server(SQLSrvrConnection);

         ScriptingOptions oScriptingOptions;

         string ScriptedFK = string.Empty;
         SMOWrapper smoWrapper = new SMOWrapper();
         if (System.IO.File.Exists(FileName))
            System.IO.File.Delete(FileName);

         Londeck.Net.Logging.LogFile _LogFile = new Londeck.Net.Logging.LogFile(FileName, true, 0);
         try
         {
            foreach (TableAndFKey _TableFKey in TblFkey)
            {
               // ***** Make sure that the Database is correct in srv.Databases[""]
               // Set up the Foreign Key Drop script
               oScriptingOptions = new ScriptingOptions();
               oScriptingOptions.IncludeDatabaseContext = true;
               oScriptingOptions.IncludeIfNotExists = true;

               oScriptingOptions.ScriptDrops = true;
               oScriptingOptions.IncludeDatabaseContext = true;

               oScriptingOptions.DriForeignKeys = true;
               oScriptingOptions.SchemaQualify = true;
               oScriptingOptions.SchemaQualifyForeignKeysReferences = true;
               oScriptingOptions.ScriptSchema = true;

               ForeignKey fk = srv.Databases[_TableFKey.DBName].Tables[_TableFKey.TableName, "dbo"].ForeignKeys[_TableFKey.FKeyName];
               //script = fk.Script(oScriptingOptions);

               ScriptedFK = smoWrapper.ScriptObject<ForeignKey, ScriptingOptions, int>(fk, oScriptingOptions, 0);
               ScriptedFK += Environment.NewLine;
               oScriptingOptions = null;

               // Set up the Create Foreign Key script
               oScriptingOptions = new ScriptingOptions();
               oScriptingOptions.IncludeDatabaseContext = true;

               ScriptedFK += smoWrapper.ScriptObject<ForeignKey, ScriptingOptions, int>(fk, oScriptingOptions, 2);

               _LogFile.WriteLogMessage(ScriptedFK);
               _LogFile.WriteBlankLines(1);
               ScriptedFK = string.Empty;
            }
            _LogFile.CloseLogFile(true);

         }
         catch (NullReferenceException ex)
         {
            _LogFile.CloseLogFile(true);
            throw ex;
         }
         catch (Exception ex)
         {
            _LogFile.CloseLogFile(true);
            throw ex;
         }
         return FileName;
      }

      public string ScriptMissingForeignKeys(List<TableAndFKey> TblFkey, string NodeMissingKeys, bool NRTs)
      {
         string SQLSrvrInstance;
         if (NRTs)
         {
            SQLSrvrInstance = @"AH-N1-NRT1-VM\AH_N1_NRT1";
         }
         else
         {
            SQLSrvrInstance = @"AHN1PRIMARY";
         }

         ServerConnection SQLSrvrConnection = new ServerConnection(SQLSrvrInstance);
         System.Collections.Specialized.StringCollection script = new StringCollection();

         // Recreate connection if necessary
         if (SQLSrvrInstance == string.Empty)
         {
            SQLSrvrConnection = new ServerConnection();
         }

         // Fill in necessary information
         SQLSrvrConnection.ServerInstance = SQLSrvrInstance;

         // Setup capture and execute to be able to display script
         SQLSrvrConnection.SqlExecutionModes = SqlExecutionModes.ExecuteAndCaptureSql;

         // Use Windows authentication
         SQLSrvrConnection.LoginSecure = true;

         // Go ahead and connect
         SQLSrvrConnection.Connect();
         Server srv = new Server(SQLSrvrConnection);

         ScriptingOptions oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = true;

         //oScriptingOptions.DriForeignKeys = true;
         //if (Replicated)
         //{
         //    oScriptingOptions.ScriptDrops = true;
         //    oScriptingOptions.IncludeIfNotExists = false;
         //}
         //oScriptingOptions.SchemaQualify = true;
         //oScriptingOptions.SchemaQualifyForeignKeysReferences = true;
         //oScriptingOptions.ScriptSchema = true;

         string ScriptedFK = string.Empty;
         string MissingFKeysFileName = ConstructMissingFKsLogFileName(NodeMissingKeys);
         int i = 0;

         if (System.IO.File.Exists(@"C:\MissingFKeys\" + MissingFKeysFileName))
            System.IO.File.Delete(@"C:\MissingFKeys\" + MissingFKeysFileName);

         Londeck.Net.Logging.LogFile _LogFile = new Londeck.Net.Logging.LogFile(@"C:\MissingFKeys\" + MissingFKeysFileName, true, 0);
         try
         {
            foreach (TableAndFKey _TableFKey in TblFkey)
            {
               i++;
               // ***** Make sure that the Database is correct in srv.Databases[""]
               ForeignKey fk = srv.Databases[_TableFKey.DBName].Tables[_TableFKey.TableName, "dbo"].ForeignKeys[_TableFKey.FKeyName];
               if (Replicated)
                  ScriptedFK = ScriptDropFK(fk); // +Environment.NewLine;

               if (ScriptedFK.Length > 0)
               {
                  ScriptedFK += Environment.NewLine;
                  ScriptedFK += ScriptCreateFK(fk) + Environment.NewLine;
               }

               //foreach (ForeignKey _fk in fk)
               //{
               //script = fk.Script(oScriptingOptions);
               ////}

               //foreach (string str in script)
               //{
               //    if (str.Contains("USE ["))
               //    {
               //        ScriptedFK += str + Environment.NewLine;
               //    }
               //    else
               //    {
               //        ScriptedFK += str;
               //    }
               //}

               // Fix the script to not include the IF EXISTS logic.
               // Just assume that the key may just not exist.
               //int idx = ScriptedFK.IndexOf("IF  EXISTS (");
               //ScriptedFK = ScriptedFK.Substring(0, idx);
               //ScriptedFK += Environment.NewLine + "GO"; // + Environment.NewLine;

               if (ScriptedFK.Length > 0)
               {
                  _LogFile.WriteLogMessage(ScriptedFK);
                  _LogFile.WriteBlankLines(1);
               }
               ScriptedFK = string.Empty;
            }
            _LogFile.CloseLogFile(true);
         }
         catch (Exception ex)
         {
            _LogFile.CloseLogFile(true);
            throw ex;
         }
         return @"C:\MissingFKeys\" + MissingFKeysFileName;
      }

      private string ScriptDropFK(ForeignKey fk)
      {
         ScriptingOptions oScriptingOptions = new ScriptingOptions();
         System.Collections.Specialized.StringCollection script = new StringCollection();
         string ScriptedFK = string.Empty;

         oScriptingOptions.DriForeignKeys = true;
         if (Replicated)
         {
            oScriptingOptions.IncludeDatabaseContext = true;
            oScriptingOptions.ScriptDrops = true;
            oScriptingOptions.IncludeIfNotExists = true;
         }

         script = fk.Script(oScriptingOptions);
         foreach (string str in script)
         {
            if (str.Contains("USE ["))
            {
               ScriptedFK += str + Environment.NewLine;
            }
            else
            {
               ScriptedFK += str + Environment.NewLine;
            }
         }

         return ScriptedFK;
      }

      private string ScriptCreateFK(ForeignKey fk)
      {
         System.Collections.Specialized.StringCollection script = new StringCollection();

         ScriptingOptions oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;
         oScriptingOptions.DriWithNoCheck = false;

         string ScriptedFK = string.Empty;
         script = fk.Script(oScriptingOptions);

         foreach (string str in script)
         {
            if (str.Contains("USE ["))
            {
               ScriptedFK += str + Environment.NewLine;
            }
            else
            {
               ScriptedFK += str;
            }
         }

         // Fix the script to not include the IF EXISTS logic.
         // Just assume that the key may just not exist.
         int idx = ScriptedFK.IndexOf("IF  EXISTS (");
         if (idx != -1)
         {
            ScriptedFK = ScriptedFK.Substring(0, idx);
         }
         ScriptedFK += Environment.NewLine + "GO";   // + Environment.NewLine;
         return ScriptedFK;
      }

      private string ConstructMissingFKsLogFileName(string SQLInstance)
      {
         int idxInstance = SQLInstance.IndexOf(@"\");
         int idxNode = SQLInstance.IndexOf(@"_N");

         // Don't need the "-" character from the name convention.
         string NodeName = SQLInstance.Substring(idxNode + 1, 2);
         string Instance = SQLInstance.Substring(0, idxInstance - 1);
         string replicated = this.Replicated ? "_Replicated" : "_Standard";

         string ConstructedFileName = NodeName + replicated + "_MissingFkeys" + DateTime.Now.ToString("MMMM") + DateTime.Now.Day.ToString() + ".sql";
         return ConstructedFileName;
      }

      #endregion

      #region "Operators"
      private List<Operator> ListSourceServerOperators(Server SourceServer)
      {
         List<Operator> _Operators = new List<Operator>();
         Operator _Operator = new Operator();

         foreach (Operator SQLOperator in SourceServer.JobServer.Operators)
         {
            _Operators.Add(SQLOperator);
         }

         return _Operators;
      }

      public void CreateOperatorsOnDestination(Server SourceServer, Server DestServer)
      {
         // Get the collection of source server operators
         List<Operator> _Operators = this.ListSourceServerOperators(SourceServer);

         foreach (Operator SQLOperator in _Operators)
         {
            DestServer.ConnectionContext.Connect();
            if (DestServer.JobServer.Operators.Contains(SQLOperator.Name))
            {
               DestServer.JobServer.Operators[SQLOperator.Name].Drop();
            }

            _LogFile.WriteLine(string.Format("Applying Operator: {0} to {1} from {2}.", SQLOperator.Name, DestServer.Name, SourceServer.Name));

            DestServer.Databases["MSDB"].ExecuteNonQuery(ScriptObject<Operator>(SQLOperator, null));
         }
         DestServer.ConnectionContext.Disconnect();
      }
      #endregion

      #region "Alert"
      public void CopyAlerts(Server SourceServer, Server DestinationServer)
      {
         List<Alert> _InstanceAlerts = new List<Alert>();
         foreach (Alert IAlert in SourceServer.JobServer.Alerts)
         {
            if (DestinationServer.JobServer.Alerts.Contains(IAlert.Name))
            {
               DestinationServer.JobServer.Alerts[IAlert.Name].Drop();
               DestinationServer.JobServer.Alerts.Add(IAlert);
            }
         }
      }

      #endregion

      #region "Utility Functions"

      private string ScriptObject<T>(dynamic ObjectToScript, ScriptingOptions so)
      {
         System.Collections.Specialized.StringCollection script = new StringCollection();
         // ScriptingOptions oScriptingOptions = new ScriptingOptions();

         string ScriptedObject = string.Empty;
         if (!(so == null))
         {
            script = ObjectToScript.Script(so);
         }
         else
         {
            script = ObjectToScript.Script();
         }

         foreach (String str in script)
         {
            ScriptedObject += str;
         }
         return ScriptedObject;
      }

      #endregion

      #region "General "

      //public static string Stuff(this string input, int start, int length, string replaceWith)
      //{
      //   return input.Remove(start, length).Insert(start, replaceWith);
      //}

      public List<string> GetInstanceDatabases(string SQLSrvrInstance)
      {
         List<string> Databases = new List<string>();
         ServerConnection SQLSrvrConnection = new ServerConnection(SQLSrvrInstance);
         System.Collections.Specialized.StringCollection script = new StringCollection();

         // Recreate connection if necessary
         if (SQLSrvrInstance == null)
         {
            SQLSrvrConnection = new ServerConnection();
         }

         // Fill in necessary information
         SQLSrvrConnection.ServerInstance = SQLSrvrInstance;

         // Setup capture and execute to be able to display script
         SQLSrvrConnection.SqlExecutionModes = SqlExecutionModes.ExecuteAndCaptureSql;

         // Use Windows authentication
         SQLSrvrConnection.LoginSecure = true;

         // Go ahead and connect
         SQLSrvrConnection.Connect();
         Server srv = new Server(SQLSrvrConnection);

         foreach (Database idb in srv.Databases)
         {
            if (!idb.IsSystemObject)
            {
               Databases.Add(idb.Name);
            }
         }
         return Databases;
      }

      public bool GoodDatabaseInstance(string SQLSrvrInstance, IntPtr WindowHandle)
      {
         ServerConnection SQLSrvrConnection = new ServerConnection(SQLSrvrInstance);
         SQLSrvrConnection.ServerInstance = SQLSrvrInstance;

         // Setup capture and execute to be able to display script
         SQLSrvrConnection.SqlExecutionModes = SqlExecutionModes.ExecuteAndCaptureSql;

         // Use Windows authentication
         SQLSrvrConnection.LoginSecure = true;

         // Go ahead and connect
         try
         {
            SQLSrvrConnection.Connect();
         }
         catch (Exception ex)
         {
            ExceptionMessageBox emb = new ExceptionMessageBox();
            emb.Message = ex;
            emb.Show(new WindowWrapper(WindowHandle));
            return false;
         }

         bool Connected = SQLSrvrConnection.InUse;
         SQLSrvrConnection.Disconnect();
         return Connected;
      }

      private void RemoveScriptFileIfExists(string ScriptFile)
      { 
         FileInfo _fi = new FileInfo(ScriptFile);
         if(_fi.Exists)
         {
            _fi.Delete();
         }      
      }

      #endregion

   }

   public static class StringExtensions
   {
      public static string Stuff(this string input, int start, int length, string replaceWith)
      {
         return input.Remove(start, length).Insert(start, replaceWith);
      }

      public static string Repeat(string StringToRepeat, int Repeats)
      {
         return string.Concat(System.Linq.Enumerable.Repeat(StringToRepeat, Repeats));

      }
   }

}