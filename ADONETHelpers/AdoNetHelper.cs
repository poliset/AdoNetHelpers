using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ADONETHelpers.Ado
{
    public class AdoNetHelper : IDisposable
    {
        private bool _disposed;
        private SqlConnection _sqlConnection;
        private SqlCommand _sqlCommand;
        private SqlDataReader _sqlDataReader;
        private AdoNetConnectionStatistics _adoNetConnectionStatistics;
        private bool _enableStatistics;


        public AdoNetHelper(string connectionString)
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder;
            try
            {
                sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
                _sqlConnection = new SqlConnection(connectionString);
#if DEBUG
                _sqlConnection.StatisticsEnabled = _enableStatistics = true;
                _sqlConnection.StateChange += _sqlConnection_StateChange;
#endif
                _sqlCommand = new SqlCommand();
                _sqlCommand.Connection = _sqlConnection;

            }
            catch (KeyNotFoundException ex)
            {
                throw;
            }
            catch (FormatException ex)
            {
                throw;
            }
            catch (ArgumentException ex)
            {
                throw;
            }
        }

        void _sqlConnection_StateChange(object sender, StateChangeEventArgs e)
        {
            if (e.CurrentState == ConnectionState.Closed)
            {
                if (((SqlConnection)sender).StatisticsEnabled)
                {
                    _adoNetConnectionStatistics = new AdoNetConnectionStatistics(((SqlConnection)sender).RetrieveStatistics());
                }
            }
        }

        public void AddInParameter(string parameterName, SqlDbType sqlDbType, object value)
        {
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = "@" + parameterName;
            sqlParameter.SqlDbType = sqlDbType;
            sqlParameter.SqlValue = value;
            sqlParameter.Direction = ParameterDirection.Input;

            _sqlCommand.Parameters.Add(sqlParameter);
        }

        public void AddInTableTypeParameter<T>(string parameterName, List<T> data)
        {
            DataTable tableValuedParameterData = data.MapBusinessEntitiesCollectionToDataTable<T>();

            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = "@" + parameterName;
            sqlParameter.SqlDbType = SqlDbType.Structured;
            sqlParameter.Value = tableValuedParameterData;
            sqlParameter.Direction = ParameterDirection.Input;
            _sqlCommand.Parameters.Add(sqlParameter);
        }


        public void SqlStoreProcedureCommand(string storeProcedureName)
        {
            _sqlCommand.CommandType = CommandType.StoredProcedure;
            _sqlCommand.CommandText = storeProcedureName;
        }

        public void ExecuteStoreProcedure()
        {
            ExecuteSqlCommand(() =>
            {
                _sqlConnection.Open();
                _sqlCommand.ExecuteNonQuery();
            });
        }

        public int ExecuteStoreProcedure()
        {
            return ExecuteSqlCommand(() =>
            {
                _sqlConnection.Open();
                return (int)_sqlCommand.ExecuteScalar();
            });
        }
        public List<T> ExecuteStoreProcedure<T>() where T : new()
        {
            return ExecuteSqlCommand(() =>
            {
                _sqlConnection.Open();
                _sqlDataReader = _sqlCommand.ExecuteReader();
                if (_sqlDataReader.HasRows)
                {
                    return _sqlDataReader.MapDataToBusinessEntityCollection<T>();
                }
                return new List<T>();
            });
        }
        private void ExecuteSqlCommand(Action codeToExecute)
        {
            try
            {
                codeToExecute.Invoke();
            }
            catch (InvalidCastException ex)
            {

                throw;
            }
            /* 
             * Severity : 11 and 25 are handled ( no Try Catch in sql query ) 
             * with Try Catch in Sql Query : Severity between 11 and 19 are caught by Catch block in SQl Statement and rethrown , >=20 are raised  directly to ADO.NET
             */
            catch (SqlException ex)
            {
                // Extenstion method
                string errorDetails = ex.FullMessage();
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                throw;
            }
            catch (InvalidOperationException ex)
            {
                throw;
            }
            catch (IOException ex)
            {
                throw;
            }
            finally
            {
                Dispose(true);
            }
        }

        private T ExecuteSqlCommand<T>(Func<T> codeToExecute)
        {
            try
            {
                return codeToExecute.Invoke();
            }
            catch (InvalidCastException ex)
            {

                throw;
            }
            catch (SqlException ex)
            {
                string errorDetails = ex.FullMessage();
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                throw;
            }
            catch (InvalidOperationException ex)
            {
                throw;
            }
            catch (IOException ex)
            {
                throw;
            }
            finally
            {
                Dispose(true);
            }
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_sqlConnection != null)
                    {
                        _sqlConnection.Dispose();
                        _sqlConnection = null;
                    }
                    if (_sqlCommand != null)
                    {
                        _sqlCommand.Dispose();
                        _sqlCommand = null;
                    }
                    if (_sqlDataReader != null)
                    {
                        _sqlDataReader.Dispose();
                        _sqlDataReader = null;
                    }
                }
                _disposed = true;
            }
        }
    }
}
