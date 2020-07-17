using System;
using System.Collections.Generic;
using System.Text;

namespace GA_SMO
{
   public class TableAndFKey
   {
      private string _DBName;
      private string _FKName;
      private string _LSName; 
      private string _TableName;
      private int _RowID;
       
      public int RowID
      {
          get { return _RowID; }
          set { _RowID = value; }
      }
      public string DBName
      {
         get { return _DBName; }
         set { _DBName = value; }
      }

      public string FKeyName
      {
         get { return _FKName; }
         set { _FKName = value; }
      }

      public string LSName
      {
         get { return _LSName; }
         set { _LSName = value; }
      }
      
      public string TableName
      {
         set { _TableName = value; }
         get { return _TableName; }
      }

      public TableAndFKey() 
      {
      }

      public TableAndFKey(string DBName, string FKeyName, string Table)
      {
         this.DBName = DBName;
         this.TableName = Table;
         this.FKeyName = FKeyName;
      }
   }

    public class TableNames
    {
        private string _TableName;
        public string TableName
        {
            set { _TableName = value; }
            get { return _TableName; }
        }
    }
}
