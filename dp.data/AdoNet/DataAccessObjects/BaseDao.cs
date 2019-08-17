using dp.data.AdoNet.SqlExecution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace dp.data.AdoNet.DataAccessObjects
{
    public class BaseDao
    {
        protected readonly SqlQueryExecutor _queryExecutor;

        public BaseDao(string dpDbConnectionString)
        {
            _queryExecutor = new SqlQueryExecutor(dpDbConnectionString);// WebConfigManager.Instance.dpDbConnectionString);
        }

        /// <summary>
        /// Returns single column and row value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oReader"></param>
        /// <param name="isResultRequired"></param>
        /// <returns></returns>
        protected T GetReturnValue<T>(IDataReader oReader, bool isResultRequired = true)
        {
            T val = default(T);

            if(oReader.Read())
                val = SqlQueryResultParser.GetReturnValue<T>(oReader, isResultRequired: isResultRequired);

            return val;
        }

        /// <summary>
        /// Builds SQL ORDER BY parameter
        /// </summary>
        /// <param name="orderByFields"></param>
        /// <param name="modelFieldMappings"></param>
        /// <returns></returns>
        protected string BuildOrderByParameter(IEnumerable<OrderFieldAndDirection> orderByFields, Dictionary<string, string> modelFieldMappings)
        {
            StringBuilder sb = new StringBuilder();

            if (orderByFields != null)
            {
                foreach (OrderFieldAndDirection field in orderByFields)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");

                    string direction;
                    switch (field.Direction)
                    {
                        case OrderFieldAndDirection.OrderDirections.Ascending:
                            direction = "asc";

                            break;

                        case OrderFieldAndDirection.OrderDirections.Descending:
                            direction = "desc";

                            break;

                        default:

                            throw new NotImplementedException("Not supported order direction: " + field.Direction);

                    }

                    sb.Append($"{modelFieldMappings[field.FieldName]} {direction}");
                }
            }

            return sb.ToString();
        }

        protected DataTable GetIdCollectionDataTable(IEnumerable<int> ids)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Order", typeof(int));
            dt.Columns.Add("Id", typeof(int));

            if (ids != null)
            {
                foreach (int id in ids)
                {
                    DataRow newRow = dt.NewRow();
                    newRow["Id"] = id;
                    dt.Rows.Add(newRow);
                }
            }

            return dt;
        }

    }
}
