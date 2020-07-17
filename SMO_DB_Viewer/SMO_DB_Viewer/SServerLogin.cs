using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace SMO_DB_Viewer
{
   public partial class SServerLogin : Form
   {
      Server srvr;

      public SServerLogin()
      {
         InitializeComponent();
      }

      private void cmdSSInstanceLogin_Click(object sender, EventArgs e)
      {
         srvr = EstablishDBConnection(tbInstance.Text, tbLogin.Text, tbPassword.Text);
         if (srvr != null)
         {
            LoadDatabases(srvr);
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

         return srvr;
      }

      private void cboDatabases_SelectedIndexChanged(object sender, EventArgs e)
      {
         SSOB _ssob = new SSOB();
         _ssob.UserID = tbLogin.Text;
         _ssob.Password = tbPassword.Text;
         _ssob.SelectedDB = ((ComboBox)sender).SelectedItem.ToString();
         _ssob.SelectedDatabaseInstance = srvr;

         _ssob.Show(new WindowWrapper(this.Handle));
      }

      public void LoadDatabases(Server _srvr)
      {
         //  List<string> ListOProcedures = null;
         cboDatabases.Items.Clear();
         foreach (Database idb in _srvr.Databases)
         {
            if (!idb.IsSystemObject)
            {
               cboDatabases.Items.Add(idb.Name);
            }
         }
         if (cboDatabases.Items.Count > 0)
         {
            cboDatabases.DroppedDown = true;
         }
      }

      private void closeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void TSMIEventLogBackup_Click(object sender, EventArgs e)
      {
         EventLogBackup ELB = new EventLogBackup();
         ELB.Show();
      }
   }
}

public class WindowWrapper : System.Windows.Forms.IWin32Window
{
   private IntPtr _hwnd;

   public WindowWrapper(IntPtr handle)
   {
      _hwnd = handle;
   }

   public IntPtr Handle
   {
      get { return _hwnd; }
   }
}