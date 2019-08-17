using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace dp.data.AdoNet.SqlExecution
{
    public class SqlQuery
    {
        public string Name { get; set; }
        public int ExecutionTimeout { get; set; }
        public CommandType CommandType { get; set; }
        public IList<SqlParameter> Params { get; set; }

        public SqlQuery(string name, int executionTimeout = 30, CommandType commandType = CommandType.StoredProcedure)
        {
            Name = name;
            CommandType = commandType;
            ExecutionTimeout = executionTimeout;
            Params = new List<SqlParameter>();
        }

        public void AddInputTableParam(string name, string tableTypeName, object tableTypeValue)
        {
            Params.Add(new SqlParameter
            {
                ParameterName = name,
                SqlDbType = SqlDbType.Structured,
                TypeName = tableTypeName,
                Value = tableTypeValue
            });
        }

        public void AddInputParam(string name, SqlDbType type, object value, bool isRequied = true,
            int size = 0, string udtTypeName = null)
        {
            var param = new SqlParameter
            {
                ParameterName = name,
                SqlDbType = type,
                Size = size
            };

            if (!string.IsNullOrEmpty(udtTypeName))
                param.UdtTypeName = udtTypeName;

            if (value != null)
                param.Value = value;
            else
            {
                if (isRequied)
                    throw new Exception($"Sql Query {Name} parameter {name} value is required.");

                param.Value = DBNull.Value;
            }

            Params.Add(param);
        }

        public void AddOutputParam(string name, SqlDbType type, bool isRequied = true, int size = 0)
        {
            Params.Add(new SqlParameter
            {
                Direction = ParameterDirection.Output,
                ParameterName = name,
                SqlDbType = type,
                Size = size
            });
        }
    }
}
