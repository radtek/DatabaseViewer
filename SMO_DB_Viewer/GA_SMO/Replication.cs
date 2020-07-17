using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

using Microsoft.SqlServer.Replication;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace GA_SMO
{
   public class GA_Replication
   {
      public void Replication_Generate_Scripts(string BaseScriptFile, string PublicationServer)
      {
         dynamic ServerConnection = new ServerConnection(PublicationServer);
         try
         {
            // connect to publisher
            ServerConnection.LoginSecure = true;
            ServerConnection.Connect();
            string Filename = string.Empty;

            ScriptOptions _so = ScriptOptions.Creation | ScriptOptions.IncludeAll ^ ScriptOptions.IncludeReplicationJobs;

            dynamic ReplicationServer = new ReplicationServer(ServerConnection);
            dynamic ReplicationDatabases = ReplicationServer.ReplicationDatabases;

            foreach (ReplicationDatabase rdb in ReplicationDatabases)
            {
               if (rdb.HasPublications)
               {
                  foreach (TransPublication tp in rdb.TransPublications)
                  {
                     Filename = BaseScriptFile + "_" + tp.Name;
                     FileInfo _fi = new FileInfo(Filename);
                     if (_fi.Extension != "sql")
                     {
                        Filename += ".sql";
                     }

                     if (_fi.FullName.Length > 260)
                     {
                        Filename = Filename.Substring(0, 256) + ".sql";
                     }

                     System.IO.File.WriteAllText(Filename, tp.Script(_so));
                  }
               }
            }

         }
         catch (Exception ex)
         { 
            throw ex; 
         }
         finally
         {
            ServerConnection.Disconnect();
         }
      }

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

      private void ScriptWriter(string ScriptFile, string Script_Text)
      {
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

      /*
      // Create connection to local server

      {
         dynamic ServerConnection = new ServerConnection("MyServer");
         //'Dim File As File

         try {
            // connect to publisher
            ServerConnection.LoginSecure = true;
            ServerConnection.Connect();

            dynamic ReplicationServer = new ReplicationServer(ServerConnection);

            dynamic ReplicationDatabases = ReplicationServer.ReplicationDatabases;
            ReplicationDatabase ReplicationDatabase = default(ReplicationDatabase);
            TransPublication TransactionalPublication = default(TransPublication);

            foreach ( ReplicationDatabase in ReplicationDatabases) {

               if (ReplicationDatabase.HasPublications) {
                  foreach ( TransactionalPublication in ReplicationDatabase.TransPublications) {
                     FileName = "c:\\temp\\" + TransactionalPublication.Name + ".sql";

                     File.WriteAllText(FileName, TransactionalPublication.Script(ScriptOptions.Creation));
                  }

               }

            }

         } catch (Exception ex) {

         } finally {

            ServerConnection.Disconnect();

         }
      }
       */

   }
}
