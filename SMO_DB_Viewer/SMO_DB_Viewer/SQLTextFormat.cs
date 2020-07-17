using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO_DB_Viewer
{
   public class SQLTextFormat
   {
      public Dictionary<string, TextFormatInstructions> SQLTextFormatting_Dictionary = new Dictionary<string, TextFormatInstructions>();
      
      public  string SQLText
      { get; set; }
      
      public SQLTextFormat(string SQLText)
      {
         this.SQLText = SQLText;

         SQLTextFormatting_Dictionary.Add("CREATE", new TextFormatInstructions { Prefix = "", ReplacementText = "", StuffAfter = string.Empty, StuffBefore = Environment.NewLine, TexttoFind = "CREATE ", Suffix = "" });
         SQLTextFormatting_Dictionary.Add("SELECT", new TextFormatInstructions { Prefix = "", ReplacementText = "", StuffAfter = Environment.NewLine + "  ", StuffBefore = Environment.NewLine, TexttoFind = "SELECT ", Suffix = "" });
         SQLTextFormatting_Dictionary.Add("FROM", new TextFormatInstructions { Prefix = "", ReplacementText = " ", StuffAfter = string.Empty, StuffBefore = Environment.NewLine, TexttoFind = " FROM ", Suffix = "" });
         SQLTextFormatting_Dictionary.Add("ON", new TextFormatInstructions { Prefix = "", ReplacementText = " ", StuffAfter = string.Empty, StuffBefore = Environment.NewLine + "  ", TexttoFind = " ON ", Suffix = "" });
         SQLTextFormatting_Dictionary.Add(" LEFT OUTER JOIN ", new TextFormatInstructions { Prefix = "", ReplacementText = string.Empty, StuffAfter = string.Empty, StuffBefore = Environment.NewLine, TexttoFind = " LEFT OUTER JOIN ", Suffix = "" });
         SQLTextFormatting_Dictionary.Add("GROUP BY", new TextFormatInstructions { Prefix = "", ReplacementText = "", StuffAfter = Environment.NewLine, StuffBefore = string.Empty, TexttoFind = " GROUP BY ", Suffix = "" });
         SQLTextFormatting_Dictionary.Add("WITH", new TextFormatInstructions { Prefix = "", ReplacementText = "", StuffAfter = string.Empty, StuffBefore = Environment.NewLine, TexttoFind = "WITH", Suffix = "" });
         SQLTextFormatting_Dictionary.Add("USE [", new TextFormatInstructions { Prefix = "", ReplacementText = "", StuffAfter = string.Empty, StuffBefore = Environment.NewLine, TexttoFind = "USE [", Suffix = "" });

         FormatSQLText();
      }

      private void FormatSQLText()
      {
         foreach(KeyValuePair<string,TextFormatInstructions> Item in SQLTextFormatting_Dictionary)
         {
            if (Item.Value.StuffAfter != string.Empty)
            {
               SQLText = SQLText.StuffAfter(Item.Value.TexttoFind, Item.Value.StuffAfter);
            }

            if (Item.Value.StuffBefore != string.Empty)
            {
               SQLText = SQLText.StuffAt(Item.Value.TexttoFind, Item.Value.StuffBefore);
            }
         }
         SQLText = SQLText.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
         SQLText = SQLText.Replace("  ", " ");
         SQLText = SQLText.Replace("\t", "  ");
         //WalkEachLine();
      }

      public void WalkEachLine()
      {
         string TidyText = string.Empty;
         using (StringReader reader = new StringReader(SQLText))
         {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
               if (line.ToUpper().Contains("FROM") || line.StartsWith(" "))
               {
                  TidyText += line.Trim() + Environment.NewLine;
               }
               else
               {
                  TidyText += line + Environment.NewLine;
               }
            }
         }
         SQLText = TidyText;
      }
   }
}
