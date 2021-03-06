﻿/********************************************************************************
Copyright (C) Binod Nepal, Mix Open Foundation (http://mixof.org).

This file is part of MixERP.

MixERP is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

MixERP is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with MixERP.  If not, see <http://www.gnu.org/licenses/>.
***********************************************************************************/

using MixERP.Net.Common;
using Npgsql;

/********************************************************************************
Copyright (C) Binod Nepal, Mix Open Foundation (http://mixof.org).

This file is part of MixERP.

MixERP is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

MixERP is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with MixERP.  If not, see <http://www.gnu.org/licenses/>.
***********************************************************************************/

using System.Data;
using System.Threading;

namespace MixERP.Net.DBFactory
{
    public static class FormHelper
    {
        public static DataTable GetView(string tableSchema, string tableName, string orderBy, int limit, int offset)
        {
            var sql = "SELECT * FROM @TableSchema.@TableName ORDER BY @OrderBy ASC LIMIT @Limit OFFSET @Offset;";

            using (var command = new NpgsqlCommand())
            {
                //We are 100% sure that the following parameters do not come from user input.
                //Having said that, it is nice to sanitize the objects before sending it to the database server.
                sql = sql.Replace("@TableSchema", Sanitizer.SanitizeIdentifierName(tableSchema));
                sql = sql.Replace("@TableName", Sanitizer.SanitizeIdentifierName(tableName));
                sql = sql.Replace("@OrderBy", Sanitizer.SanitizeIdentifierName(orderBy));
                sql = sql.Replace("@Limit", Conversion.TryCastString(limit));
                sql = sql.Replace("@Offset", Conversion.TryCastString(offset));
                command.CommandText = sql;

                return DbOperation.GetDataTable(command);
            }
        }

        public static DataTable GetTable(string tableSchema, string tableName, string orderBy)
        {
            var sql = "SELECT * FROM @TableSchema.@TableName ORDER BY @OrderBy ASC;";
            using (var command = new NpgsqlCommand())
            {
                sql = sql.Replace("@TableSchema", Sanitizer.SanitizeIdentifierName(tableSchema));
                sql = sql.Replace("@TableName", Sanitizer.SanitizeIdentifierName(tableName));
                sql = sql.Replace("@OrderBy", Sanitizer.SanitizeIdentifierName(orderBy));
                command.CommandText = sql;

                return DbOperation.GetDataTable(command);
            }
        }

        public static DataTable GetTable(string tableSchema, string tableName, string columnNames, string columnValues, string orderBy)
        {
            if (string.IsNullOrWhiteSpace(columnNames))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(columnValues))
            {
                return null;
            }

            var columns = columnNames.Split(',');
            var values = columnValues.Split(',');

            if (!columns.Length.Equals(values.Length))
            {
                return null;
            }

            var counter = 0;
            var sql = "SELECT * FROM @TableSchema.@TableName WHERE ";

            foreach (var column in columns)
            {
                if (!counter.Equals(0))
                {
                    sql += " AND ";
                }

                sql += Sanitizer.SanitizeIdentifierName(column.Trim()) + " = @" + Sanitizer.SanitizeIdentifierName(column.Trim());

                counter++;
            }

            sql += " ORDER BY @OrderBy ASC;";

            using (var command = new NpgsqlCommand())
            {
                sql = sql.Replace("@TableSchema", Sanitizer.SanitizeIdentifierName(tableSchema));
                sql = sql.Replace("@TableName", Sanitizer.SanitizeIdentifierName(tableName));
                sql = sql.Replace("@OrderBy", Sanitizer.SanitizeIdentifierName(orderBy));

                command.CommandText = sql;

                counter = 0;
                foreach (var column in columns)
                {
                    command.Parameters.AddWithValue("@" + Sanitizer.SanitizeIdentifierName(column.Trim()), values[counter]);
                    counter++;
                }

                return DbOperation.GetDataTable(command);
            }
        }

        public static DataTable GetTable(string tableSchema, string tableName, string columnNames, string columnValuesLike, int limit, string orderBy)
        {
            if (columnNames == null)
            {
                columnNames = string.Empty;
            }

            if (columnValuesLike == null)
            {
                columnValuesLike = string.Empty;
            }

            var columns = columnNames.Split(',');
            var values = columnValuesLike.Split(',');

            if (!columns.Length.Equals(values.Length))
            {
                return null;
            }

            var counter = 0;
            var sql = "SELECT * FROM @TableSchema.@TableName ";

            foreach (var column in columns)
            {
                if (!string.IsNullOrWhiteSpace(column))
                {
                    if (counter.Equals(0))
                    {
                        sql += " WHERE ";
                    }
                    else
                    {
                        sql += " AND ";
                    }

                    sql += " lower(" + Sanitizer.SanitizeIdentifierName(column.Trim()) + "::text) LIKE @" + Sanitizer.SanitizeIdentifierName(column.Trim());
                    counter++;
                }
            }

            sql += " ORDER BY @OrderBy ASC LIMIT @Limit;";

            using (var command = new NpgsqlCommand())
            {
                sql = sql.Replace("@TableSchema", Sanitizer.SanitizeIdentifierName(tableSchema));
                sql = sql.Replace("@TableName", Sanitizer.SanitizeIdentifierName(tableName));
                sql = sql.Replace("@OrderBy", Sanitizer.SanitizeIdentifierName(orderBy));

                command.CommandText = sql;

                counter = 0;
                foreach (var column in columns)
                {
                    if (!string.IsNullOrWhiteSpace(column))
                    {
                        command.Parameters.AddWithValue("@" + Sanitizer.SanitizeIdentifierName(column.Trim()), "%" + values[counter].ToLower(Thread.CurrentThread.CurrentCulture) + "%");
                        counter++;
                    }
                }

                command.Parameters.AddWithValue("@Limit", limit);

                return DbOperation.GetDataTable(command);
            }
        }

        public static int GetTotalRecords(string tableSchema, string tableName)
        {
            var sql = "SELECT COUNT(*) FROM @TableSchema.@TableName";
            using (var command = new NpgsqlCommand())
            {
                sql = sql.Replace("@TableSchema", Sanitizer.SanitizeIdentifierName(tableSchema));
                sql = sql.Replace("@TableName", Sanitizer.SanitizeIdentifierName(tableName));

                command.CommandText = sql;

                return Conversion.TryCastInteger(DbOperation.GetScalarValue(command));
            }
        }
    }
}