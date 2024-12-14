using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.Reflection;

namespace OA.Infrastructure.SQL
{
    public class BaseConnection
    {
        private static BaseConnection? _instance;
        private string? _connectionString { get; set; }

        private BaseConnection(IConfiguration? configuration)
        {
            _connectionString = configuration?.GetConnectionString(nameof(ConnectionStrings.DefaultConnection));
        }
        public static BaseConnection Instance(IConfiguration? configuration = null)
        {
            return _instance ?? (_instance = new BaseConnection(configuration));
        }
        public SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;
        }
        public SqlCommand GetCommand(DbConnection connection, string commandText, CommandType commandType)
        {
            SqlCommand command = new SqlCommand(commandText, connection as SqlConnection);
            command.CommandType = commandType;
            return command;
        }
        public SqlParameter GetParameter(string parameter, object value)
        {
            SqlParameter parameterObject = new SqlParameter(parameter, value != null ? value : DBNull.Value);
            parameterObject.Direction = ParameterDirection.Input;
            return parameterObject;
        }
        public SqlParameter GetParameterOut(string parameter, SqlDbType type, object? value = null, ParameterDirection parameterDirection = ParameterDirection.InputOutput)
        {
            SqlParameter parameterObject = new SqlParameter(parameter, type);
            if (type == SqlDbType.NVarChar || type == SqlDbType.VarChar || type == SqlDbType.NText || type == SqlDbType.Text)
            {
                parameterObject.Size = -1;
            }
            parameterObject.Direction = parameterDirection;
            if (value != null)
            {
                parameterObject.Value = value;
            }
            else
            {
                parameterObject.Value = DBNull.Value;
            }
            return parameterObject;
        }

        public List<DbParameter> GetParametersFromModel(BaseConnection dbConnectSQL, object model, List<string> excludePropertyNames)
        {
            var parameters = new List<DbParameter>();
            var properties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (excludePropertyNames != null && excludePropertyNames.Contains(property.Name))
                {
                    continue;
                }

                var parameterName = "@" + property.Name;
                var parameterValue = property.GetValue(model);
                var parameter = dbConnectSQL.GetParameter(parameterName, parameterValue ?? new object());
                parameters.Add(parameter);
            }

            return parameters;
        }

        public int ExecuteNonQuery(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            int returnValue = -1;
            using (SqlConnection connection = GetConnection())
            {
                try
                {
                    SqlCommand cmd = GetCommand(connection, procedureName, commandType);
                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    returnValue = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            return returnValue;
        }
        public IDataReader ExecuteReader(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = GetConnection())
            {
                try
                {
                    SqlCommand cmd = GetCommand(connection, procedureName, commandType);
                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    SqlDataReader sqlDr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    dt.Load(sqlDr);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            return dt.CreateDataReader();
        }
        public DataTable? ExecuteTable(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            DataTable? dt = new DataTable();
            using (SqlConnection connection = GetConnection())
            {
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd = GetCommand(connection, procedureName, commandType);
                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    var sqlDa = new SqlDataAdapter(cmd);
                    sqlDa.Fill(ds);
                    dt = (ds.Tables != null && ds.Tables.Count > 0) ? ds.Tables[0] : null;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            return dt;
        }
        public DataSet ExecuteDataSet(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = GetConnection())
            {
                try
                {
                    SqlCommand cmd = GetCommand(connection, procedureName, commandType);
                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    var sqlDa = new SqlDataAdapter(cmd);
                    sqlDa.Fill(ds);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            return ds;
        }
        public DataSet ExecuteDataSet(string procedureName, List<DbParameter> parameters, DataTable dataTables, string nameOfDataTable, CommandType commandType = CommandType.StoredProcedure)
        {
            DataSet ds = new();
            using (SqlConnection connection = GetConnection())
            {
                try
                {
                    SqlCommand cmd = GetCommand(connection, procedureName, commandType);
                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    if (dataTables != null)
                    {
                        SqlParameter tvparam = cmd.Parameters.AddWithValue(nameOfDataTable, dataTables);
                        tvparam.SqlDbType = SqlDbType.Structured;
                    }
                    var sqlDa = new SqlDataAdapter(cmd);
                    sqlDa.Fill(ds);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            return ds;
        }
        public List<Dictionary<string, object>> ConvertToListDictionary(DataTable dataTable)
        {
            var columns = new List<string>();
            var rows = new List<Dictionary<string, object>>();
            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                columns.Add(dataTable.Columns[i].ColumnName);
            }
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                rows.Add(
                    columns.ToDictionary(
                        column => column,
                        column => (object?)((dataTable.Rows[i][column] != null && dataTable.Rows[i][column].ToString() != "") ? dataTable.Rows[i][column] : null))
                        as Dictionary<string, object>
                    );
            }
            return rows;
        }
        public List<dynamic> ConvertToListDynamic(DataTable dataTable)
        {
            var resultObject = new List<dynamic>();
            var rowCount = dataTable.Rows.Count;
            var colCount = dataTable.Columns.Count;
            for (int row = 0; row < rowCount; row++)
            {
                var dataRow = new ExpandoObject() as IDictionary<string, object>;
                for (int col = 0; col < colCount; col++)
                {
                    var fieldName = dataTable.Columns[col].ColumnName.ToString().Trim();
                    var valueCell = (dataTable.Rows[row][col] != null && dataTable.Rows[row][col].ToString() != string.Empty) ? dataTable.Rows[row][col] : null;
                    dataRow.Add(fieldName, valueCell ?? "");
                }
                resultObject.Add(dataRow);
            }
            return resultObject;
        }
        public List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public string? ExecuteScalar(string sqlQuery)
        {
            string? result = string.Empty;
            using (SqlConnection connection = GetConnection())
            {
                SqlCommand cmd = GetCommand(connection, sqlQuery, CommandType.Text);
                var resultExec = cmd.ExecuteScalar();
                if (resultExec != null)
                {
                    result = resultExec.ToString();
                }
            }
            return result;
        }
        private class ConnectionStrings
        {
            public string? DefaultConnection { get; set; }
        }
        private T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName && !dr.IsNull(column.ColumnName))
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}
