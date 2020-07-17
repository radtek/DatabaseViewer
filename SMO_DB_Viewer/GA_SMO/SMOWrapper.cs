using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using Microsoft.SqlServer;
using Microsoft.SqlServer.MessageBox;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Londeck.Net.Logging;

namespace GA_SMO
{
   public class SMOWrapper
   {
      public string _LogFileName;

      public string LogFileName
      {
         get { return _LogFileName; }
         set { _LogFileName = value; }
      }

      public SMOWrapper()
      { }

      #region "Catalog/Backup Job Object to DB table"
      public void BackupDestinationJobstoFile(Server TargetServer)
      {
         try
         {
            JobServer TargetServerJobCollection = TargetServer.JobServer;
            string FileName = @"C:\Temp\Scripts\" + TargetServer.InstanceName + DateStamp.GetDateStamp(false) + ".JobsBackup.txt";
            Londeck.Net.Logging.LogFile _LogFile = new Londeck.Net.Logging.LogFile(FileName, true, Londeck.Net.Logging.LogFile.LogFileNamingType.DateStamped);

            foreach (Job J in TargetServerJobCollection.Jobs)
            {
               ScriptingOptions so = new ScriptingOptions();
               _LogFile.WriteLogMessage(ScriptObject<Job, ScriptingOptions>(J, so));

            }
         }
         catch (ConnectionFailureException ex)
         {
            throw ex;
         }
      }

      #endregion


      #region " Utility Functions "

      public string ScriptObject<T, U, V>(dynamic ObjectToScript, ScriptingOptions oScriptingOptions, int Passes)
      {
         System.Collections.Specialized.StringCollection script = new StringCollection();
         string ScriptedObject = string.Empty;
         script = ObjectToScript.Script(oScriptingOptions);
         bool AlreadyAppended = false;
         int Loop = 1;
         foreach (String str in script)
         {
            if (str.StartsWith("USE"))
            {
               ScriptedObject += Environment.NewLine + Environment.NewLine + str + Environment.NewLine;
               AlreadyAppended = true;
            }

            if (str.StartsWith("ALTER"))
            {
               if (Loop == 1)
               {
                  ScriptedObject += str.Replace("ALTER", Environment.NewLine + Environment.NewLine + "ALTER");
                  AlreadyAppended = true;
               }
               else
               {
                  ScriptedObject += str.Replace("ALTER", Environment.NewLine + "ALTER");
                  AlreadyAppended = true;
               }
            }

            if (str.StartsWith("CREATE"))
            {
               ScriptedObject += str.Replace("CREATE", Environment.NewLine + Environment.NewLine + "CREATE");
               AlreadyAppended = true;
            }

            if (!AlreadyAppended)
            {
               ScriptedObject += str;
            }
            AlreadyAppended = false;

            //if (Passes != 0)
            //{
            //   //if (Loop == Passes)
            //   //   break;
            //}
            Loop++;
         }
         return ScriptedObject;
      }

      public string ScriptObject<T, U>(dynamic ObjectToScript, ScriptingOptions so)
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

         foreach (string str in script)
         {
            ScriptedObject += str;
         }
         return ScriptedObject;
      }

      #endregion

   }
}