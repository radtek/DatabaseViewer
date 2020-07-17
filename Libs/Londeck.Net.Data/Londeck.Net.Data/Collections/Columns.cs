using Microsoft.VisualBasic;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Londeck.Net.Data
{
    public class ColumnCollection : Dictionary<string, object>
    {
        public ColumnCollection(SqlDataReader dr, bool fillValue )
        {
            for (int i = 0; i <= dr.FieldCount - 1; i++)
            {
                try
                {
                    if (( !base.ContainsKey(dr.GetName(i).ToLower()) ))
                    {
                        if (fillValue)
                        {
                            base.Add(dr.GetName(i).ToLower(), dr.GetValue(i));
                        }
                        else
                        {
                            base.Add(dr.GetName(i).ToLower(), null);
                        }
                    }
                }
                catch (System.ArgumentException ex)
                {
                  throw ex;
                }
            }
        }

      public ColumnCollection(DataSet ds, bool fillValue)
      {
         if (ds == null)
            return;

         for (int i = 0; i <= ds.Tables[0].Columns.Count - 1; i++)
         {
            try
            {
               if ((!base.ContainsKey(ds.Tables[0].Columns[i].ToString().ToLower())))
               {
                  if (fillValue)
                  {
                     base.Add(ds.Tables[0].Columns[i].ToString().ToLower(), ds.Tables[0].Columns[i]);
                  }
                  else
                  {
                     base.Add(ds.Tables[0].Columns[i].ToString().ToLower(), null);
                  }
               }
            }
            catch (System.ArgumentException ex)
            {
               throw ex;
            }
         }
      }

        public ColumnCollection(DataRow dr)
        {
            for (int i = 0; i <= dr.Table.Columns.Count - 1; i++)
            {
                try
                {
                    Add(dr.Table.Columns[i].Caption.ToLower(), dr[i]);
                }
                catch (System.ArgumentException ex)
                {
          throw ex;
                }
            }
        }

        public bool Fetch(string ColumnName, ref object Value)
        {
            if (base.ContainsKey(ColumnName.ToLower()) && !object.ReferenceEquals(base[ColumnName.ToLower()], System.DBNull.Value))
            {
                if (Value is string)
                {
                    Value = base[ColumnName.ToLower()].ToString();

                }
                else if (Value is DateTime)
                {
                    object RowValue = base[ColumnName.ToLower()];
                    Type RowValueType = RowValue.GetType();
                    switch (RowValueType.Name)
                    {
                        case "int":
                            Value = new DateTime((int)RowValue);
                            break;

                        case "long":
                            Value = new DateTime((long)RowValue);
                            break;

                        case "string":
                            DateTime dValue = new DateTime();
                            DateTime.TryParse(RowValue.ToString(), out dValue);
                            Value = dValue;
                            break;
                    }
                }
                else if (Value is System.DateTime)
                {
                    object RowValue = base[ColumnName.ToLower()];
                    Type RowValueType = RowValue.GetType();
                    switch (RowValueType.ToString())
                    {
                        case "int":
                            Value = new DateTime((int)RowValue);
                            break;

                        case "long":
                            Value = new DateTime((long)RowValue);
                            break;
                    }
                }
                else if (Value is int)
                {
                    int intValue;
                    int.TryParse(base[ColumnName.ToLower()].ToString(), out intValue);
                    Value = intValue;
                }
                else if (Value is bool)
                {
                    bool fValue;
                    bool.TryParse(base[ColumnName.ToLower()].ToString(), out fValue);
                    Value = fValue;
                }
                else
                {
                    Value = base[ColumnName.ToLower()];
                }
                return true;
            }
            return false;
        }
    }
}