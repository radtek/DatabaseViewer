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
   public partial class CalcScheduledTime : Form
   {
      public CalcScheduledTime()
      {
         InitializeComponent();
      }

      private void cmdCompute_Click(object sender, EventArgs e)
      {
         DateTime tsDelayDuration = new DateTime(); ;

         if (DateTime.Now.Hour > 8)
         {
            int HoursFromNow = (24 - DateTime.Now.Hour) + 8;
            int MinutesFromNow = DateTime.Now.Minute * -1;
            int SecondsFromNow = DateTime.Now.Second * -1;
            tsDelayDuration = DateTime.Now.AddDays(0).AddHours(HoursFromNow).AddMinutes(MinutesFromNow).AddSeconds(SecondsFromNow);
            
         }

         if (DateTime.Now.Hour < 8)
         {
            int HoursFromNow = (8 - DateTime.Now.Hour);
            int MinutesFromNow = DateTime.Now.Minute * -1;
            int SecondsFromNow = DateTime.Now.Second * -1;
            tsDelayDuration = DateTime.Now.AddDays(0).AddHours(HoursFromNow).AddMinutes(MinutesFromNow).AddSeconds(SecondsFromNow);
         }

         tbTime1.Text = tsDelayDuration.ToString();

         //DateTime EndDate = DateTime.Now.AddDays(Days).AddHours(Hours).AddMinutes(Minutes).AddSeconds(Seconds);
         //TimeSpan DateTimeDifference = EndDate - DateTime.Now;

      }
   }
}
