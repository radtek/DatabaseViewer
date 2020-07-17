using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMO_DB_Viewer
{
   public partial class EventLogBackup : Form
   {
      public EventLogBackup()
      {
         InitializeComponent();
      }

      private void cmdGO_Click(object sender, EventArgs e)
      {
         BackupEventLog BEL = new SMO_DB_Viewer.BackupEventLog();
         BEL.BackupRemoteEventLog(tbComputerName.Text, tbUserName.Text, tbPassword.Text);

      }

      private void closeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Close();
      }
   }
}
