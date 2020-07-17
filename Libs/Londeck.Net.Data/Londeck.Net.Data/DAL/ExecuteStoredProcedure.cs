using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Xml;
using Microsoft.SqlServer.MessageBox;
using System.Xml.Xsl;

namespace Londeck.Net.Data
{
   public class ExecuteStoredProcedure : IDisposable
   {
      private List<SqlParameter> InputParams = new List<SqlParameter>();
      public Dictionary<string, System.Data.SqlClient.SqlParameter> Results = new Dictionary<string, System.Data.SqlClient.SqlParameter>();
      private string _StoredProcedureName;
      public const int COMMAND_TIMEOUT = 45;
      public static int CommandTimeout = COMMAND_TIMEOUT;

      private SqlCommand Command;
      public ExecuteStoredProcedure()
      { }

      public ExecuteStoredProcedure(string StoredProcName)
      {
         _StoredProcedureName = StoredProcName;
         InputParams.Clear();
      }

      public int TimeOut
      {
         get { return CommandTimeout; }
         set { CommandTimeout = value; }
      }

      public string StoredProcedureName
      {
         get { return _StoredProcedureName; }
         set { _StoredProcedureName = value; }
      }

#region "Add SQL Input Parameter"
      public void AddSqlParameter(string ParameterName, SqlDbType DbType, int Size, object Value, ParameterDirection Direction)
      {
         System.Data.SqlClient.SqlParameter stParam = null;
         if (DbType == SqlDbType.NVarChar | DbType == SqlDbType.VarChar | DbType == SqlDbType.NChar | DbType == SqlDbType.Char)
         {
            stParam = new System.Data.SqlClient.SqlParameter(ParameterName, DbType, Size);
         }
         else
         {
            stParam = new System.Data.SqlClient.SqlParameter(ParameterName, DbType);
         }

         if (DbType == SqlDbType.Xml & Value != null)
         {
            System.Xml.XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml((string)Value);
            stParam.Value = RemoveXMLNameSpace(xdoc);
         }
         else
         {
            stParam.Value = Value;
         }
         stParam.Direction = Direction;
         InputParams.Add(stParam);
      }

      public void AddSqlParameter(string ParameterName, SqlDbType DbType, object Value, ParameterDirection Direction)
      {
         System.Data.SqlClient.SqlParameter stParam = null;
         stParam = new System.Data.SqlClient.SqlParameter(ParameterName, DbType);
         if (DbType == SqlDbType.Xml & Value != null)
         {
            System.Xml.XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml((string)Value);
            stParam.Value = RemoveXMLNameSpace(xdoc);
         }
         else
         {
            stParam.Value = Value;
         }
         stParam.Direction = Direction;
         InputParams.Add(stParam);
      }

      /// <summary>
      /// This is useful for a quick output parameter definition.  Sets the Parameter Value to NULL.
      /// </summary>
      /// <param name="ParameterName"></param>
      /// <param name="DbType"></param>
      /// <param name="Direction"></param>
      public void AddSqlParameter(string ParameterName, SqlDbType DbType, ParameterDirection Direction)
      {
         System.Data.SqlClient.SqlParameter stParam = null;
         stParam = new System.Data.SqlClient.SqlParameter(ParameterName, DbType);
         if (DbType == SqlDbType.Xml)
         {
            System.Xml.XmlDocument xdoc = new XmlDocument();
            //xdoc.LoadXml(DirectCast(Value, String))
            stParam.Value = RemoveXMLNameSpace(xdoc);
         }
         else
         {
            //stParam.Value = Value
         }
         stParam.Direction = Direction;
         stParam.Value = null;
         InputParams.Add(stParam);
      }
#endregion

#region "Add SQL Output Parameter"
      public void AddSqlOutputParameter(string ParameterName, SqlDbType DbType, int Size)
      {
         System.Data.SqlClient.SqlParameter stParam = null;
         if (DbType == SqlDbType.NVarChar | DbType == SqlDbType.VarChar | DbType == SqlDbType.NChar | DbType == SqlDbType.Char)
         {
            stParam = new System.Data.SqlClient.SqlParameter(ParameterName, DbType, Size);
         }
         else
         {
            stParam = new System.Data.SqlClient.SqlParameter(ParameterName, DbType);
         }
         stParam.Direction = ParameterDirection.Output;
         InputParams.Add(stParam);
      }

#endregion
      /// <summary>
      /// Executes the Stored procedure using the connection provided and returns a dataset..
      /// </summary>
      /// <param name="ProcedureName"></param>
      /// <param name="Connection"></param>
      /// <returns></returns>
      /// <remarks></remarks>
      public DataSet ExecuteProcedureDS(string ProcedureName, ref SqlConnection Connection)
      {
         try
         {
            string ProcName = (string)IsNull(ProcedureName, _StoredProcedureName);
            Command = new SqlCommand(ProcName, Connection);
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = ProcedureName;
            Command.CommandTimeout = this.TimeOut;
            foreach (System.Data.SqlClient.SqlParameter SQLParam in InputParams)
            {
               Command.Parameters.Add(SQLParam);
            }

            if (Connection.State == ConnectionState.Closed)
               Connection.Open();

            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(Command);
            adapter.Fill(ds);
            if (Results.Count > 0)
            {
               Results.Clear();
            }
            UpdateQueryResults(Command);
            return ds;

         }
         catch (SqlException ex)
         {
            //throw ex;
            return null;
         }
         finally
         {
            Connection.Close();
         }
      }

      public DataSet ExecuteProcedureDS(string ProcedureName, ref SqlConnection Connection, string TableName)
      {
         try
         {
            string ProcName = (string)IsNull(ProcedureName, _StoredProcedureName);
            Command = new SqlCommand(ProcName, Connection);
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = ProcedureName;
            Command.CommandTimeout = this.TimeOut;
            foreach (System.Data.SqlClient.SqlParameter SQLParam in InputParams)
            {
               Command.Parameters.Add(SQLParam);
            }

            if (Connection.State == ConnectionState.Closed)
               Connection.Open();

            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(Command);
            if (!(string.IsNullOrEmpty(TableName)))
            {
               adapter.Fill(ds, TableName);
            }
            else
            {
               adapter.Fill(ds);
            }

            if (Results.Count > 0)
            {
               Results.Clear();
            }
            UpdateQueryResults(Command);
            return ds;

         }
         catch (SqlException ex)
         {
            throw ex;
         }
         finally
         {
            Connection.Close();
         }
      }

      public DataSet ExecuteSQLTextDS(string SQLQuery, SqlConnection Connection, string TableName)
      {
         try
         {
            Command = new SqlCommand(SQLQuery, Connection);
            Command.CommandType = CommandType.Text;
            Command.CommandTimeout = this.TimeOut + 30;

            if (Connection.State == ConnectionState.Closed)
               Connection.Open();

            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(Command);
            if (!(string.IsNullOrEmpty(TableName)))
            {
               adapter.Fill(ds, TableName);
            }
            else
            {
               adapter.Fill(ds);
            }
            return ds;

         }
         catch (SqlException ex)
         {
            throw ex;
         }
         finally
         {
            Connection.Close();
         }
      }

      public string ExecuteSQLDSasXML(string SQL, string SourceTable, SqlConnection Connection)
      {
         try
         {
            Command = new SqlCommand(SQL, Connection);
            //cmd.CommandType = CommandType.StoredProcedure

            Command.CommandTimeout = this.TimeOut;
            foreach (System.Data.SqlClient.SqlParameter SQLParam in InputParams)
            {
               Command.Parameters.Add(SQLParam);
            }

            Connection.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(Command);
            adapter.Fill(ds, SourceTable);
            UpdateQueryResults(Command);

            Connection.Close();
            return ds.GetXml();
         }
         catch (SqlException ex)
         {
            throw ex;
         }
      }

      public string ExecuteSQLDSasXML(string SQL, SqlConnection Connection)
      {
         try
         {
            Command = new SqlCommand(SQL, Connection);
            //cmd.CommandType = CommandType.StoredProcedure
            Command.CommandTimeout = this.TimeOut;
            foreach (System.Data.SqlClient.SqlParameter SQLParam in InputParams)
            {
               Command.Parameters.Add(SQLParam);
            }

            Connection.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(Command);
            adapter.Fill(ds);
            UpdateQueryResults(Command);

            Connection.Close();
            return ds.GetXml();
         }
         catch (SqlException ex)
         {
            throw ex;
         }
      }

      public string ExecuteSPReturningXML(string StoredProcedureName, string TableName, SqlConnection Connection)
      {
         Command = new SqlCommand(StoredProcedureName, Connection);
         try
         {
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandTimeout = this.TimeOut;

            foreach (System.Data.SqlClient.SqlParameter SQLParam in InputParams)
            {
               Command.Parameters.Add(SQLParam);
            }
            Connection.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(Command);
            adapter.Fill(ds, TableName);
            UpdateQueryResults(Command);
            return ds.GetXml();
         }
         catch (SqlException ex)
         {
            throw ex;
         }
      }

      public void ClearInputParams()
      {
         InputParams.Clear();
      }

      /// <summary>
      /// Executes the Stored procedure using the connection provided and returns a data reader..
      /// for Asynchronous execution the Connection string must append or have 'Asynchronouse processing = True'
      /// somewhere in the string.
      /// </summary>
      /// <param name="ProcedureName"></param>
      /// <param name="Connection"></param>
      /// <returns></returns>
      /// <remarks></remarks>
      public SqlDataReader ExecuteProcedureReaderAsync(string ProcedureName, SqlConnection Connection)
      {
         string ProcName = (string)IsNull(ProcedureName, _StoredProcedureName);
         Command = new SqlCommand(ProcName, Connection);
         Command.CommandTimeout = this.TimeOut;
         Command.CommandText = ProcedureName;
         Command.CommandType = CommandType.StoredProcedure;

         foreach (System.Data.SqlClient.SqlParameter SQLParam in InputParams)
         {
            Command.Parameters.Add(SQLParam);
         }
         try
         {
            Connection.Open();
            System.IAsyncResult result = Command.BeginExecuteReader();
            SqlDataReader dr = null;
            dr = Command.EndExecuteReader(result);
            // System.Data.CommandBehavior.CloseConnection

            if (Results.Count > 0)
            {
               Results.Clear();
            }

            UpdateQueryResults(Command);
            return dr;
         }
         catch (SqlException ex)
         {
            throw ex;
         }
      }

      /// <summary>
      /// Executes the Stored procedure using the connection provided and returns a data reader..
      /// </summary>
      /// <param name="ProcedureName"></param>
      /// <param name="Connection"></param>
      /// <returns></returns>
      /// <remarks></remarks>
      public SqlDataReader ExecuteSQLTextReader(string SQLQuery, ref SqlConnection Connection)
      {
         Command = new SqlCommand(SQLQuery, Connection);
         Command.CommandTimeout = this.TimeOut;
         Command.CommandType = CommandType.Text;

         SqlDataReader dr = null;

         try
         {
            Connection.Open();

            dr = Command.ExecuteReader();
            if (Results.Count > 0)
            {
               Results.Clear();
            }
            UpdateQueryResults(Command);
         }
         catch (SqlException ex)
         {
            throw ex;
         }
         return dr;
      }

      /// <summary>
      /// Executes the Stored procedure using the connection provided and returns a data reader..
      /// </summary>
      /// <param name="ProcedureName"></param>
      /// <param name="Connection"></param>
      /// <returns></returns>
      /// <remarks></remarks>
      public SqlDataReader ExecuteProcedureReader(string ProcedureName, ref SqlConnection Connection)
      {
         string ProcName = (string)IsNull(ProcedureName, _StoredProcedureName);
         Command = new SqlCommand(ProcName, Connection);
         Command.CommandTimeout = this.TimeOut;
         Command.CommandText = ProcedureName;
         Command.CommandType = CommandType.StoredProcedure;

         foreach (System.Data.SqlClient.SqlParameter SQLParam in InputParams)
         {
            Command.Parameters.Add(SQLParam);
         }
         SqlDataReader dr = null;

         try
         {
            Connection.Open();

            dr = Command.ExecuteReader();
            if (Results.Count > 0)
            {
               Results.Clear();
            }
            UpdateQueryResults(Command);
         }
         catch (SqlException ex)
         {
            throw ex;
         }
         return dr;
      }

      /// <summary>
      /// Executes the Stored procedure using the connection provided and returns the number of rows changed..
      /// This executes Command.ExecuteNonQuery() and retuns the number of rows affected.  
      /// Ideal for updates.
      /// </summary>
      /// <param name="ProcedureName"></param>
      /// <param name="Connection"></param>
      /// <returns></returns>
      /// <remarks></remarks>
      public int ExecuteProcedure(string ProcedureName, ref SqlConnection Connection)
      {
         string ProcName = (string)IsNull(ProcedureName, _StoredProcedureName);
         Command = new SqlCommand(ProcName, Connection);
         Command.CommandTimeout = this.TimeOut;
         Command.CommandText = ProcedureName;
         Command.CommandType = CommandType.StoredProcedure;

         if (Connection.State == ConnectionState.Closed)
            Connection.Open();

         foreach (System.Data.SqlClient.SqlParameter SQLParam in InputParams)
         {
            Command.Parameters.Add(SQLParam);
         }

         try
         {
            int ReturnCode = 0;
            ReturnCode = Command.ExecuteNonQuery();
            if (Results.Count > 0)
            {
               Results.Clear();
            }
            UpdateQueryResults(Command);
            return ReturnCode;
         }
         catch (SqlException ex)
         {
            //ProcedureParameterError(Connection, ProcedureName, ex);
            Debug.WriteLine(ex.Message);
            return 0;
            //throw ex;
         }
         finally
         {
            Connection.Close();
            Connection.Dispose();
         }
      }

      public object ExecuteProcedureScalar(string ProcedureName, SqlConnection Connection)
      {
         string ProcName = (string)IsNull(ProcedureName, _StoredProcedureName);
         Command = new SqlCommand(ProcName, Connection);
         Command.CommandText = ProcedureName;

         object ReturnObj = null;
         Command.CommandType = CommandType.StoredProcedure;

         foreach (System.Data.SqlClient.SqlParameter SQLParam in InputParams)
         {
            Command.Parameters.Add(SQLParam);
         }
         try
         {
            Connection.Open();
            ReturnObj = Command.ExecuteScalar();
            if (Results.Count > 0)
            {
               Results.Clear();
            }

            UpdateQueryResults(Command);
            return ReturnObj;
         }
         catch (SqlException ex)
         {
            // throw ex;
            ProcedureParameterError(Connection, ProcedureName, ex);
            throw ex;
         }
      }

      public object ExecuteTextScalar(string Query, SqlConnection Connection)
      {
         // string ProcName = (string)IsNull(ProcedureName, _StoredProcedureName);
         Command = new SqlCommand(Query, Connection);
         object ReturnObj = null;
         Command.CommandType = CommandType.Text;

         foreach (System.Data.SqlClient.SqlParameter SQLParam in InputParams)
         {
            Command.Parameters.Add(SQLParam);
         }
         try
         {
            Connection.Open();
            ReturnObj = Command.ExecuteScalar();
            if (Results.Count > 0)
            {
               Results.Clear();
            }
            UpdateQueryResults(Command);
            return ReturnObj;
         }
         catch (SqlException ex)
         {
            throw ex;
         }

      }

      public string RemoveXMLNameSpace(XmlDocument XmlDoc)
      {
         //Remove the xmlns attribute from all elements in the XmlDoc 
         //Replace instances of a single quote with a double single quote.
         XmlAttribute Attrib = null;
         foreach (XmlNode Node in XmlDoc)
         {
            if (Node.HasChildNodes)
            {
               foreach (XmlNode ChildNode in Node)
               {
                  if (ChildNode.NodeType == XmlNodeType.Element)
                  {
                     Attrib = ChildNode.Attributes["xmlns"];
                     if ((Attrib != null))
                     {
                        Attrib.RemoveAll();
                     }
                  }
               }
            }
         }

         //NOTE: I do not think we need to escape the quotes. Stored Procedure Parameters already do that.
         //      Otherwise we just double the quotes in the DB.
         //Dim XmlOut As String = XmlDoc.OuterXml.Replace("'", "''")
         //XmlOut = XmlOut.Replace("utf-8", "utf-16")

         string XmlOut = XmlDoc.OuterXml.Replace("'", "''");
         return XmlOut;
      }

      private void UpdateQueryResults(SqlCommand CommandParameters)
      {
         if (Results.Count > 0)
         {
            Results.Clear();
         }

         foreach (System.Data.SqlClient.SqlParameter Param in CommandParameters.Parameters)
         {
            if (Param.Direction == ParameterDirection.InputOutput || Param.Direction == ParameterDirection.Output || Param.Direction == ParameterDirection.ReturnValue)
            {
               Results.Add(Param.ParameterName, Param);
            }
         }
      }

      private void ProcedureParameterError( SqlConnection SqlConn, string ProcedureName, Exception ex)
      {
         string CommandText =
                        " SELECT ";
         CommandText += "   PA.Name as ParameterName, ";
         CommandText += "   CASE WHEN PA.is_output = 1 THEN 'Output' ";
         CommandText += "   ELSE ";
         CommandText += "     'Input' ";
         CommandText += "   END AS [In/Output], ";
         CommandText += "   UPPER(st.name) AS DataType, ";
         CommandText += "   PA.max_length AS Length";
         CommandText += " FROM Sys.Procedures AS PR ";
         CommandText += " JOIN Sys.Parameters AS PA ";
         CommandText += "   ON PR.object_id = PA.object_id ";
         CommandText += " JOIN sys.types AS ST ";
         CommandText += "   ON PA.user_type_id = ST.user_type_id ";
         CommandText += " WHERE PR.Name = '{0}' ";
         CommandText = string.Format(CommandText, ProcedureName);

         SqlCommand _cmd = new SqlCommand(CommandText, SqlConn);
         SqlDataAdapter adapter = new SqlDataAdapter(_cmd);
         DataSet ds = new DataSet();
         adapter.Fill(ds, "ParameterListing");
         string ParameterName = string.Empty;
         string ErrorMessage = string.Empty;
         if (ex.Message.Contains("expects parameter"))
         {
            // The table we are generating will be the source of truth
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
               ParameterName = dr["ParameterName"].ToString();
               SqlParameter param = InputParams.Find(x => x.ParameterName == ParameterName);
               if (param == null)      // Parameter is missing
               {
                  // ApplicationException exTop = new ApplicationException("Missing SQL Parameter", ex);
                  ExceptionMessageBox MsgBox = new ExceptionMessageBox();
                  ErrorMessage = string.Format("Procedure call is missing parameter {0}" + Environment.NewLine, ParameterName);
                  ErrorMessage += "Datatype: " + dr["DataType"] + "; " + dr["Length"] + " bytes; Direction: " + dr["In/Output"];
                  System.Windows.Forms.MessageBox.Show(ErrorMessage, "Missing SQL Parameter.", System.Windows.Forms.MessageBoxButtons.OK);
               }
            }
         }
         else
         {

            throw ex;
         }
      }

      private object IsNull(object A, object B)
      {
         if ((A != null))
         {
            return A;
         }

         if ((B != null))
         {
            return B;
         }
         return null;
      }

/*
      private string DateTimeStamp()
      {
         string _MonthDayStart;
         string _MonthDayEnd = string.Empty;
         string DateRange;

         DateTime Start_Date = StartDate.Value;
         DateTime End_Date = EndDate.Value;

         _MonthDayStart = Start_Date.Month.ToString().PadLeft(2, '0');
         _MonthDayStart += Start_Date.Day.ToString().PadLeft(2, '0');

         //if ((End_Date.Day - Start_Date.Day) > 1)
         if (End_Date.Subtract(Start_Date).Days > 1)
         {
            _MonthDayEnd = End_Date.Month.ToString().PadLeft(2, '0');
            //_MonthDayEnd += ((string)(End_Date.Day - 1).ToString()).PadLeft(2, '0');
            _MonthDayEnd += ((string)(End_Date.AddDays(-1)).Day.ToString()).PadLeft(2, '0');

            // .ToString().PadLeft(2, '0');
            DateRange = _MonthDayStart + "-" + _MonthDayEnd;
         }
         else
         {
            DateRange = _MonthDayStart;
         }
         string Stamp = "_" + DateRange + "_" +
            DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Year.ToString() + "_";
         Stamp += DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
         return Stamp;
      }
 */

      public void Dispose()
      {
         InputParams.Clear();
         Command.Dispose();
      }
   }
}