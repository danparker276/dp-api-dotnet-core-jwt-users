using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace dp.data.AdoNet.SqlExecution
{
    public static class SqlQueryResultParser
    {
        /// <summary>
        /// Converts value to specified type.
        /// </summary>
        /// <typeparam name="T">Type to which value must be converted.</typeparam>
        /// <param name="value">Object which needs to convert.</param>
        /// <param name="nameForErrorMessage">Name which would be used if the conversion failed.</param>
        /// <param name="isResultRequired">Indicates should the Exception be raised if the Value is null.</param>
        public static T GetValue<T>(object value, string nameForErrorMessage, bool isResultRequired = true)
        {
            return ParseData<T>(value, nameForErrorMessage, isResultRequired);
        }

        /// <summary>
        /// Converts value to specified type.
        /// </summary>
        /// <typeparam name="T">Type to which value must be converted.</typeparam>
        /// <param name="dataEntity">IDataReader object with the value which need to be converted.</param>
        /// <param name="valueName">Name of the column in the IDataReader where the value is stored.</param>
        /// <param name="isResultRequired">Indicates should the Exception be raised if the Value is null.</param>
        /// <param name="isColumnRequired">Indicates should the Exception be raised if IDataReader doesn't contain specified column.</param>
        public static T GetValue<T>(IDataReader dataEntity, string valueName, bool isResultRequired = true, bool isColumnRequired = true)
        {
            object value = TryGetValue(dataEntity, valueName, isColumnRequired);

            return GetValue<T>(value, valueName, isResultRequired);
        }

        //Returns a single column and row value
        public static T GetReturnValue<T>(IDataReader dataEntity, string columnName = "No Column Name", bool isResultRequired = true)
        {
            object value = dataEntity[0];

            return GetValue<T>(value, columnName, isResultRequired);
        }

        /// <summary>
        /// Converts value to specified type.
        /// </summary>
        /// <typeparam name="T">Type to which value must be converted.</typeparam>
        /// <param name="dataEntity">Dictionary of objects with the value which need to be converted.</param>
        /// <param name="valueName">Name of the key in the Dictionary where the value is stored.</param>
        /// <param name="isResultRequired">Indicates should the Exception be raised if the value is null.</param>
        /// <param name="isColumnRequired">Indicates should the Exception should be raised if Dictionary
        ///  doesn't contain specified key.</param>
        public static T GetValue<T>(IDictionary<string, object> dataEntity, string valueName, bool isResultRequired = true, bool isColumnRequired = true)
        {
            object value = TryGetValue(dataEntity, valueName, isColumnRequired);

            return GetValue<T>(value, valueName, isResultRequired);
        }

        #region Private Helpers

        private static T ParseData<T>(object value, string valueName, bool isResultRequired)
        {
            if (!IsInputValueCorrect(value, valueName, isResultRequired))
                return default(T);

            Exception innerException = null;
            try
            {
                if (typeof(T) == value.GetType())
                {
                    return (T)value;
                }

                if (typeof(T) == typeof(bool))
                {
                    bool result;
                    if (ParseBoolSafe(value.ToString(), out result))
                        return (T)(object)result;
                }

                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter.CanConvertFrom(value.GetType()))
                {
                    return (T)converter.ConvertFrom(value);
                }

                if (converter.CanConvertFrom(typeof(string)))
                    return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, string.Format(CultureInfo.InvariantCulture, "{0}", value));
            }
            catch (Exception ex)
            {
                innerException = ex;
            }

            throw new Exception($"Sql query result type '{value.GetType()}' can't be converted to type '{typeof(T)}'. Value: {value}",
                innerException);

            //try
            //{
            //    return (T)value;
            //}
            //catch (InvalidCastException)
            //{
            //    try
            //    {
            //        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            //        return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, string.Format(CultureInfo.InvariantCulture, "{0}", value));
            //    }
            //    catch (NotSupportedException ex)
            //    {
            //        throw new Exception(
            //            $"Sql query result type '{value.GetType()}' couldn't be converted to desired type '{typeof(T)}'. Value: {value}",
            //            ex);
            //    }
            //    catch (FormatException)
            //    {
            //        try
            //        {
            //            var tType = typeof(T);
            //            if (tType == typeof(bool))
            //                value = ParseBool(value.ToString());

            //            var converter = TypeDescriptor.GetConverter(typeof(T));
            //            return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, string.Format(CultureInfo.InvariantCulture, "{0}", value));
            //        }
            //        catch (InvalidCastException ex)
            //        {
            //            throw new Exception(
            //                $"Sql query result type '{value.GetType()}' can't be converted to type '{typeof(T)}'. Value: {value}",
            //                ex);
            //        }
            //    }
            //}
        }

        private static bool IsInputValueCorrect(object value, string valueName, bool isResultRequired)
        {
            if (value == null || value is DBNull || value.ToString().Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                switch (isResultRequired)
                {
                    case true:
                        throw new ArgumentNullException(valueName, "Required value can't be null or empty");

                    case false:
                        return false;
                }
            }

            return true;
        }

        private static object TryGetValue(IDictionary<string, object> procedureData, string valueName, bool isColumnRequired)
        {
            if (procedureData.ContainsKey(valueName))
            {
                return procedureData[valueName];
            }

            if (isColumnRequired)
                throw new Exception($"Sql query result doesn't contain column '{valueName}'");

            return null;
        }

        private static object TryGetValue(IDataReader dataReader, string valueName, bool isColumnRequired)
        {
            int columnIndex = GetColumnIndex(dataReader, valueName);
            if (columnIndex >= 0)
            {
                return dataReader[columnIndex];
            }

            if (isColumnRequired)
                throw new Exception($"Sql query result doesn't contain column '{valueName}'");

            return null;
        }

        //private static string ParseBool(string value)
        //{
        //    bool result;

        //    switch (value)
        //    {
        //        case "1":
        //            result = true;
        //            break;
        //        case "0":
        //            result = false;
        //            break;
        //        default:
        //            result = Convert.ToBoolean(value);
        //            break;
        //    }

        //    return result.ToString();
        //}

        private static bool ParseBoolSafe(string value, out bool result)
        {
            switch (value)
            {
                case "1":
                    result = true;
                    return true;
                    
                case "0":
                    result = false;
                    return true;
                    
                default:
                    return bool.TryParse(value, out result);
            }
        }

        public static int GetColumnIndex(IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }
        #endregion
    }
}
