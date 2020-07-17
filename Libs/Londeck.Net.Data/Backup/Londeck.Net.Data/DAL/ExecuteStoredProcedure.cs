using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Xml;
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
        {
        }

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
            InputParams.Add(stParam);
        }

        /// <summary>
        /// Executes the Stored procedure using the connection provided and returns a dataset..
        /// </summary>
        /// <param name="ProcedureName"></param>
        /// <param name="Connection"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DataSet ExecuteProcedureDS(string ProcedureName,ref SqlConnection Connection)
        {
            try
            {
                string ProcName = (string)IsNull(ProcedureName, _StoredProcedureName);
                Command = new SqlCommand(ProcName, Connection);
                Command.CommandType = CommandType.StoredProcedure;
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
                throw ex;
            }
            finally
            {
                Connection.Close();
            }
        }

        public DataSet ExecuteProcedureDS(string ProcedureName, SqlConnection Connection, string TableName)
        {
            try
            {
                string ProcName = (string)IsNull(ProcedureName, _StoredProcedureName);
                Command = new SqlCommand(ProcName, Connection);
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandTimeout = this.TimeOut;
                foreach (System.Data.SqlClient.SqlParameter SQLParam in InputParams)
                {
                    Command.Parameters.Add(SQLParam);
                }

                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();

                DataSet ds = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(Command);
                if (!( string.IsNullOrEmpty(TableName) ))
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

                return ds.GetXml();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public string ExecuteSPDSasXML(string StoredProcedurName, SqlConnection Connection)
        {
            Command = new SqlCommand(StoredProcedurName, Connection);
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
                adapter.Fill(ds);
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
        /// for Asynchronous execution the Connection string must append or have ''Asynchronouse processing = True''
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
        public SqlDataReader ExecuteProcedureReader(string ProcedureName, ref SqlConnection Connection)
        {
            string ProcName = (string)IsNull(ProcedureName, _StoredProcedureName);
            Command = new SqlCommand(ProcName, Connection);
            Command.CommandTimeout = this.TimeOut;
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
                throw ex;
            }
        }

        public object ExecuteProcedureScalar(string ProcedureName, SqlConnection Connection)
        {
            string ProcName = (string)IsNull(ProcedureName, _StoredProcedureName);
            Command = new SqlCommand(ProcName, Connection);
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
                            if (( Attrib != null ))
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
                if (Param.Direction == ParameterDirection.InputOutput | Param.Direction == ParameterDirection.Output || Param.Direction == ParameterDirection.ReturnValue)
                {
                    Results.Add(Param.ParameterName, Param);
                }
            }
        }

        private object IsNull(object A, object B)
        {
            if (( A != null ))
            {
                return A;
            }

            if (( B != null ))
            {
                return B;
            }
            return null;
        }


        public void Dispose()
        {
            InputParams.Clear();
            Command.Dispose();
        }
    }
}