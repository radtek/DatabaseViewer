using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.MessageBox;
/*
    https://www.c-sharpcorner.com/UploadFile/d3e4b1/dynamically-getting-tables-collection-from-sql-server-using/
 */

namespace SMO_DB_Viewer
{
   public partial class Index_Viewer : Form
   {

      public ServerConnection SrvC;
      public Server srvr;

      public Index_Viewer()
      {
         InitializeComponent();
      }

      private void Index_Viewer_Load(object sender, EventArgs e)
      {
         /*
          * fbsql2014.cloudapp.net
          * fivebrothers1.cloudapp.net
          */
         cboServer.Items.Add(new SQLServerItems { Server_Name = "HTI-VISUAL" });
         //cboServer.Items.Add(new SQLServerItems { Server_Name = @"fivebrothers1.cloudapp.net" });
         cboServer.DisplayMember = "Server_Name";
         cboServer.ValueMember = "Server_Name";
         tbScripts.Margins[0].Width = 30;
      }

      //private void tbLogin_Click(object sender, EventArgs e)
      //{
      //  ServerConnection svrc = new ServerConnection(tbServer.Text);
      //  Server JobServer = new Server((cboServerList.Text);
      //}

      private void cmdLogin_Click(object sender, EventArgs e)
      {
         SrvC = new ServerConnection(cboServer.Text);
         LoadDatabases(SrvC);
      }

      private void LoadDatabases(ServerConnection SvrC)
      {
         srvr = new Server(SrvC);

         //foreach( Database db in srvr.Databases)
         //{
         if (!(srvr == null))
         {
            cboDatabases.SelectedIndexChanged -= cboDatabases_SelectedIndexChanged;
            cboDatabases.DataSource = GetInstanceDatabases(srvr.Name);
            cboDatabases.SelectedIndexChanged += cboDatabases_SelectedIndexChanged;
         }
      }

      public List<string> GetInstanceDatabases(string SQLSrvrInstance)
      {
         List<string> Databases = new List<string>();
         ServerConnection SQLSrvrConnection = new ServerConnection(SQLSrvrInstance);
         System.Collections.Specialized.StringCollection script = new StringCollection();

         // Fill in necessary information
         SQLSrvrConnection.ServerInstance = SQLSrvrInstance;

         // Setup capture and execute to be able to display script
         SQLSrvrConnection.SqlExecutionModes = SqlExecutionModes.ExecuteAndCaptureSql;

         // Do not Use Windows authentication
         SQLSrvrConnection.LoginSecure = false;
         SQLSrvrConnection.Password = tbPassword.Text;
         SQLSrvrConnection.Login = tbUser.Text;

         // Go ahead and connect
         srvr = new Server(SQLSrvrConnection);
         SQLSrvrConnection.Connect();

         foreach (Database idb in srvr.Databases)
         {
            if (!idb.IsSystemObject)
            {
               Databases.Add(idb.Name);
            }
         }
         return Databases;
      }

      private void cboDatabases_SelectedIndexChanged(object sender, EventArgs e)
      {
         string Database = (string)cboDatabases.SelectedItem;

         if (!(string.IsNullOrEmpty(Database)))
         {
            GetTables(Database);
         }
      }

      public void GetTables(string db)
      {
         if (string.IsNullOrEmpty(db))
            return;

         cboTables.SelectedIndexChanged -= cboTables_SelectedIndexChanged;
         foreach (Table table in srvr.Databases[db].Tables)
         {
            cboTables.Items.Add(table.Name);
         }

         cboTables.SelectedIndexChanged += cboTables_SelectedIndexChanged;
      }

      private void cboTables_SelectedIndexChanged(object sender, EventArgs e)
      {

         return;
         //tbScripts.Text = string.Empty;
         //SMOFunctions SMO = new SMOFunctions();

         //tbScripts.Text = SMO.Script_Indexes(Srvr, (string)cboDatabases.SelectedItem, (string)cboTables.SelectedItem, string.Empty);
         //tbScripts.Margins[0].Width = 30;
         // Script_Indexes()
      }

      private void cmdScript_Indexes_Click(object sender, EventArgs e)
      {
         string SQLSrvrInstance = cboServer.SelectedText;
         // ServerConnection SQLSrvrConnection = new ServerConnection(SQLSrvrInstance);
         System.Collections.Specialized.StringCollection script = new StringCollection();

         string Database = cboDatabases.SelectedValue.ToString();
         string dbTable = cboTables.SelectedItem.ToString();
         Table _dbTable;

         if (string.IsNullOrWhiteSpace(Database) || string.IsNullOrWhiteSpace(dbTable))
            return;

         tbScripts.Text = string.Empty;

         // Recreate connection if necessary
         //if (SQLSrvrInstance == null)
         //{
         //   SQLSrvrConnection = new ServerConnection();
         //}

         // Fill in necessary information
         //SQLSrvrConnection.ServerInstance = SQLSrvrInstance;

         //// Setup capture and execute to be able to display script
         //SQLSrvrConnection.SqlExecutionModes = SqlExecutionModes.ExecuteAndCaptureSql;

         //// Use Windows authentication
         //SQLSrvrConnection.LoginSecure = true;

         // Go ahead and connect
         //srvr = new Server(SQLSrvrConnection);
         //SQLSrvrConnection.Connect();

         ScriptingOptions oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = true;

         //         Table SelectedTable = srv.Databases[Database].Tables[dbTable, "dbo"];
         //IEnumerator TIEnumerator = srv.Databases[Database].Tables.GetEnumerator();
         //         Table SDBTable = TIEnumerator.find
         // TableCollection _table = srvr.Databases[Database].Tables;
         _dbTable = srvr.Databases[Database].Tables[dbTable, "dbo"];

         foreach (Index idx in _dbTable.Indexes)
         {
            script = idx.Script(oScriptingOptions);
            foreach (String str in script)
            {
               tbScripts.Text += str;
            }
         }
      }

      private void cmdFKeys_Click(object sender, EventArgs e)
      {
         //string SQLSrvrInstance = cboServer.SelectedText;
         //ServerConnection SQLSrvrConnection = new ServerConnection(SQLSrvrInstance);
         System.Collections.Specialized.StringCollection script = new StringCollection();

         string Database = cboDatabases.SelectedValue.ToString();
         string dbTable = cboTables.SelectedItem.ToString();
         Table _dbTable;

         if (string.IsNullOrWhiteSpace(Database) || string.IsNullOrWhiteSpace(dbTable))
            return;

         tbScripts.Text = string.Empty;

         ScriptingOptions oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         _dbTable = srvr.Databases[Database].Tables[dbTable, "dbo"];

         foreach (ForeignKey fk in _dbTable.ForeignKeys)
         {
            script = fk.Script(oScriptingOptions);
            foreach (String str in script)
            {
               if (str.Contains("USE ["))
               {
                  tbScripts.Text += Environment.NewLine + str + Environment.NewLine;
               }
               else
               {
                  tbScripts.Text += str + Environment.NewLine;
               }
            }
         }
      }

      public Server EstablishDBConnection(string DBInstance, string UserName, string Password)
      {
         ServerConnection SQLSrvrConnection = new ServerConnection(DBInstance);
         System.Collections.Specialized.StringCollection script = new StringCollection();

         // Fill in necessary information
         SQLSrvrConnection.ServerInstance = DBInstance;

         // Setup capture and execute to be able to display script
         SQLSrvrConnection.SqlExecutionModes = SqlExecutionModes.ExecuteAndCaptureSql;

         // Do not Use Windows authentication
         SQLSrvrConnection.LoginSecure = false;
         SQLSrvrConnection.Login = UserName;
         SQLSrvrConnection.Password = Password;

         // Go ahead and connect
         srvr = new Server(SQLSrvrConnection);
         SQLSrvrConnection.Connect();

         return srvr;
      }

      private void cboTables_KeyUp(object sender, KeyEventArgs e)
      {
         string Database = cboDatabases.SelectedValue.ToString();
         TableCollection _dbTables = srvr.Databases[Database].Tables;

         List<string> _dbTableList = new List<string>();

         if (((ComboBox)sender).Text.Length >= 2)
         {
            foreach (Table t in _dbTables)
            {
               if (t.Name.ToUpper().StartsWith(((ComboBox)sender).Text.ToUpper()))
               {
                  _dbTableList.Add(t.Name);
               }
            }
            cboTables.DataSource = _dbTableList;
         }
         cboTables.DroppedDown = true;
      }
   }
      public class SQLServerItems
      {
         public string Server_Name
         {
            get;
            set;
         }
      }

      public class SQLTableDatabaseClass
      {
         #region Database
         public static TableCollection GetTablesFromDatabase(string connectionString, string databaseName)
         {
            Database db = SinglDatabase(connectionString, databaseName);
            return db.Tables;
         }
         public static Table SingleTable(string connectionString, string databaseName, string tableName)
         {
            TableCollection tableCol = GetTablesFromDatabase(connectionString, databaseName);
            return tableCol[tableName];
         }
         public static Database SinglDatabase(string connectionString, string databaseName)
         {
            return GetServer(connectionString).Databases[databaseName];
         }
         #endregion
         public static Server GetServer(string connectionString)
         {
            Server server = new Server(GetServerConnection(connectionString));
            return server;
         }
         public static ServerConnection GetServerConnection(string connectionString)
         {
            ServerConnection serverCon = new ServerConnection(Connection(connectionString));
            serverCon.Connect();
            return serverCon;
         }
         public static ServerConnection GetServerConnectionByLogin(bool isWindows, string serverName)
         {
            ServerConnection serverCon = new ServerConnection();
            serverCon.LoginSecure = isWindows;
            serverCon.ServerInstance = serverName;
            serverCon.Connect();
            return serverCon;
         }
         public static SqlConnection Connection(string connectionString)
         {
            SqlConnection con = new SqlConnection(connectionString);
            return con;
         }
      }

   }

