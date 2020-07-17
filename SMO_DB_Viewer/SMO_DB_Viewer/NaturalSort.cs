using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security;


namespace SMO_DB_Viewer
{
   [SuppressUnmanagedCodeSecurity]
   internal static class SafeNativeMethods
   {
      [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
      public static extern int StrCmpLogicalW(string psz1, string psz2);
   }

   public class NaturalStringComparer : Comparer<string>
   {
      string delimiter;
      bool isAscending;

      public NaturalStringComparer(string Delimeter, bool Ascending)
      {
         delimiter = Delimeter;
         isAscending = Ascending;
      }

      public override int Compare(string a, string b)
      {
         var r = SafeNativeMethods.StrCmpLogicalW(a, b);
         return isAscending ? r : -r;
      }

      string GetMySubstring(string str)
      {
         /*
          * https://stackoverflow.com/questions/13417912/sorting-a-string-array-according-to-a-string-position-c
          */
         //return str.IndexOf(delimiter) != -1 ? str.Substring(str.LastIndexOf(delimiter)) : string.Empty;
         return str.IndexOf(delimiter) != -1 ? str.Substring(str.LastIndexOf(delimiter) + delimiter.Length) : string.Empty;
      }
   }
}

/*
 *    private void Results_Natural_Sort()
 *    {
 *       string[] InputString = new string[tbScripts.Lines.Count];
 *       ScintillaNet.LinesCollection input = tbScripts.Lines;
 *       input.CopyTo(InputString, 0);
 *
 *       Array.Sort(InputString, new NaturalStringComparer(tbScripts.Text, true));
 *
 *       tbScripts.Text = string.Empty;
 *       foreach (string s in InputString)
 *       {
 *          if (s.Length > 0)
 *          {
 *             //tbRefactoredResults.Text += s;
 *             if (!s.EndsWith(Environment.NewLine))
 *             {
 *                tbScripts.Text += s + Environment.NewLine;
 *             }
 *             else
 *             {
 *                tbScripts.Text += s;
 *             }
 *          }
 *       }
 *    }
 */
