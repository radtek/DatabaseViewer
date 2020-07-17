using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
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
      public delegate List<UserDefViews> GetUserDefViews_Delegate(string DBName);

      public SSOB()
      {
         InitializeComponent();
      }

#region Properties
      public string UserID { get; set; }

      public string Password { get; set; }

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

      private TreeNode Context_Node { get; set; }

      private TreeNode CurrentNode { get; set; }
#endregion

#region Form Events
      private void SSOB_Load(object sender, EventArgs e)
      {
         tbScripts.Margins[0].Width = 33;
         tbScripts.Indentation.ShowGuides = true;
         tbScripts.Indentation.IndentWidth = 2;
         tbScripts.Indentation.TabIndents = true;
         tbScripts.Indentation.UseTabs = false;        
      }

      private void SSOB_Shown(object sender, EventArgs e)
      {
         TreeNode treeNode = new TreeNode(srvr.Name);
         tvSQLServerObjs.Nodes.Add(treeNode);

         GetInstanceDBObjects(srvr, SelectedDB);
      }

      private void tvSQLServerObjs_DragEnter(object sender, DragEventArgs e)
      {
         e.Effect = DragDropEffects.Copy;
      }

      private void tvSQLServerObjs_ItemDrag(object sender, ItemDragEventArgs e)
      {
         DoDragDrop(e.Item, DragDropEffects.Copy);
      }

      private void tvSQLServerObjs_MouseHover(object sender, EventArgs e)
      {
         TreeNode selNode = (TreeNode)tvSQLServerObjs.GetNodeAt(tvSQLServerObjs.PointToClient(Cursor.Position));
         if (selNode == null || selNode == tvSQLServerObjs.TopNode)
            return;

         if (selNode.Parent.Text == "Tables")
         {
            Point mousePos = tvSQLServerObjs.PointToClient(MousePosition);
            if (selNode.Bounds.Contains(mousePos))
            {
               //  ToolTip RowCount
               toolTip1.IsBalloon = true;
               
               // Node location in treeView coordinates.
               Point loc = selNode.Bounds.Location;

               // Node location in form client coordinates.
               loc.Offset(tvSQLServerObjs.Location);

               // Make balloon point to upper right corner of the node.
               loc.Offset(selNode.Bounds.Width - 7, -12);

               toolTip1.Show(GetRowCount(selNode.Text), tvSQLServerObjs, loc.X, loc.Y);
               // toolTip1.SetToolTip(tvSQLServerObjs, );
            }
         }
      }

      private void tvSQLServerObjs_MouseLeave(object sender, EventArgs e)
      {
         toolTip1.Hide(this);
      }
      
      private void tvSQLServerObjs_AfterExpand(object sender, TreeViewEventArgs e)
      {
         CurrentNode = e.Node;
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
               docMenu = new ContextMenuStrip();

               if (SelectedNode.Level == 3 && SelectedNode.Parent.Text == "Tables")
               {
                  if (SelectedNode.FirstNode == null)
                  {
                     SelectedNode.Nodes.Add("Columns");
                     SelectedNode.Nodes.Add("Foreign Keys");
                     SelectedNode.Nodes.Add("Indexes");
                     SelectedNode.Nodes.Add("Triggers");
                  }
                  TreeNode Column_Node = SelectedNode.FindtvNode("Columns");
                  ToolStripMenuItem PrintColumns = new ToolStripMenuItem();
                  PrintColumns.Text = "Print Columns";
                  PrintColumns.Click += new System.EventHandler(this.tsmi_PrintColumns_Click);

                  //Add the menu items to the menu.
                  docMenu.Items.Add(PrintColumns);
                  Context_Node = Column_Node;
                  Column_Node.ContextMenuStrip = docMenu;

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
               if (SelectedNode.Text == "Columns")
               {
                  List_TableColumns(SelectedNode);
               }

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

      private void sortResultsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         SortResults();
      }

      private void closeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void cmdClipBoard_Click(object sender, EventArgs e)
      {
         System.Windows.Forms.Clipboard.SetDataObject(this.tbScripts.Text);
      }

      private void cmdFindNode_Click(object sender, EventArgs e)
      {
         if (!(string.IsNullOrEmpty(tbFindNode.Text)))
         {
            TreeNode FoundNode = CurrentNode.FindtvNode(tbFindNode.Text);
            if (FoundNode == null)
            {
               FoundNode = CurrentNode.FindtvNodeContains(tbFindNode.Text);
            }

            if (FoundNode != null)
            {
               tvSQLServerObjs.SelectedNode = FoundNode;
            }
         }
      }

      private ContextMenuStrip docMenu;
      
      private void tsmi_PrintColumns_Click(object sender, EventArgs e)
      {
         tbScripts.Text = string.Empty;
         foreach (TreeNode column in Context_Node.Nodes)
         {
            tbScripts.Text += string.Format("{0}," + Environment.NewLine, column.Text);
         }
      }
      
      private void naturalSortToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Results_Natural_Sort();
      }

#endregion

#region SMO DB Objects
      public void AddSTProcsToTree(List<string> STProcedures, string db)
      {
         //// TreeNode DBNode = FindtvNode(tvSQLServerObjs.TopNode, db);
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

      public void AddUserDefViewsToTree(List<UserDefViews> UserViews, string db)
      {
         ////         TreeNode DBNode = FindtvNode(tvSQLServerObjs.TopNode, db);
         TreeNode MyDBNode = tvSQLServerObjs.TopNode;
         TreeNode DBNode = MyDBNode.FindtvNode(db);
         DBNode.Nodes.Add("User Views");

         //// Get the newly created node
         TreeNode UserViewsNode = DBNode.LastNode;
         // List<string> UserStoredProcedures = new List<string>();
         foreach (UserDefViews SProc in UserViews)
         {
            UserViewsNode.Nodes.Add(SProc.Name);
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
         SQLSrvrConnection.LoginSecure = true;
         if (SQLSrvrConnection.LoginSecure == false)
         {
            SQLSrvrConnection.Login = UserName;
            SQLSrvrConnection.Password = Password;
         }
         // Go ahead and connect
         srvr = new Server(SQLSrvrConnection);
         SQLSrvrConnection.Connect();

         TreeNode treeNode = new TreeNode(DBInstance);
         tvSQLServerObjs.Nodes.Add(treeNode);
         return srvr;
      }

      private SqlConnection GetDBConnection(string DBName )
      {
         SqlConnection conn = new SqlConnection();
         if (!(string.IsNullOrEmpty(SelectedDB)))
         {
            conn.ConnectionString =
              "Data Source=" + srvr.Name + ";"
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

      private SqlConnection GetDBConnection(string DBName, bool Trusted)
      {
         SqlConnection conn = new SqlConnection();
         if (!(string.IsNullOrEmpty(SelectedDB)))
         {
            conn.ConnectionString =
              "Data Source=" + srvr.Name + ";"
            + "Initial Catalog=" + DBName + ";"
            + "Trusted_Connection=True;";
            //+ "User id=" + this.UserID + ";"
            //+ "Password=" + this.Password + ";";
            conn.Open();
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

         while (!DBView_ar.IsCompleted)
         {
            // 10 Second sleep
            Thread.Sleep(1500);
         }

         List<UserDefViews> UserDefViews = GetViews.EndInvoke(DBView_ar);
         if (UserDefViews != null && UserDefViews.Count > 0)
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

      public void List_TableColumns(TreeNode Table)
      {
         ExecuteStoredProcedure _esp = new ExecuteStoredProcedure();
         SqlConnection conn;

         string TableName = Table.Parent.Text;
         string CommandText =
              "SELECT DISTINCT"
            + "  so.Name AS TableName, "       // TableName
            + "  sc.Name AS ColumnName, "      // Columns
            + "  st.name AS DataType, "        // Data Type
            + "  sc.[Length] AS Data_Length "  // Data Lengths
            + "FROM sys.sysobjects AS SO "
            + "JOIN sys.syscolumns AS SC "
            + " ON so.ID = SC.ID "
            + "JOIN sys.systypes AS ST "
            + "  ON sc.xtype = st.xtype "
            + "WHERE sc.ID = OBJECT_ID('" + TableName + "')"
            + "  AND st.Name != 'sysname';";

         List<TableColumnInfo> _TableColumns = new List<TableColumnInfo>();
         conn = GetDBConnection(SelectedDB, true);

         DataSet TableColumns = _esp.ExecuteSQLTextDS(CommandText, conn, "TableColumns");
         _TableColumns = Db.FillGenObjectDS<TableColumnInfo, DataSet>(new TableColumnInfo(), TableColumns);

         foreach (TableColumnInfo Column in _TableColumns)
         {
            Table.Nodes.Add(Column.ColumnName);
            Debug.WriteLine(Column.ColumnName);
         }
      }
#endregion

#region ADO

      public string GetRowCount(string TableName)
      {
         ExecuteStoredProcedure _esp = new ExecuteStoredProcedure();
         List<TableRowCount> _TableRowCount = new List<TableRowCount>();

         string CommandText =
           "SELECT "
         + "  SCHEMA_NAME(schema_id) AS [SchemaName], "
         + "  [Tables].name AS [TableName], "
         + "  CAST(SUM([Partitions].[rows]) AS INT) AS [TotalRowCount] "
         + "FROM sys.tables AS [Tables] "
         + "  JOIN sys.partitions AS [Partitions] "
         + "  ON [Tables].[object_id] = [Partitions].[object_id] "
         + "  AND [Partitions].index_id IN (0, 1) "
         + "WHERE [Tables].name = '" + TableName + "' "
         + "GROUP BY "
         + "  SCHEMA_NAME(schema_id), "
         + "  [Tables].name";

         SqlConnection conn = GetDBConnection(SelectedDB, true);
         if (conn == null)
            return null;

         DataSet DS_TableRowCount = _esp.ExecuteSQLTextDS(CommandText, conn, "TableRowCount");
         _TableRowCount = Db.FillGenObjectDS<TableRowCount, DataSet>(new TableRowCount(), DS_TableRowCount);

         if (conn.State == ConnectionState.Open)
            conn.Close();

         return string.Format("Table: {0} Rows: {1}", _TableRowCount[0].TableName.ToString(), _TableRowCount[0].TotalRowCount.ToString());
      }

      private List<UserDefProcedures> GetUserSTProcs(string DBName)
      {
         ExecuteStoredProcedure _esp = new ExecuteStoredProcedure();
         SqlConnection conn;

         string CommandText =
            "SELECT "
            + "  Object_id, "
            + "  Name "
            + "FROM sys.procedures "
            + "WHERE is_ms_shipped = 0 "
            + "ORDER BY Name ";

         //if(!(SelectedDatabaseInstance == null))
         //{
         //   conn = SelectedDatabaseInstance.ConnectionContext.SqlConnectionObject;
         //}
         //else
         //{
            conn = GetDBConnection(DBName, true);
         //}
         
         if (conn == null)
            return null;

         List<UserDefProcedures> _UserDefStProcedures = new List<UserDefProcedures>();
         DataSet UserDefStProcedures = _esp.ExecuteSQLTextDS(CommandText, conn, "StoredProcedures");
         _UserDefStProcedures = Db.FillGenObjectDS<UserDefProcedures, DataSet>(new UserDefProcedures(), UserDefStProcedures);

         if (conn.State == ConnectionState.Open)
            conn.Close();

         return _UserDefStProcedures;
      }

      private List<UserDefViews> GetUserDefViews(string DBName)
      {
         ExecuteStoredProcedure _esp = new ExecuteStoredProcedure();
         SqlConnection conn; // = GetDBConnection(DBName);

         string CommandText =
            "SELECT "
            + "  Object_id, "
            + "  Name "
            + "FROM sys.views "
            + "WHERE is_ms_shipped = 0 "
            + "ORDER BY Name ";

         //if (!(SelectedDatabaseInstance == null))
         //{
         //   conn = SelectedDatabaseInstance.ConnectionContext.SqlConnectionObject;
         //}
         //else
         //{
            conn = GetDBConnection(DBName, true);
         //}

         if (conn == null)
            return null;

         List<UserDefViews> _UserDefViews = new List<UserDefViews>();
         DataSet UserDefViewsDS = _esp.ExecuteSQLTextDS(CommandText, conn, "UserDefViews");
         _UserDefViews = Db.FillGenObjectDS<UserDefViews, DataSet>(new UserDefViews(), UserDefViewsDS);

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

         // string Database = TableNode.Parent.Parent.Parent.Text;
         string Database = WalkBack(TableNode, 3);
         string dbTable = TableNode.Parent.Text;

         if
            (
               string.IsNullOrWhiteSpace(Database) ||
               string.IsNullOrWhiteSpace(dbTable)
            )
            return;

         tbScripts.Text = string.Empty;

         ScriptingOptions oScriptingOptions = new ScriptingOptions();
         oScriptingOptions.IncludeDatabaseContext = true;
         oScriptingOptions.IncludeIfNotExists = false;
         oScriptingOptions.DriAllConstraints = true;

         // Table SelectedTable = srv.Databases[Database].Tables[dbTable, "dbo"];
         // IEnumerator TIEnumerator = srv.Databases[Database].Tables.GetEnumerator();
         // Table SDBTable = TIEnumerator.find
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
         FormatSQL(tbScripts.Text);
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
         /*
            string[] StuffBefore = { "SELECT ", " FROM ", "WHERE", " ON ", " INNER JOIN"," LEFT JOIN ", " RIGHT JOIN ", " RIGHT OUTER JOIN ", "CREATE ", " LEFT OUTER JOIN ", " ORDER BY ", "GROUP BY ", "USE [", " AND " };
            string[] StuffAfter = { "SELECT ", "," };

            foreach ( string TexttoFormat in StuffBefore)
            {
               if (SQLtoFormat.ToUpper().Contains(TexttoFormat))
               {
                  SQLtoFormat = SQLtoFormat.StuffAt(TexttoFormat, Environment.NewLine);
               }
            }

            foreach (string TexttoFormat in StuffAfter)
            {
               if (SQLtoFormat.ToUpper().Contains(TexttoFormat))
               {
                  SQLtoFormat = SQLtoFormat.StuffAfter(TexttoFormat, Environment.NewLine + "  ");
               }
            }
            SQLtoFormat = SQLtoFormat.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            SQLtoFormat = SQLtoFormat.Replace("  ", " ");
            SQLtoFormat = SQLtoFormat.Replace("\t", "  ");
         */

         //tbScripts.Text = SQLtoFormat;
         SQLTextFormat _SQLText = new SQLTextFormat(SQLtoFormat);
         tbScripts.Text = _SQLText.SQLText;
      }

      private void SortResults()
      {
         List<string> Results = new List<string>();
         string tbResults = tbScripts.Text;
         string LineOfText = string.Empty;
         //         StringDelimitedParser sdp = new StringDelimitedParser();
         System.IO.StringReader strReader = new System.IO.StringReader(tbScripts.Text);

         while (true)
         {
            LineOfText = strReader.ReadLine();
            if (string.IsNullOrEmpty(LineOfText))
            {
               break;
            }
            Results.Add(LineOfText);
         }

         var vResults =
            from vstring in Results
            orderby vstring ascending
            select vstring;

         tbScripts.Text = string.Empty;
         foreach (string s in vResults)
         {
            tbScripts.Text += string.Format("{0}" + Environment.NewLine, s);
         }
      }

      private void Results_Natural_Sort()
      {
         string[] InputString = new string[tbScripts.Lines.Count];
         ScintillaNet.LinesCollection input = tbScripts.Lines;
         input.CopyTo(InputString, 0);

         Array.Sort(InputString, new NaturalStringComparer(tbScripts.Text, true));

         tbScripts.Text = string.Empty;
         foreach (string s in InputString)
         {
            if (s.Length > 0)
            {
               //tbRefactoredResults.Text += s;
               if (!s.EndsWith(Environment.NewLine))
               {
                  tbScripts.Text += s + Environment.NewLine;
               }
               else
               {
                  tbScripts.Text += s;
               }
            }
         }
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

   }
   
   #region Treeview Helper extensions
   public static class TreeViewHelpers
   {
      public static TreeNode FindtvNode(this TreeNode motherNode, string findNodeText)
      {
         // List<TreeNode> nodeList = new List<TreeNode>();
         TreeNode DBNode = null;
         foreach(TreeNode childNode in motherNode.Nodes)
         {
            if (childNode.Text.Equals(findNodeText, StringComparison.CurrentCultureIgnoreCase))
            {
               DBNode = childNode;
            }
         }
         return DBNode;
      }

      public static TreeNode FindtvNodeContains(this TreeNode motherNode, string findNodeText)
      {
         // List<TreeNode> nodeList = new List<TreeNode>();
         TreeNode DBNode = null;
         foreach (TreeNode childNode in motherNode.Nodes)
         {
            if (childNode.Text.ToLower().Contains(findNodeText.ToLower()))
            {
               DBNode = childNode;
            }
         }
         return DBNode;
      }
   }
#endregion

   public static class Extension
   {
      public static Form CenterForm(this Form child, Form parent)
      {
         child.StartPosition = FormStartPosition.Manual;

         Point pt;
         pt = new Point(parent.Location.X + (parent.Width - child.Width) / 2, parent.Location.Y + (parent.Height - child.Height) / 2);

         if (pt.X < 0)
            pt.X = 0;

         if (pt.Y < 0)
            pt.Y = 0;

         child.Location = pt;
         return child;
      }
   }

   public class UserDefProcedures
   {
      public int Object_id { get; set; }

      public string Name { get; set; }
   }

   public class UserDefViews
   {
      public int Object_id { get; set; }

      public string Name { get; set; }
   }


   public class TableRowCount
   {
      public string SchemaName { get; set; }
      public string TableName { get; set; }
      public int TotalRowCount { get; set; }
   }

   public class TableColumnInfo
   {
      public string ColumnName { get; set; }
      public string TableName { get; set; }
      public int Data_Length { get; set; }
      public string DataType { get; set; }
   }
}