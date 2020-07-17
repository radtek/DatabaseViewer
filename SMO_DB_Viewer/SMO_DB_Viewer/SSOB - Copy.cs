using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.MessageBox;

using Londeck.Net.Data;
using ScintillaNet;

/*
 * https://msdn.microsoft.com/en-us/library/system.windows.forms.treeview.beforeexpand.aspx?f=255&MSPPError=-2147217396
 * https://www.c-sharpcorner.com/UploadFile/c5c6e2/populate-a-treeview-dynamically/
 * 
 * Install-Package Microsoft.SqlServer.SqlManagementObjects
 */

namespace SMO_DB_Viewer
{
   public partial class SSOB : Form
   {
      public ServerConnection SrvC;
      public Server srvr;

      public delegate void AddtvNode_Delegate(Control ctl, TreeNode STProcNode, string NodeText);
      public delegate List<string> GetProcedures_Delegate(string db);
      public delegate List<UserDefProcedures> GetUserSTProcs_Delegate(string DBName);
      public delegate List<UserDefProcedures> GetUserDefViews_Delegate(string DBName);


      public SSOB()
      {
         InitializeComponent();
      }


      #region Properties

      public string UserID
      { get; set; }
      public string Password
      { get; set; }

      public string SelectedDB
      {
         get;
         set;
      }
    public Server SelectedDatabaseInstance
      {
         get
         {
            return srvr;
         }
         set
         {
            srvr = value;
            // GetInstanceDBObjects(srvr, SelectedDB);

         }
      }

      #endregion


      #region Form Events
      private void SSOB_Load(object sender, EventArgs e)
      {
         tbScripts.Margins[0].Width = 33;
      }
      
      private void SSOB_Shown(object sender, EventArgs e)
      {
         TreeNode treeNode = new TreeNode(srvr.Name);
         tvSQLServerObjs.Nodes.Add(treeNode);

         GetInstanceDBObjects(srvr, SelectedDB);
      }

      private void cmdSSInstanceLogin_Click(object sender, EventArgs e)
      {
         Server srvr = EstablishDBConnection(tbInstance.Text, tbLogin.Text, tbPassword.Text);
         if (srvr != null)
         {
            LoadDatabases(srvr);
         }
         // GetInstanceDatabases(_srvr);
      }

      private void closeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void cboDatabases_SelectedIndexChanged(object sender, EventArgs e)
      {
         SelectedDB = ((ComboBox)sender).SelectedItem.ToString();
         GetInstanceDBObjects(srvr, SelectedDB);
      }

      private void tvSQLServerObjs_AfterSelect(object sender, TreeViewEventArgs e)
      {
         TreeNode SelectedNode = e.Node;

         switch (SelectedNode.Level)
         {
            //case 2:
            //   if (SelectedNode.Text == "User Views")
            //   {

            //   }


            case 3:
               if (SelectedNode.Level == 3 && SelectedNode.Parent.Text == "Tables")
               {
                  if (SelectedNode.FirstNode == null)
                  {
                     SelectedNode.Nodes.Add("Foreign Keys");
                     SelectedNode.Nodes.Add("Indexes");
                     SelectedNode.Nodes.Add("Triggers");
                  }
               }

               if (SelectedNode.Level == 3 && SelectedNode.Parent.Text == "Stored Procedures")
               {
                  if (SelectedNode.FirstNode == null)
                  {
                     Script_Procedures(SelectedNode);
                  }
               }

               if (SelectedNode.Level == 3 && SelectedNode.Parent.Text == "User Views")
               {
                  if (SelectedNode.FirstNode == null)
                  {
                     Script_UserDefView(SelectedNode);
                  }
               }
               break;


            case 4:
               if (SelectedNode.Text == "Foreign Keys")
               {
                  Script_ForeignKeys(SelectedNode);
               }

               if (SelectedNode.Text == "Indexes")
               {
                  Script_Indexes(SelectedNode);
               }

               if (SelectedNode.Text == "Triggers")
               {
                  Script_Triggers(SelectedNode);
               }

               if (SelectedNode.Parent.Text == "Stored Procedures")
               {
                  Script_Procedures(SelectedNode);
               }
               break;
         }
      }

      private void tvSQLServerObjs_AfterExpand(object sender, TreeViewEventArgs e)
      {
         CurrentNode = e.Node;
      }

      private void cmdFindNode_Click(object sender, EventArgs e)
      {
         TreeNode FoundNode = CurrentNode.FindtvNode(tbFindNode.Text);
         if (FoundNode != null)
         {
            tvSQLServerObjs.SelectedNode = FoundNode;
         }
      }
      #endregion

      #region SMO DB Objects
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

         TreeNode treeNode = new TreeNode(DBInstance);
         tvSQLServerObjs.Nodes.Add(treeNode);
         return srvr;
      }

      private SqlConnection GetDBConnection(string DBName)
      {
         SqlConnection conn = new SqlConnection();
         if (!(string.IsNullOrEmpty(SelectedDB )))
         {
            conn.ConnectionString =
              "Data Source=" + tbInstance.Text + ";"
            + "Initial Catalog=" + DBName + ";"
            + "User id=" + this.UserID + ";" 
            + "Password=" + this.Password + ";";
            //conn.Open();
         }
         else
         {
            conn = null;
         }
         return conn;
      }

      public void GetTables(string db)
      {
         if (string.IsNullOrEmpty(db))
            return;

         // TreeNode DBNode = FindtvNode(tvSQLServerObjs.TopNode, db);
         TreeNode MyDBNode = tvSQLServerObjs.TopNode;
         TreeNode DBNode = MyDBNode.FindtvNode(db);
         DBNode.Nodes.Add("Tables");

         // Get the newly created node
         TreeNode TBLNode = DBNode.LastNode;
         foreach (Table table in srvr.Databases[db].Tables)
         {
            TBLNode.Nodes.Add(table.Name);
         }
      }

      public void LoadDatabases(Server _srvr)
      {
         // TreeNode Parent = tvSQLServerObjs.TopNode;
         //         List<string> ListOProcedures = null;
         foreach (Database idb in _srvr.Databases)
         {
            if (!idb.IsSystemObject)
            {
               cboDatabases.Items.Add(idb.Name);
            }
         }
      }

      public void GetInstanceDBObjects(Server _srvr, string DBName)
      {
         TreeNode Parent = tvSQLServerObjs.TopNode;
         //         List<string> ListOProcedures = null;
         Database idb = _srvr.Databases[DBName];

         Parent.Nodes.Add(DBName);
         GetTables(DBName);
//******
         GetUserSTProcs_Delegate GetProcs = GetUserSTProcs;
         IAsyncResult STProc_ar = GetProcs.BeginInvoke(DBName, null, null);
         //while (!ar.IsCompleted)
         //{
         //   // 10 Second sleep
         //   Thread.Sleep(1000);
         //}
         List<UserDefProcedures> STProcs = GetProcs.EndInvoke(STProc_ar);
         if (STProcs != null)
         {
            AddSTProcsToTree(STProcs, DBName);
         }
//******
         GetUserDefViews_Delegate GetViews = GetUserDefViews;
         IAsyncResult DBView_ar = GetViews.BeginInvoke(DBName, null, null);
         //while (!ar.IsCompleted)
         //{
         //   // 10 Second sleep
         //   Thread.Sleep(1000);
         //}
         List<UserDefProcedures> UserDefViews = GetViews.EndInvoke(DBView_ar);
         if (UserDefViews != null)
         {
            AddUserDefViewsToTree(UserDefViews, DBName);
         }
//******
         /*       GetProcedures_Delegate GetProcs = GetProcedures_Threaded;
                  IAsyncResult ar = GetProcs.BeginInvoke(DBName, null, null);
                  while (!ar.IsCompleted)
                  {
                     // 10 Second sleep
                     Thread.Sleep(1000);
                  }
                  List<string> STProcs = GetProcs.EndInvoke(ar);
                  AddSTProcsToTree(STProcs, DBName);
          */
         /*
            * This threading block blows up the whole method.
            * */
         /*
            List<string> STProcs = new List<string>();
            Thread CollectProcedures =
               new Thread(() =>
               STProcs = GetProcedures_Threaded(idb.Name));

            CollectProcedures.Priority = ThreadPriority.Highest;
            bool APTStateChanged = CollectProcedures.TrySetApartmentState(ApartmentState.STA);
            if (APTStateChanged)
            {
               CollectProcedures.Start();
            }
            AddSTProcsToTree(STProcs, idb.Name);
            */
      }

      public void GetProcedures(string db)
      {
         if (string.IsNullOrEmpty(db))
            return;

         //         TreeNode DBNode = FindtvNode(tvSQLServerObjs.TopNode, db);
         TreeNode MyDBNode = tvSQLServerObjs.TopNode;
         TreeNode DBNode = MyDBNode.FindtvNode(db);
         DBNode.Nodes.Add("Stored Procedures");

         // Get the newly created node
         TreeNode SPROCNode = DBNode.LastNode;
         // List<string> UserStoredProcedures = new List<string>();
         foreach (StoredProcedure SProc in srvr.Databases[db].StoredProcedures)
         {
            if (!SProc.IsSystemObject)
            {
               //UserStoredProcedures.Add(SProc.Name);
               SPROCNode.Nodes.Add(SProc.Name);

               //AddtvNode_Delegate STProc = AddtvNode;
               //IAsyncResult ar = STProc.BeginInvoke(tvSQLServerObjs, SPROCNode, SProc.Name, null, null);

               //while (!ar.IsCompleted)
               //{  // 1 Second sleep.
               //   Thread.Sleep(1000);
               //}
            }
         }
         //         return UserStoredProcedures;
      }

      public List<string> GetProcedures_Threaded(string db)
      {
         if (string.IsNullOrEmpty(db))
            return null;

         List<string> ListOProcedures = new List<string>();

         ////         TreeNode DBNode = FindtvNode(tvSQLServerObjs.TopNode, db);
         //TreeNode MyDBNode = tvSQLServerObjs.TopNode;
         //TreeNode DBNode = MyDBNode.FindtvNode(db);
         //DBNode.Nodes.Add("Stored Procedures");

         //// Get the newly created node
         //TreeNode SPROCNode = DBNode.LastNode;
         // List<string> UserStoredProcedures = new List<string>();
         foreach (StoredProcedure SProc in srvr.Databases[db].StoredProcedures)
         {
            if (!SProc.IsSystemObject)
            {
               // UserStoredProcedures.Add(SProc.Name);
               // SPROCNode.Nodes.Add(SProc.Name);
               ListOProcedures.Add(SProc.Name);

               //AddtvNode_Delegate STProc = AddtvNode;
               //IAsyncResult ar = STProc.BeginInvoke(tvSQLServerObjs, SPROCNode, SProc.Name, null, null);

               //while (!ar.IsCompleted)
               //{  // 1 Second sleep.
               //   Thread.Sleep(1000);
               //}
            }
         }
         return ListOProcedures;
      }
      
      public List<string> GetUserViews_Threaded(string db)
      {
         if (string.IsNullOrEmpty(db))
            return null;

         List<string> ListOUserViews = new List<string>();

         //// TreeNode DBNode = FindtvNode(tvSQLServerObjs.TopNode, db);
         //TreeNode MyDBNode = tvSQLServerObjs.TopNode;
         //TreeNode DBNode = MyDBNode.FindtvNode(db);
         //DBNode.Nodes.Add("Stored Procedures");

         //// Get the newly created node
         //TreeNode SPROCNode = DBNode.LastNode;
         // List<string> UserStoredProcedures = new List<string>();
         foreach (StoredProcedure UView in srvr.Databases[db].Views)
         {
            if (!UView.IsSystemObject)
            {
               // UserStoredProcedures.Add(SProc.Name);
               // SPROCNode.Nodes.Add(SProc.Name);
               ListOUserViews.Add(UView.Name);

               //AddtvNode_Delegate STProc = AddtvNode;
               //IAsyncResult ar = STProc.BeginInvoke(tvSQLServerObjs, SPROCNode, SProc.Name, null, null);

               //while (!ar.IsCompleted)
               //{  // 1 Second sleep.
               //   Thread.Sleep(1000);
               //}
            }
         }
         return ListOUserViews;
      }
      
      public void AddSTProcsToTree(List<string> STProcedures, string db)
      {
         ////         TreeNode DBNode = FindtvNode(tvSQLServerObjs.TopNode, db);
         TreeNode MyDBNode = tvSQLServerObjs.TopNode;
         TreeNode DBNode = MyDBNode.FindtvNode(db);
         DBNode.Nodes.Add("Stored Procedures");

         //// Get the newly created node
         TreeNode SPROCNode = DBNode.LastNode;
         // List<string> UserStoredProcedures = new List<string>();
         foreach (string SProc in STProcedures)
         {
            SPROCNode.Nodes.Add(SProc);
         }
      }

      public void AddSTProcsToTree(List<UserDefProcedures> STProcedures, string db)
      {
         ////         TreeNode DBNode = FindtvNode(tvSQLServerObjs.TopNode, db);
         TreeNode MyDBNode = tvSQLServerObjs.TopNode;
         TreeNode DBNode = MyDBNode.FindtvNode(db);
         DBNode.Nodes.Add("Stored Procedures");

         //// Get the newly created node
         TreeNode SPROCNode = DBNode.LastNode;
         // List<string> UserStoredProcedures = new List<string>();
         foreach (UserDefProcedures SProc in STProcedures)
         {
            SPROCNode.Nodes.Add(SProc.Name);
         }
      }

      public void AddUserDefViewsToTree(List<UserDefProcedures> UserViews, string db)
      {
         ////         TreeNode DBNode = FindtvNode(tvSQLServerObjs.TopNode, db);
         TreeNode MyDBNode = tvSQLServerObjs.TopNode;
         TreeNode DBNode = MyDBNode.FindtvNode(db);
         DBNode.Nodes.Add("User Views");

         //// Get the newly created node
         TreeNode UserViewsNode = DBNode.LastNode;
         // List<string> UserStoredProcedures = new List<string>();
         foreach (UserDefProcedures SProc in UserViews)
         {
            UserViewsNode.Nodes.Add(SProc.Name);
         }
      }
      
      #endregion

      #region ADO 
      private List<UserDefProcedures> GetUserSTProcs(string DBName)
      {
         ExecuteStoredProcedure _esp = new ExecuteStoredProcedure();

         List<string> UserDefProcedures = new List<string>();
         string CommandText = 
            "SELECT "
            + "  Object_id, "
            + "  Name "
            + "FROM sys.procedures "
            + "WHERE is_ms_shipped = 0 "
            + "ORDER BY Name ";

         SqlConnection conn = GetDBConnection(DBName);

         if (conn == null)
            return null;

         List<UserDefProcedures> _UserDefStProcedures = new List<UserDefProcedures>();
         DataSet UserDefStProcedures = _esp.ExecuteSQLTextDS(CommandText, conn, "StoredProcedures");
         _UserDefStProcedures = Db.FillGenObjectDS<UserDefProcedures, DataSet>(new UserDefProcedures(), UserDefStProcedures);

         if( conn.State == ConnectionState.Open)
            conn.Close();

         return _UserDefStProcedures;
      }

      private List<UserDefProcedures> GetUserDefViews(string DBName)
      {
         ExecuteStoredProcedure _esp = new ExecuteStoredProcedure();

         List<UserDefProcedures> _UserDefViews = new List<UserDefProcedures>();
         string CommandText =
            "SELECT "
            + "  Object_id, "
            + "  Name "
            + "FROM sys.views "
            + "WHERE is_ms_shipped = 0 "
            + "ORDER BY Name ";

         SqlConnection conn = GetDBConnection(DBName);

         if (conn == null)
            return null;

         List<UserDefProcedures> _UserDefStProcedures = new List<UserDefProcedures>();
         DataSet UserDefViews = _esp.ExecuteSQLTextDS(CommandText, conn, "UserDefViews");
         _UserDefViews = Db.FillGenObjectDS<UserDefProcedures, DataSet>(new UserDefProcedures(), UserDefViews);

         if (conn.State == ConnectionState.Open)
            conn.Close();

         return _UserDefViews;
      }

      #endregion

      #region Script DB Objects
      private void Script_ForeignKeys(TreeNode TableNode)
      {
         System.Collections.Specialized.StringCollection script = new StringCollection();

         string Database = WalkBack(TableNode, 3);
         string dbTable = TableNode.Parent.Text;
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

      private void Script_Indexes(TreeNode TableNode)
      {
         System.Collections.Specialized.StringCollection script = new StringCollection();

         //         string Database = TableNode.Parent.Parent.Parent.Text;
         string Database = WalkBack(TableNode, 3);
         string dbTable = TableNode.Parent.Text;

         if (string.IsNullOrWhiteSpace(Database) || string.IsNullOrWhiteSpace(dbTable))
            return;

         tbScripts.Text = string.Empty;

         ScriptingOptions oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         //         Table SelectedTable = srv.Databases[Database].Tables[dbTable, "dbo"];
         //IEnumerator TIEnumerator = srv.Databases[Database].Tables.GetEnumerator();
         //         Table SDBTable = TIEnumerator.find
         // TableCollection _table = srvr.Databases[Database].Tables;
         Table _dbTable = srvr.Databases[Database].Tables[dbTable, "dbo"];

         foreach (Index idx in _dbTable.Indexes)
         {
            script = idx.Script(oScriptingOptions);
            foreach (String str in script)
            {
               if (str.ToUpper().Contains("USE ["))
               { // Environment.NewLine + 
                  tbScripts.Text += Environment.NewLine + str + Environment.NewLine;
               }
               else
               {
                  tbScripts.Text += str + Environment.NewLine;
               }
            }
         }
      }

      private void Script_Procedures(TreeNode STProcNode)
      {
         System.Collections.Specialized.StringCollection script = new StringCollection();

         string _Database = WalkBack(STProcNode, 2);
         //string dbTable = TableNode.Parent.Text;
         StoredProcedure _dbProcedure;

         Database MyDB = srvr.Databases[_Database];

         if (string.IsNullOrWhiteSpace(_Database))
            return;

         tbScripts.Text = string.Empty;

         ScriptingOptions oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         //         _dbProcedure = srvr.Databases[Database].StoredProcedures[STProcNode.Text];
         _dbProcedure = (StoredProcedure)MyDB.StoredProcedures[STProcNode.Text];
         //foreach (StoredProcedure procedure in _dbProcedures)
         //{
         script = _dbProcedure.Script(oScriptingOptions);
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
         //}
      }

      private void Script_Triggers(TreeNode TableNode)
      {
         System.Collections.Specialized.StringCollection script = new StringCollection();

         string Database = WalkBack(TableNode, 3);
         string dbTable = TableNode.Parent.Text;
         Table _dbTable;

         if (string.IsNullOrWhiteSpace(Database) || string.IsNullOrWhiteSpace(dbTable))
            return;

         tbScripts.Text = string.Empty;

         ScriptingOptions oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         _dbTable = srvr.Databases[Database].Tables[dbTable, "dbo"];

         foreach (Trigger trigger in _dbTable.Triggers)
         {
            script = trigger.Script(oScriptingOptions);
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

      private void Script_UserDefView(TreeNode ViewNode)
      {
         System.Collections.Specialized.StringCollection script = new StringCollection();

         string Database = WalkBack(ViewNode, 2);
         string dbTable = ViewNode.Parent.Text;
         Microsoft.SqlServer.Management.Smo.View _dbView = new Microsoft.SqlServer.Management.Smo.View();

         if (string.IsNullOrWhiteSpace(Database) || string.IsNullOrWhiteSpace(dbTable))
            return;

         tbScripts.Text = string.Empty;

         ScriptingOptions oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;

         _dbView = srvr.Databases[Database].Views[ViewNode.Text];

         //foreach (Microsoft.SqlServer.Management.Smo.View View in srvr.Databases[Database].Views)
         //{
            script = _dbView.Script(oScriptingOptions);
         foreach (string str in script)
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
         FormatSQL(tbScripts.Text);
         //}
      }

      private void FormatSQL(string SQLtoFormat)
      {
         string[] StuffBefore = { "SELECT", "FROM", "WHERE", "ON", "LEFT JOIN", "RIGHT JOIN", "RIGHT OUTER JOIN", "CREATE", "LEFT OUTER JOIN", "ORDER BY", "GROUP BY", "USE [" };
         string[] StuffAfter = { "SELECT", "," };

         foreach ( string TexttoFormat in StuffBefore)
         {
            SQLtoFormat = SQLtoFormat.StuffAt(TexttoFormat, Environment.NewLine);
         }

         foreach (string TexttoFormat in StuffAfter)
         {
            SQLtoFormat = SQLtoFormat.StuffAfter(TexttoFormat, Environment.NewLine);
         }
         tbScripts.Text = SQLtoFormat;
      }

      #endregion

      #region Helper Functions
      public void AddtvNode(Control ctl, TreeNode STProcNode, string NodeText)
      {
         // STProcNode.Nodes.Add(NodeText);
         if (ctl.InvokeRequired)
         {
            // BeginInvoke(new ShowTimerEventFiredDelegate(ShowTimerEventFired), new object[] { ts });
            ctl.Invoke(new AddtvNode_Delegate(AddtvNode), new object[] { STProcNode, NodeText });
         }
         else
         {
            STProcNode.Nodes.Add(NodeText);
         }

      }

      public string WalkBack(TreeNode BaseNode, int LevelsBack)
      {
         TreeNode ReturnNode;
         if (BaseNode.Level < LevelsBack)
            return null;

         ReturnNode = BaseNode.Parent;
         if (ReturnNode.Level != BaseNode.Level - LevelsBack)
         {
            // Go Back 1 more
            do
            {
               ReturnNode = ReturnNode.Parent;
            }
            while (ReturnNode.Level != BaseNode.Level - LevelsBack);
         }
         return ReturnNode.Text;
      }
      #endregion

      private TreeNode CurrentNode
      { get; set; }

   }

   public static class TreeViewHelpers
   {
     #region Treeview Helpers
     public static TreeNode FindtvNode(this TreeNode motherNode, string findNodeText)
      {
         // List<TreeNode> nodeList = new List<TreeNode>();
         TreeNode DBNode = null;
         foreach (TreeNode childNode in motherNode.Nodes)
         {
            if (childNode.Text.Equals(findNodeText, StringComparison.CurrentCultureIgnoreCase))
            {
               DBNode = childNode;
            }
            // nodeList.Add(childNode);
         }
         // return nodeList.ToArray<TreeNode>();
         return DBNode;
      }

     #endregion
   }

   public static class Extension
   {
      public static Form CenterForm(this Form child, Form parent)
      {
         child.StartPosition = FormStartPosition.Manual;
         child.Location = new Point(parent.Location.X + (parent.Width - child.Width) / 2, parent.Location.Y + (parent.Height - child.Height) / 2);
         return child;
      }
   }

   public class UserDefProcedures
   {
      public int Object_id { get; set; }

      public string Name { get; set; }
   }
}