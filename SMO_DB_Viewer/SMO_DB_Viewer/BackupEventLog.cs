using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SMO_DB_Viewer
{
   public class BackupEventLog
   {
      public void BackupRemoteEventLog(string IPAddress, string UserName, string Password)
      {
         try
         {
            ManagementObjectSearcher searcher;
            ObjectQuery query;
            ConnectionOptions connection;
            ManagementScope scope = null;

            if ((UserName != "") && (Password != ""))
            {
               connection = new ConnectionOptions();
               connection.Username = UserName;
               connection.Password = Password;
               connection.Authentication = AuthenticationLevel.Connect;
               connection.Authority = "ntlmdomain:HTICYBERNETICS";

               scope = new ManagementScope("\\\\" + IPAddress + "\\root\\CIMV2", connection);
               scope.Connect();
            }
            else
            {
               scope = new ManagementScope();
            }
            scope.Options.EnablePrivileges = true;
            scope.Options.Impersonation = ImpersonationLevel.Impersonate;
            query = new ObjectQuery("Select * from Win32_NTEventLogFile Where LogFileName='Application'");
            searcher = new ManagementObjectSearcher(scope, query);

            foreach (ManagementObject o in searcher.Get())
            {
               ManagementBaseObject inParams = o.GetMethodParameters("BackupEventlog");
               inParams["ArchiveFileName"] = @"g:\ELogBackup\Application.evt";
               ManagementBaseObject outParams = o.InvokeMethod("BackupEventLog", inParams, null);
               Debug.WriteLine(outParams.Properties["ReturnValue"].Value.ToString());
            }
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);
         }
      }

   }
}


/*
 * https://blogs.technet.microsoft.com/janelewis/2010/04/30/giving-non-administrators-permission-to-read-event-logs-windows-2003-and-windows-2008/
 * https://support.microsoft.com/en-us/help/842209/you-receive-an-access-is-denied-error-message-when-you-try-to-access-a
 * https://stackoverflow.com/questions/417578/how-to-view-windows-event-log-remotely-with-limited-privileges
   Click Start, click Run, type mmc, and then click OK.
      On the Console menu, click Add/Remove Snap-in.
   Click Add, click Group Policy, click Add, click Browse, click Default Domain Policy, click OK, and then click Finish.
   Click Close, and then click OK.
      
   In the left-pane, expand Default Domain Policy, expand Computer Configuration, expand Windows Settings, expand Security Settings, 
      expand Event Log, and then click Settings for Event Logs. Double-click Restrict guest access to application log, click to clear 
      the Define this policy setting check box, and then click OK.

   Double-click Restrict guest access to security log, click to clear the Define this policy setting check box, and then click OK.
   Double-click Restrict guest access to system log, click to clear the Define this policy setting check box, and then click OK.
   Click Start, click Run, type regedit, and then click OK.
      Locate and then click the following registry subkey:
   HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application
      Point to New on the Edit menu, and then click DWORD Value. Type RestrictGuestAccess, and then press ENTER.
      Double-click RestrictGuestAccess, type 1 in the Value data box, and then click OK.
   Repeat steps 9 through 11 for the following registry subkeys:
      HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\System
      HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Security
 */
