using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO_DB_Viewer
{
   public static class StringExtensions
   {
      public static string Stuff(this string input, int start, int length, string replaceWith)
      {
         return input.Remove(start, length).Insert(start, replaceWith);
      }

      public static string Splice(this string str, int start, int length,
                          string replacement)
      {
         return str.Substring(0, start) +
                replacement +
                str.Substring(start + length);
      }

      public static string StuffAfter(this string input, string searchText, string StuffText)
      {
         ///ToDO: Add logic to sfuff each matching instance of Searchtext with replacewith text.
         if (input.Length == 0)
            return string.Empty;

         int idx = 0;
         do
         {
            idx = input.IndexOf(searchText, idx, StringComparison.CurrentCultureIgnoreCase);
            if (idx == -1)
               break;

            input = input.Insert(idx + searchText.Length, StuffText);
            idx += searchText.Length;
         } while (idx != -1);
         return input;
      }

      public static string StuffAt(this string input, string SearchText, string StuffText)
      {
         int idx = 0;
         do
         {
            idx = input.IndexOf(SearchText, idx, StringComparison.CurrentCultureIgnoreCase);
            if (idx == -1)
               break;
            if (SearchText == "," && StuffText == Environment.NewLine)
            {
               input = input.Insert(idx, StuffText + "  ");
            }
            else
            {
               input = input.Insert(idx, StuffText);
            }
            idx += (SearchText.Length + StuffText.Length );
         } while (idx != -1);

         return input;
      }

      public static string Repeat(this string s, int n)
      {
         return new String(Enumerable.Range(0, n).SelectMany(x => s).ToArray());
      }

      public static string Repeat(this char c, int n)
      {
         return new String(c, n);
      }

      /// <summary>
      /// Customize the StringComparison 
      /// </summary>
      /// <param name="source"></param>
      /// <param name="toCheck"></param>
      /// <param name="comp"></param>
      /// <returns></returns>
      public static bool ContainsCI(this string source, string toCheck, StringComparison stringcomparison)
      {
         return source.IndexOf(toCheck, stringcomparison) >= 0;
      }
   }
}
