using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace dp.data.AdoNet.SqlExecution
{
    public class SqlQueryExecutor
    {
        private const string ReturnParameterName = "returnCode";

        private readonly string _connectionString;


        public SqlQueryExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Returns only stored procedure out parameters.
        /// </summary>
        public async Task<T> ExecuteAsync<T>(SqlQuery query, Func<IDictionary<string, object>, T> getQueryResult)
        {
            ExecuteProcedureResult<T> result = await ExecuteProcedure(SqlQueryResultType.OutParams, query, getQueryOutParamsResult: getQueryResult);

            return result.SelectedResult;
        }

        /// <summary>
        /// Returns stored procedure out parameters and returns the SQL RETURN code.
        /// </summary>
        public async Task<ExecuteProcedureResult<T>> ExecuteAndGetReturnedCodeAsync<T>(SqlQuery query, Func<IDictionary<string, object>, T> getQueryResult)
        {
            ExecuteProcedureResult<T> result = await ExecuteProcedure(SqlQueryResultType.OutParams, query, getQueryOutParamsResult: getQueryResult, getReturnedValue: true);

            return result;
        }

        /// <summary>
        /// Returns only stored procedure data.
        /// </summary>
        public async Task<T> ExecuteAsync<T>(SqlQuery query, Func<IDataReader, T> getQueryResult)
        {
            ExecuteProcedureResult<T> result = await ExecuteProcedure(SqlQueryResultType.Data, query, getQueryDataResult: getQueryResult);

            return result.SelectedResult;
        }

        /// <summary>
        /// Returns stored procedure data and returns the SQL RETURN code.
        /// </summary>
        public async Task<ExecuteProcedureResult<T>> ExecuteAndGetReturnedCodeAsync<T>(SqlQuery query, Func<IDataReader, T> getQueryResult)
        {
            ExecuteProcedureResult<T> result = await ExecuteProcedure(SqlQueryResultType.Data, query, getQueryDataResult: getQueryResult, getReturnedValue: true);

            return result;
        }

        /// <summary>
        /// Executes without returning any data.
        /// </summary>
        public async Task ExecuteAsync(SqlQuery query)
        {
            await ExecuteProcedure<bool>(SqlQueryResultType.None, query);
        }

        /// <summary>
        /// Executes and returns the SQL RETURN code.
        /// </summary>
        public async Task<int?> ExecuteAndGetReturnedCodeAsync(SqlQuery query)
        {
            ExecuteProcedureResult<bool> result = await ExecuteProcedure<bool>(SqlQueryResultType.None, query, getReturnedValue: true);

            return result.ReturnedCode;
        }

        #region Private Helpers

        private async Task<ExecuteProcedureResult<T>> ExecuteProcedure<T>(SqlQueryResultType resultType, SqlQuery query,
           Func<IDataReader, T> getQueryDataResult = null, Func<IDictionary<string, object>, T> getQueryOutParamsResult = null, bool getReturnedValue = false)
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        if (getReturnedValue)
                        {
                            query.Params.Add(new SqlParameter(ReturnParameterName, SqlDbType.Int)
                            {
                                Direction = ParameterDirection.ReturnValue
                            });
                        }

                        SetupCommand(cmd, query);
                        await con.OpenAsync();

                        var result = new ExecuteProcedureResult<T>();
                        switch (resultType)
                        {
                            case SqlQueryResultType.Data:
                                {
                                    var reader = await cmd.ExecuteReaderAsync();

                                    result.SelectedResult = getQueryDataResult(reader);

                                    reader.Close();

                                    break;
                                }

                            case SqlQueryResultType.OutParams:
                                {
                                    await cmd.ExecuteNonQueryAsync();
                                    var outParams = GetOutputParametrs(cmd.Parameters);

                                    result.SelectedResult = getQueryOutParamsResult(outParams);

                                    break;
                                }

                            case SqlQueryResultType.None:
                                {
                                    await cmd.ExecuteNonQueryAsync();

                                    break;
                                }

                            default:
                                throw new NotImplementedException();
                        }

                        if (getReturnedValue)
                            result.ReturnedCode = GetReturnedValue(cmd.Parameters);

                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                SqlException sqlException = e as SqlException;
                if (sqlException != null)
                {
                    //customize this as please
                    switch (sqlException.Number)
                    {
                        case 2627:  // Unique constraint error
                        case 547:   // Constraint check violation
                        case 2601:  // Duplicated key row error
                            throw new DuplicateNameException(); 
                        case 50000: //validation exception
                            throw new NotSupportedException(e.Message);
                        case 60001:
                            throw new NotSupportedException(e.Message);
                        default:
                            throw;
                    }
                }
                Console.WriteLine(e);
                throw;
            }
        }

        private void SetupCommand(SqlCommand cmd, SqlQuery query)
        {
            cmd.CommandType = query.CommandType;
            cmd.CommandText = query.Name;
            cmd.CommandTimeout = query.ExecutionTimeout;

            foreach (var param in query.Params)
            {
                cmd.Parameters.Add(param);
            }
        }

        private IDictionary<string, object> GetOutputParametrs(SqlParameterCollection parameters)
        {
            var result = new Dictionary<string, object>();

            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].Direction == ParameterDirection.Output)
                    result.Add(parameters[i].ParameterName, parameters[i].Value);
            }

            return result;
        }

        private int? GetReturnedValue(SqlParameterCollection parameters)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].Direction == ParameterDirection.ReturnValue)
                    return (int)parameters[i].Value;
            }

            return null;
        }


        public class ExecuteProcedureResult<T>
        {
            public int? ReturnedCode { get; set; }
            public T SelectedResult { get; set; }
        }

        #endregion
    }
}
