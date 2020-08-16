using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Security.Principal;
using System.Net;

public class DataAccess
{
    #region Objects & Variables
   public string _sourcePath;
    private SqlConnection _sqlCon;
    private SqlDataAdapter _sqlDa;
    private DataSet _dataSet;
    #endregion

    #region ExecuteToDataSet
    public DataSet ExecuteToDataSet(SqlCommand sqlCmd)
    {
        try
        {
            _dataSet = new DataSet();
            _sqlCon = new SqlConnection(_sourcePath);
            _sqlCon.Open();
            sqlCmd.Connection = _sqlCon;
            _sqlDa = new SqlDataAdapter(sqlCmd);
            _sqlDa.Fill(_dataSet);         
            sqlCmd.Dispose();
            return _dataSet;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            _sqlCon.Close();
            _sqlCon.Dispose(); 
            _sqlDa.Dispose();
        }
    }
    #endregion

    #region DataAccess
    public DataAccess()
    {
        _sourcePath = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
    }

    public static SqlConnection GetConnection()
    {
        SqlConnection _sourcePath = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString());
        return _sourcePath;
    }

    #endregion DataAccess

    #region ExecuteToInt
    public int ExecuteToInt(SqlCommand sqlCmd)
    {
        try
        {
            _sqlCon = new SqlConnection(_sourcePath);
            _sqlCon.Open();
            sqlCmd.Connection = _sqlCon;
            var retVal = sqlCmd.ExecuteNonQuery();
            _sqlCon.Close();
            sqlCmd.Dispose();
            return retVal;
        }
        catch (Exception ex)
        {
            if (((SqlException)ex).Number == 2627)
            {

                throw new Exception("Duplicate Username", ex);
            }
            else
            {
                throw new Exception(ex.Message);
            }
        }

        finally
        {
            _sqlCon.Dispose();
        }
    }
    #endregion

    #region ExecuteToObject

    public object ExecuteToObject(SqlCommand sqlCmd)
    {
        try
        {
            _sqlCon = new SqlConnection(_sourcePath);
            _sqlCon.Open();
            sqlCmd.Connection = _sqlCon;
            var retObj = sqlCmd.ExecuteScalar();
            _sqlCon.Close();
            sqlCmd.Dispose();
            return retObj;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            _sqlCon.Dispose();
        }
    }
    #endregion ExecuteToObject		

    public static string sqlConnectionString = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
    // Method for update 
    public int Updatedb(SqlCommand cmd)
    {
        try
        {
            using (var scon = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                cmd.Connection = scon;
                scon.Open();
                var i = cmd.ExecuteNonQuery();
                scon.Close();
                cmd.Dispose();
                return i;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static string ExecuteSPScalar(string sp_name, params SqlParameter[] cmdParams)
    {
        string sRetVal = "";
        SqlCommand cmd = new SqlCommand();
        SqlConnection conn = new SqlConnection(sqlConnectionString);
        System.Data.SqlClient.SqlDataAdapter adapter = new SqlDataAdapter();
        try
        {
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sp_name;
            cmd.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand = cmd;
            if (cmdParams != null)
            {
                foreach (SqlParameter param in cmdParams)
                    cmd.Parameters.Add(param);
            }
            System.Data.DataSet ds = new DataSet();
            adapter.Fill(ds);
            cmd.Dispose();
            conn.Close();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                    sRetVal = Convert.ToString(ds.Tables[0].Rows[0][0]);
                else
                    sRetVal = "";

            }
            else
                sRetVal = "";
            ds.Dispose();
            return sRetVal;
        }
        catch
        {
            conn.Close();
            throw;
        }

    }
    public static System.Data.DataTable ExecuteSPDataTable(string sp_name, params SqlParameter[] cmdParams)
    {
        SqlCommand cmd = new SqlCommand();
        SqlConnection conn = new SqlConnection(sqlConnectionString);
        System.Data.SqlClient.SqlDataAdapter adapter = new SqlDataAdapter();
        try
        {

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sp_name;
            cmd.CommandType = CommandType.StoredProcedure;

            adapter.SelectCommand = cmd;

            if (cmdParams != null)
            {
                foreach (SqlParameter param in cmdParams)
                    cmd.Parameters.Add(param);
            }

            System.Data.DataTable ds = new DataTable();
            adapter.Fill(ds);
            cmd.Dispose();
            conn.Close();
            return ds;
        }
        catch
        {
            conn.Close();
            throw;
        }
    }
    public static System.Data.DataTable ExecuteSPDataTable(string sp_name)
    {
        SqlCommand cmd = new SqlCommand();
        SqlConnection conn = new SqlConnection(sqlConnectionString);
        System.Data.SqlClient.SqlDataAdapter adapter = new SqlDataAdapter();
        try
        {

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sp_name;
            cmd.CommandType = CommandType.StoredProcedure;

            adapter.SelectCommand = cmd;

            System.Data.DataTable ds = new DataTable();
            adapter.Fill(ds);
            cmd.Dispose();
            conn.Close();
            return ds;
        }
        catch
        {
            conn.Close();
            throw;
        }
    }
    public static System.Data.DataSet ExecuteSPDataSet(string sp_name, params SqlParameter[] cmdParams)
    {
        SqlCommand cmd = new SqlCommand();
        SqlConnection conn = new SqlConnection(sqlConnectionString);
        System.Data.SqlClient.SqlDataAdapter adapter = new SqlDataAdapter();
        try
        {
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sp_name;
            cmd.CommandType = CommandType.StoredProcedure;

            adapter.SelectCommand = cmd;

            if (cmdParams != null)
            {
                foreach (SqlParameter param in cmdParams)
                    cmd.Parameters.Add(param);
            }

            System.Data.DataSet ds = new DataSet();
            adapter.Fill(ds);
            cmd.Dispose();
            conn.Close();
            return ds;
        }
        catch
        {
            conn.Close();
            throw;
        }
    }
   
}

