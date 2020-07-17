using Microsoft.VisualBasic;
using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;

namespace Londeck.Net.Data
{
  public class Db
  {
    public static bool ThrowExceptions = true;
    public const int COMMAND_TIMEOUT = 45;
    public static int CommandTimeout = COMMAND_TIMEOUT;

    #region "    Connection String Properties    "

    public static string GetConnectionString(string connString, bool encrypted)
    {
      string SQLConnection;
      if (!(connString == null))
      {
        SQLConnection = FetchConnectionString(connString);
        if (encrypted)
        {
          SQLConnection = DecryptString(SQLConnection);
        }
      }
      else
      {
        SQLConnection = string.Empty;
      }
      return SQLConnection;

    }

    private static string FetchConnectionString(string Key)
    {
      // string ConnString = WebConfigurationManager.ConnectionStrings[Key].ConnectionString;
      string ConnString;
      try
      {
        ConnString = ConfigurationManager.ConnectionStrings[Key].ConnectionString;
        //if (ConnString == String.Empty)
        //{
        //  return string.Empty;
        //}
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return ConnString;
    }
    #endregion

    #region "Data Encryption"

    public static void EncryptConnectionStrings()
    {
      System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
      System.Configuration.ConnectionStringsSection sec = config.ConnectionStrings;

      if ((!sec.SectionInformation.IsProtected))
      {
        sec.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
        sec.SectionInformation.ForceSave = true;
        config.Save(ConfigurationSaveMode.Full);
      }
    }

    public static string DecryptString(string Message)
    {
      byte[] Results;
      System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
      MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
      byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes("HTICyberneticsDefault"));
      TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
      TDESAlgorithm.Key = TDESKey;
      TDESAlgorithm.Mode = CipherMode.ECB;
      TDESAlgorithm.Padding = PaddingMode.PKCS7;
      byte[] DataToDecrypt = Convert.FromBase64String(Message);

      try
      {
        ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
        Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
      }
      finally
      {
        TDESAlgorithm.Clear();
        HashProvider.Clear();
      }
      return UTF8.GetString(Results);
    }

    public static string EncryptData(string Message)
    {
      byte[] Results;
      System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
      MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
      byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes("HTICyberneticsDefault"));
      TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
      TDESAlgorithm.Key = TDESKey;
      TDESAlgorithm.Mode = CipherMode.ECB;
      TDESAlgorithm.Padding = PaddingMode.PKCS7;
      byte[] DataToEncrypt = UTF8.GetBytes(Message);

      try
      {
        ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
        Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
      }
      finally
      {
        TDESAlgorithm.Clear();
        HashProvider.Clear();
      }
      return Convert.ToBase64String(Results);
    }

    #endregion

    #region "    Connections    "
    public static SqlConnection OpenConnection(string ConnectionString)
    {
      SqlConnection Connection = new SqlConnection(ConnectionString);
      Connection.Open();

      if (Connection.State != ConnectionState.Open)
        throw new Exception("Database connection failure");

      return Connection;
    }

    public static SqlCommand OpenCommand(string Sql, string ConnectionString, ref SqlConnection Connection)
    {
      SqlCommand Command = new SqlCommand(Sql);
      if (!(Connection == null) && string.IsNullOrEmpty(ConnectionString))
      {
        ConnectionString = Connection.ConnectionString;
      }

      if (Connection == null)
      {
        Connection = OpenConnection(ConnectionString);
      }
      else
      {
        if (Connection.State != ConnectionState.Open)
        {
          Connection.Dispose();
          Connection = OpenConnection(ConnectionString);
        }
      }
      Command.Connection = Connection;
      Command.CommandTimeout = CommandTimeout;

      return Command;
    }

    public static void ConnectionDispose(ref SqlConnection Connection)
    {
      if (Connection == null)
        return;

      if (Connection.State != ConnectionState.Closed)
        Connection.Close();
      Connection.Dispose();
      Connection = null;
    }
    #endregion

    #region "    Query    "

    public static DataTable Query(string Sql, ArrayList Params, string ConnectString)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return Query(Sql, Params, ConnectString, ref conn);
    }

    public static DataTable Query(string Sql, ArrayList Params, string ConnectString, ref SqlConnection Connection)
    {
      DataTable dt = new DataTable();
      SqlCommand Command = null;
      bool CloseConnection = false;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(Sql, ConnectString, ref Connection);
      SqlDataAdapter da = null;

      try
      {
        if ((Params != null))
        {
          foreach (SqlParameter Param in Params)
          {
            Command.Parameters.Add(Param);
          }
        }

        da = new SqlDataAdapter(Command);
        try
        {
          da.Fill(dt);

        }
        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") & ex.Message.Contains("deadlocked"))
          {
            try
            {
              da.Fill(dt);
            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            if (ThrowExceptions)
              throw ex;
          }
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;

        da.Dispose();
        da = null;

        if (CloseConnection)
        {
          Connection.Close();
          Connection.Dispose();
          Connection = null;
        }
      }
      return dt;
    }

    public static DataTable Query(string Sql, SqlParameter SqlParam, string ConnectString)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return Query(Sql, SqlParam, ConnectString, ref conn);
    }

    public static DataTable Query(string Sql, SqlParameter SqlParam, string ConnectString, ref SqlConnection Connection)
    {
      DataTable dt = new DataTable();
      SqlCommand Command = null;
      bool CloseConnection = false;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(Sql, ConnectString, ref Connection);
      SqlDataAdapter da = null;
      try
      {
        Command.Parameters.Add(SqlParam);

        da = new SqlDataAdapter(Command);

        try
        {
          da.Fill(dt);
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") & ex.Message.Contains("deadlocked"))
          {
            try
            {
              da.Fill(dt);
            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            if (ThrowExceptions)
              throw ex;
          }
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;

        da.Dispose();
        da = null;

        if (CloseConnection)
        {
          Connection.Close();
          Connection.Dispose();
          Connection = null;
        }
      }
      return dt;
    }

    public static DataTable Query(string Sql, string ConnectString)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return Query(Sql, ConnectString, ref conn);
    }

    public static DataTable Query(string Sql, string ConnectString, ref SqlConnection Connection)
    {
      ArrayList arParams = null;
      return Query(Sql, arParams, ConnectString, ref Connection);
    }

    #endregion

    #region "    Query Reader   "

    public static SqlDataReader QueryReader(string Sql, ArrayList Params, string ConnectString)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return QueryReader(Sql, Params, ConnectString, ref conn);
    }

    public static SqlDataReader QueryReader(string Sql, ArrayList Params, string ConnectString, ref SqlConnection Connection)
    {
      SqlDataReader dr = null;
      SqlCommand Command = null;
      bool CloseConnection = false;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(Sql, ConnectString, ref Connection);
      try
      {
        if ((Params != null))
        {
          foreach (SqlParameter Param in Params)
          {
            Command.Parameters.Add(Param);
          }
        }
        try
        {
          if (CloseConnection)
          {
            dr = Command.ExecuteReader(CommandBehavior.CloseConnection);
          }
          else
          {
            dr = Command.ExecuteReader();
          }
        }

        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") && ex.Message.Contains("deadlocked"))
          {
            try
            {
              if (CloseConnection)
              {
                if (Command.Connection.State == ConnectionState.Closed)
                {
                  Command.Connection.Dispose();
                  Command.Connection = OpenConnection(ConnectString);
                }
              }
              else
              {
                return null;
              }

              if (CloseConnection)
              {
                dr = Command.ExecuteReader(CommandBehavior.CloseConnection);
              }
              else
              {
                dr = Command.ExecuteReader();
              }
            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            if (ThrowExceptions)
              throw ex;
          }
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;
        //If CloseConnection Then
        //   If Connection.State = ConnectionState.Open Then Connection.Close()
        //   Connection.Dispose()
        //   Connection = Nothing
        //End If
      }
      return dr;
    }

    public static SqlDataReader QueryReader(string Sql, SqlParameter SqlParam, string ConnectString)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return QueryReader(Sql, SqlParam, ConnectString, ref conn);
    }

    public static SqlDataReader QueryReader(string Sql, SqlParameter SqlParam, string ConnectString, ref SqlConnection Connection)
    {
      SqlDataReader dr = null;
      SqlCommand Command = null;
      bool CloseConnection = false;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(Sql, ConnectString, ref Connection);
      try
      {
        Command.Parameters.Add(SqlParam);
        try
        {
          if (CloseConnection)
          {
            dr = Command.ExecuteReader(CommandBehavior.CloseConnection);
          }
          else
          {
            dr = Command.ExecuteReader();
          }
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") & ex.Message.Contains("deadlocked"))
          {
            try
            {
              if (CloseConnection)
              {
                if (Command.Connection.State == ConnectionState.Closed)
                {
                  Command.Connection.Dispose();
                  Command.Connection = OpenConnection(ConnectString);
                }
              }
              else
              {
                return null;
              }

              if (CloseConnection)
              {
                dr = Command.ExecuteReader(CommandBehavior.CloseConnection);
              }
              else
              {
                dr = Command.ExecuteReader();
              }
            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            if (ThrowExceptions)
              throw ex;
          }
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;
      }
      return dr;
    }

    public static SqlDataReader QueryReader(string Sql, string ConnectString)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return QueryReader(Sql, ConnectString, ref conn);
    }

    public static SqlDataReader QueryReader(string Sql, string ConnectString, ref SqlConnection Connection)
    {
      ArrayList arParams = null;
      return QueryReader(Sql, arParams, ConnectString, ref Connection);
    }

    #endregion

    #region "Query Dataset"
    public static DataSet QueryDataSet(string Sql, string TableName, string ConnectString)
    {
      SqlConnection sqlconn = new SqlConnection(ConnectString);
      SqlCommand cmd = new SqlCommand(Sql, sqlconn);
      SqlDataAdapter da = new SqlDataAdapter(cmd);
      DataSet ds = new DataSet(TableName);
      try
      {
        sqlconn.Open();
        da.Fill(ds);
      }
      catch (SqlException sqlex)
      {
        throw sqlex;
      }
      finally
      {
        ConnectionDispose(ref sqlconn);
      }
      return ds;
    }
    #endregion

    #region "    QueryProc    "

    public static DataTable QueryProc(string Sql, ArrayList Params, string ConnectString, ref SqlConnection Connection)
    {
      DataTable dt = new DataTable();
      SqlCommand Command = null;
      bool CloseConnection = false;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(Sql, ConnectString, ref Connection);
      Command.CommandTimeout = CommandTimeout;
      //Added by Pat 4/11/06
      Command.CommandType = CommandType.StoredProcedure;
      SqlDataAdapter da = null;

      try
      {
        if ((Params != null))
        {
          foreach (SqlParameter Param in Params)
          {
            Command.Parameters.Add(Param);
          }
        }
        da = new SqlDataAdapter(Command);

        try
        {
          try
          {
            da.Fill(dt);

          }
          catch (System.Data.SqlClient.SqlException ex)
          {
            try
            {
              da.Fill(dt);

            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.

            }
          }
        }
        catch (ThreadAbortException tae)
        { throw tae; }
        catch (Exception ex)
        {
          //System.Net.Debug.LogError(ex, "Try/Catch Exception");
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;
        da.Dispose();
        da = null;

        if (CloseConnection)
        {
          Connection.Close();
          Connection.Dispose();
          Connection = null;
        }
      }
      return dt;
    }

    public static DataTable QueryProc(string Sql, SqlParameter SqlParam, string ConnectString, ref SqlConnection Connection)
    {
      DataTable dt = new DataTable();
      SqlCommand Command = null;
      bool CloseConnection = false;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(Sql, ConnectString, ref Connection);
      Command.CommandTimeout = CommandTimeout;
      //Added by Pat 4/11/06
      Command.CommandType = CommandType.StoredProcedure;

      SqlDataAdapter da = null;

      try
      {
        Command.Parameters.Add(SqlParam);
        da = new SqlDataAdapter(Command);
        try
        {
          try
          {
            da.Fill(dt);
          }
          catch (System.Data.SqlClient.SqlException ex)
          {
            try
            {
              da.Fill(dt);
            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
        }
        catch (ThreadAbortException tae)
        {
        }
        catch (Exception ex)
        {
          //System.Net.Debug.LogError(ex, "Try/Catch Exception");
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;

        da.Dispose();
        da = null;

        if (CloseConnection)
        {
          Connection.Close();
          Connection.Dispose();
          Connection = null;

        }
      }
      return dt;
    }
    #endregion

    #region "    Query Reader Proc  "

    public static SqlDataReader QueryReaderProc(string Sql, ArrayList Params, string ConnectString, ref SqlConnection Connection)
    {
      SqlDataReader dr = null;
      SqlCommand Command = null;
      bool CloseConnection = false;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(Sql, ConnectString, ref Connection);
      Command.CommandType = CommandType.StoredProcedure;

      try
      {
        if ((Params != null))
        {
          foreach (SqlParameter Param in Params)
          {
            Command.Parameters.Add(Param);
          }
        }
        try
        {
          if (CloseConnection)
          {
            dr = Command.ExecuteReader(CommandBehavior.CloseConnection);
          }
          else
          {
            dr = Command.ExecuteReader();
          }
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") & ex.Message.Contains("deadlocked"))
          {
            try
            {
              if (CloseConnection)
              {
                if (Command.Connection.State == ConnectionState.Closed)
                {
                  Command.Connection.Dispose();
                  Command.Connection = OpenConnection(ConnectString);
                }
              }
              else
              {
                return null;
              }

              if (CloseConnection)
              {
                dr = Command.ExecuteReader(CommandBehavior.CloseConnection);

              }
              else
              {
                dr = Command.ExecuteReader();
              }

            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            if (ThrowExceptions)
              throw ex;
          }
          throw ex;
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;
      }
      return dr;

    }

    public static SqlDataReader QueryReaderProc(string Sql, SqlParameter SqlParam, string ConnectString, ref SqlConnection Connection)
    {

      SqlDataReader dr = null;
      SqlCommand Command = null;
      bool CloseConnection = false;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(Sql, ConnectString, ref Connection);
      Command.CommandType = CommandType.StoredProcedure;

      try
      {
        Command.Parameters.Add(SqlParam);
        try
        {
          if (CloseConnection)
          {
            dr = Command.ExecuteReader(CommandBehavior.CloseConnection);
          }
          else
          {
            dr = Command.ExecuteReader();
          }
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") & ex.Message.Contains("deadlocked"))
          {
            try
            {
              if (CloseConnection)
              {
                if (Command.Connection.State == ConnectionState.Closed)
                {
                  Command.Connection.Dispose();
                  Command.Connection = OpenConnection(ConnectString);
                }
              }
              else
              {
                return null;
              }

              if (CloseConnection)
              {
                dr = Command.ExecuteReader(CommandBehavior.CloseConnection);
              }
              else
              {
                dr = Command.ExecuteReader();
              }
            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            if (ThrowExceptions)
              throw ex;

          }
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;

      }
      return dr;
    }

    public static SqlDataReader QueryReaderProc(string Sql, string ConnectString, ref SqlConnection Connection)
    {
      ArrayList arParams = null;
      return QueryReaderProc(Sql, arParams, ConnectString, ref Connection);
    }

    #endregion

    #region "    NonQuery Proc     "

    public static int NonQueryProc(string Sql, ArrayList Params, string ConnectString, ref SqlConnection Connection)
    {
      SqlCommand Command = null;
      bool CloseConnection = false;
      int Result = -1;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(Sql, ConnectString, ref Connection);
      Command.CommandType = CommandType.StoredProcedure;

      try
      {
        if ((Params != null))
        {
          foreach (SqlParameter Param in Params)
          {
            Command.Parameters.Add(Param);
          }
        }
        try
        {
          Result = Command.ExecuteNonQuery();
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") & ex.Message.Contains("deadlocked"))
          {
            try
            {
              Result = Command.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            if (ThrowExceptions)
              throw ex;
          }
        }

      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;

        if (CloseConnection)
        {
          Connection.Close();
          Connection.Dispose();
          Connection = null;
        }
      }
      return Result;
    }

    public static int NonQueryProc(string Sql, SqlParameter SqlParam, string ConnectString, ref SqlConnection Connection)
    {
      SqlCommand Command = null;
      bool CloseConnection = false;
      int Result = -1;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(Sql, ConnectString, ref Connection);
      Command.CommandType = CommandType.StoredProcedure;

      try
      {
        if ((SqlParam != null))
          Command.Parameters.Add(SqlParam);
        try
        {
          Result = Command.ExecuteNonQuery();
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") & ex.Message.Contains("deadlocked"))
          {
            try
            {
              Result = Command.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            if (ThrowExceptions)
              throw ex;
          }
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;

        if (CloseConnection)
        {
          Connection.Close();
          Connection.Dispose();
          Connection = null;
        }
      }
      return Result;
    }

    #endregion

    #region "    NonQuery     "

    public static int NonQuery(string Sql, string ConnectString)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return NonQuery(Sql, ConnectString, ref conn);
    }

    public static int NonQuery(string Sql, string ConnectString, ref SqlConnection Connection)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return NonQuery(Sql, (ArrayList)null, ConnectString, ref conn);
    }

    public static int NonQuery(string Sql, ArrayList Params, string ConnectString)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return NonQuery(Sql, Params, ConnectString, ref conn, 0);
    }

    public static int NonQuery(string Sql, ArrayList Params, string ConnectString, int CommandTimeout)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return NonQuery(Sql, Params, ConnectString, ref conn, CommandTimeout);
    }

    public static int NonQuery(string Sql, ArrayList Params, string ConnectString, ref SqlConnection Connection)
    {
      return NonQuery(Sql, Params, ConnectString, ref Connection, 0);
    }

    public static int NonQuery(string Sql, ArrayList Params, string ConnectString, ref SqlConnection Connection, int CommandTimeout)
    {

      SqlCommand Command = null;
      bool CloseConnection = false;
      int Result = -1;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(Sql, ConnectString, ref Connection);

      if (CommandTimeout > 0)
      {
        Command.CommandTimeout = CommandTimeout;
      }

      try
      {
        if ((Params != null))
        {
          foreach (SqlParameter Param in Params)
          {
            Command.Parameters.Add(Param);

          }
        }
        try
        {
          Result = Command.ExecuteNonQuery();

        }
        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") & ex.Message.Contains("deadlocked"))
          {
            try
            {
              Result = Command.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            if (ThrowExceptions)
              throw ex;
          }
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;

        if (CloseConnection)
        {
          Connection.Close();
          Connection.Dispose();
          Connection = null;
        }
      }
      return Result;
    }

    public static int NonQuery(string Sql, SqlParameter SqlParam, string ConnectString)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return NonQuery(Sql, SqlParam, ConnectString, ref conn);
    }

    public static int NonQuery(string Sql, SqlParameter SqlParam, string ConnectString, ref SqlConnection Connection)
    {
      ArrayList @params = new ArrayList();
      @params.Add(SqlParam);
      return NonQuery(Sql, @params, ConnectString);
    }

    #endregion

    #region "    Scalar    "

    public static object Scalar(string SQL, ArrayList Params, string ConnectString)
    {
      SqlConnection conn = new SqlConnection(ConnectString);
      return Scalar(SQL, Params, ConnectString, ref conn, 0);
    }

    public static object Scalar(string SQL, ArrayList Params, string ConnectString, ref SqlConnection Connection)
    {
      return Scalar(SQL, Params, ConnectString, ref Connection, 0);
    }

    public static object Scalar(string SQL, ArrayList Params, string ConnectString, ref SqlConnection Connection, int CommandTimeOut)
    {
      SqlCommand Command = null;
      bool CloseConnection = false;
      object Result = null;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(SQL, ConnectString, ref Connection);

      if (CommandTimeOut > 0)
      {
        Command.CommandTimeout = CommandTimeOut;
      }
      try
      {
        if ((Params != null))
        {
          foreach (SqlParameter Param in Params)
          {
            Command.Parameters.Add(Param);

          }
        }
        try
        {
          Result = Command.ExecuteScalar();
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") & ex.Message.Contains("deadlocked"))
          {
            try
            {
              Result = Command.ExecuteScalar();

            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            throw ex;
          }
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;

        if (CloseConnection)
        {
          Connection.Close();
          Connection.Dispose();
          Connection = null;
        }
      }
      return Result;
    }

    public static object Scalar(string SQL, SqlParameter SqlParam, string ConnectString, ref SqlConnection Connection)
    {
      SqlCommand Command = null;
      bool CloseConnection = false;
      object Result = null;

      if (Connection == null)
        CloseConnection = true;

      Command = OpenCommand(SQL, ConnectString, ref Connection);
      try
      {
        Command.Parameters.Add(SqlParam);
        try
        {
          Result = Command.ExecuteScalar();
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") & ex.Message.Contains("deadlocked"))
          {
            try
            {
              Result = Command.ExecuteScalar();

            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            throw ex;
          }
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;

        if (CloseConnection)
        {
          Connection.Close();
          Connection.Dispose();
          Connection = null;
        }
      }
      return Result;
    }

    public static object Scalar(string SQL, string ConnectString, ref SqlConnection Connection)
    {

      ArrayList arParams = null;
      return Scalar(SQL, arParams, ConnectString, ref Connection);
    }

    #endregion

    #region "    Scalar Proc    "

    public static object ScalarProc(string SQL, ArrayList Params, string ConnectString, ref SqlConnection Connection)
    {
      SqlCommand Command = null;
      bool CloseConnection = false;
      object Result = null;

      if (Connection == null)
        CloseConnection = true;
      Command = OpenCommand(SQL, ConnectString, ref Connection);
      Command.CommandType = CommandType.StoredProcedure;

      try
      {
        if ((Params != null))
        {
          foreach (SqlParameter Param in Params)
          {
            Command.Parameters.Add(Param);
          }
        }
        try
        {
          Result = Command.ExecuteScalar();
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
          if (ex.Message.Contains("Transaction") & ex.Message.Contains("deadlocked"))
          {
            try
            {
              Result = Command.ExecuteScalar();
            }
            catch (System.Data.SqlClient.SqlException RedundantEx)
            {
              //Do nothing. We're going to allow the method to return an empty object.
            }
          }
          else
          {
            throw ex;
          }
        }
      }
      finally
      {
        Command.Parameters.Clear();
        Command.Dispose();
        Command = null;

        if (CloseConnection)
        {
          Connection.Close();
          Connection.Dispose();
          Connection = null;
        }
      }
      return Result;
    }

    #endregion

    #region "   Logical/Utility Methods    "

    public static bool DataTableEmpty(ref DataTable dt)
    {
      if (dt == null)
        return true;
      if (dt.Rows.Count <= 0)
        return true;
      if (dt.Rows[0] == null)
        return true;

      return false;
    }

    public static bool DataReaderEmpty(ref SqlDataReader dr)
    {
      if (dr == null)
        return true;
      if (dr.IsClosed)
        return true;
      if (dr.HasRows == false)
        return true;

      return false;
    }

    public static void DataTableDispose(ref DataTable dt)
    {
      if ((dt != null))
        dt.Dispose();
    }


    public static void DataSetDispose(ref DataSet ds)
    {
      if (ds == null)
        return;
      ds.Dispose();
      ds = null;
    }


    public static void DataReaderDispose(ref SqlDataReader dr)
    {
      if ((dr != null))
      {
        if (!dr.IsClosed)
        {
          dr.Close();
          dr.Dispose();
          dr = null;
        }
      }
    }

    public static void DataReaderDispose(ref SqlDataReader dr, ref SqlConnection Connection)
    {
      if ((dr != null))
      {
        if (!dr.IsClosed)
          dr.Close();
        dr = null;
      }

      if ((Connection != null))
      {
        if (Connection.State == ConnectionState.Open)
          Connection.Close();
        Connection.Dispose();
        Connection = null;
      }
    }

    public static DataTable DataTableSelect(DataTable dt, string Filter, string Sort, int RowCount)
    {
      if (dt == null)
        return new DataTable();

      DataTable ResultDt = null;
      DataView dv = new DataView(dt);

      if (Filter.Trim() != string.Empty)
        dv.RowFilter = Filter;

      dv.Sort = Sort.Trim();
      ResultDt = dv.Table.Clone();

      //Go Through Rows Of Dataview
      foreach (DataRowView drv in dv)
      {
        //go through each column of the dataview and create a corresponding datarow in the datatable
        DataRow dr = ResultDt.NewRow();
        for (int x = 0; x <= dv.Table.Columns.Count - 1; x++)
        {
          //Add Each Cell To The Data
          dr[x] = drv[x];
        }
        ResultDt.Rows.Add(dr);
        if (ResultDt.Rows.Count == RowCount)
          break; // TODO: Might Not Be Correct. Was : Exit For
      }
      DataTableDispose(ref dt);
      return ResultDt;
    }
    #endregion

    /// <summary>
    /// Escapes strings making them safe for raw SQL input.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// 
    public static string Escape(string value)
    {
      if (string.IsNullOrEmpty(value))
        return value;

      value = value.Replace("'", "''");
      return value;
    }

    /// <summary>
    /// Escapes strings making them safe for raw SQL input and for use in a LIKE comparison.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>
    /// Created, Andrew Powell - 02.06.08
    /// </remarks>
    public static string EscapeLike(string value)
    {
      if (string.IsNullOrEmpty(value))
        return value;

      value = Escape(value);
      value = value.Replace("%", "[%]");
      value = value.Replace("_", "[_]");

      return value;
    }

    /*
      /// <summary>
      ///   Usage: Set up a object instance as List( Of YourObject) we will call it ListInstance.
      ///   vb  ListInstance = FillGenObject(Of YourObject, SqlDataReader)(New YourObject(), dr)
      ///   cs  Wrap [ (Of ] with a "Less Than" and wrap SqlDataReader) to be SqlDataReader + "Greater Than"
      ///   cs  ListInstance = FillGenObject<YourObject, SqlDataReader>(New YourObject(), dr)
      ///   Where DR is an instance Example a SqlDataReader.
      /// </summary>
      /// <typeparam name="T">Object type to Add to the List ie. Product</typeparam>
      /// <typeparam name="U">SqlDataReader</typeparam>
      /// <param name="Obj">NEW + Object to Add to the List (new Product)</param>
      /// <param name="dr">Data Reader Instance</param>
      /// <returns>List(Of YourObject)</returns>
      /// <remarks></remarks>
     */
    public static List<T> FillGenObjectDR<T, U>(T Obj, SqlDataReader dr)
    {
      ColumnCollection columns = new ColumnCollection(dr, false);
      List<T> ObjList = new List<T>();
      T drDataRow = default(T);

      Type ObjType = typeof(T);
      PropertyInfo[] p = ObjType.GetProperties();
      object[] args = { };
      object[] custAttr = null;
      //string output = null;
      while (dr.Read())
      {
        drDataRow = (T)System.Activator.CreateInstance(ObjType, args);

        foreach (PropertyInfo pi in p)
        {
          try
          {
            custAttr = pi.GetCustomAttributes(true);
            Type AttrType = custAttr.GetType();
            if (columns.ContainsKey(pi.Name.ToLower()) && (custAttr.Length == 0))
            {
              if (!(dr[pi.Name] is System.DBNull))
              {
                //if (pi.PropertyType.Name == "DateTime" && dr[pi.Name].GetType().Name == "Int64")
                //{
                //  // Hack to fix datetimes saved as ticks
                //  pi.SetValue(drDataRow, new DateTime((long)dr[pi.Name]), null);
                //}
                //else
                //{
                //output = string.Concat((pi.Name), ";", dr[pi.Name]);
                //System.Diagnostics.Debug.WriteLine(output);
                pi.SetValue(drDataRow, dr[pi.Name], null);
                //}
              }
              else
              {
                pi.SetValue(drDataRow, dr[pi.Name], null);
              }
            }
          }
          catch (System.IndexOutOfRangeException generatedExceptionName)
          {
          }
          catch (System.ArgumentException ArgEx)
          {
            System.Diagnostics.Debug.WriteLine(ArgEx.Message);
          }
        }
        ObjList.Add(drDataRow);
      }
      // Close the DataReader
      //dr.Close()
      //Return the object List(Of T)
      return ObjList;
    }

    /// <summary>
    /// Usage: Set up a object instance as List( Of YourObject) we will call it ListInstance.
    ///           ListInstance = FillGenObject(Of YourObject, Instance of your DataSet)(New YourObject(), ds)
    /// vb        ListInstance = FillGenObject(Of YourObject, SqlDataReader)(New YourObject(), dr)
    /// cs        Wrap [ (Of ] with a "Less Than"  and wrap SqlDataReader) to be SqlDataReader + "Greater Than"
    /// cs        ListInstance = FillGenObject<YourObject, SqlDataReader>(New YourObject(), dr)
    /// Where DS is an instance Example a DataSet.
    /// </summary>
    /// <typeparam name="T">Object to Add to the List.  ie. Product</typeparam>
    /// <typeparam name="U">SqlDataReaderInstance</typeparam>
    /// <param name="Obj">Object to Add to the List. ie new Product</param>
    /// <param name="ds">Data Set Instance with rows.  </param>
    /// <returns>List(Of YourObject)</returns>
    /// <remarks></remarks>
    public static List<T> FillGenObjectDS<T, U>(T Obj, DataSet ds)
    {
      ColumnCollection columns = new ColumnCollection(ds, false);
      List<T> ObjList = new List<T>();
      T dsDataRow = default(T);
      //-- =====
      Type ObjType = typeof(T);
      string output = null;

      PropertyInfo[] p = ObjType.GetProperties();
      object[] args = { };
      object[] custAttr = null;

      if (ds == null)
         return null;

      for (int I = 0; I <= ds.Tables[0].Rows.Count - 1; I++)
      {
        dsDataRow = (T)Activator.CreateInstance(ObjType, args);
        DataRow dds = ds.Tables[0].Rows[I];

        foreach (PropertyInfo pi in p)
        {
          try
          {
            custAttr = pi.GetCustomAttributes(true);
            if (columns.ContainsKey(pi.Name.ToLower()) && (custAttr.Length == 0))
            {
              if ((dds[pi.Name] != null) && (custAttr.Length == 0))
              {
                if (!(dds[pi.Name] is System.DBNull))
                {
                  //if (pi.PropertyType.Name == "DateTime" && dds[pi.Name].GetType().Name == "Int64")
                  //{
                  //  // Hack to fix datetimes saved as ticks
                  //  pi.SetValue(drDataRow, new DateTime((long)dds[pi.Name]), null);
                  //}
                  //else
                  //{
                  //  pi.SetValue(drDataRow, dds[pi.Name], null);
                  //}

                  //output = string.Concat((pi.Name), ";", dds[pi.Name]);
                  //System.Diagnostics.Debug.WriteLine(output);
                  pi.SetValue(dsDataRow, dds[pi.Name], null);

                }
                else
                {
                  pi.SetValue(dsDataRow, dds[pi.Name], null);
                  //pi.SetValue(dsDataRow, dds[pi.Name].ToString(), null);

                }
              }
            }
          }
          catch (System.IndexOutOfRangeException OutofRangeEX)
          { throw OutofRangeEX; }
          catch (System.Exception SysEx)
          { throw SysEx; }

        }
        ObjList.Add(dsDataRow);
      }
      return ObjList;
    }

    //public static DataTable ToDataTable<T>(this IList<T> data)
    public static DataTable ToDataTable<T>(IList<T> data)
    {
      PropertyDescriptorCollection props =
          TypeDescriptor.GetProperties(typeof(T));

      DataTable table = new DataTable();
      for (int i = 0; i < props.Count; i++)
      {
        PropertyDescriptor prop = props[i];
        table.Columns.Add(prop.Name, prop.PropertyType);
      }

      object[] values = new object[props.Count];
      foreach (T item in data)
      {
        for (int i = 0; i < values.Length; i++)
        {
          if (props[i].PropertyType == typeof(DateTime))
          {
            DateTime currDT = (DateTime)props[i].GetValue(item);
            values[i] = currDT.ToUniversalTime();
          }
          else
          {
            values[i] = props[i].GetValue(item);
          }
        }
        table.Rows.Add(values);
      }
      return table;
    }
    /*public static void FillObject(object Obj, SqlDataReader Reader)
      {
          System.Reflection.PropertyInfo[] Properties = Obj.GetType().GetProperties();
          ColumnCollection Columns = new ColumnCollection(Reader);
          dynamic TypeProp = Obj.GetType();

          for (int PropertyIndex = 0; PropertyIndex <= Properties.Length - 1; PropertyIndex++)
          {
              System.Reflection.PropertyInfo PropInfo = Properties[PropertyIndex];

              if (!PropInfo.CanWrite)
                  continue;

              object Value = null;
              if (object.ReferenceEquals(PropInfo.PropertyType, typeof(string)))
              {
                  Value = string.Empty;
              }
              else if (object.ReferenceEquals(PropInfo.PropertyType, typeof(DateTime)))
              {
                  Value = new DateTime(0);
              }
              else if (object.ReferenceEquals(PropInfo.PropertyType, typeof(System.DateTime)))
              {
                  Value = new System.DateTime(0);
              }
              else if (PropInfo.PropertyType.IsValueType)
              {
                  Value = Activator.CreateInstance(PropInfo.PropertyType);

                  //We really dont want to be dealing with non-valuetypes
                  //ElseIf Not PropInfo.PropertyType.GetConstructor(New Type() {}) Is Nothing Then
                  //   Value = Activator.CreateInstance(PropInfo.PropertyType)
              }
              else
              {
                  continue;
              }
              Columns.Fetch(PropInfo.Name, ref Value);
              if (( Value != null ))
                  PropInfo.SetValue(Obj, Value, null);
          }
      }
     */
  }
}
