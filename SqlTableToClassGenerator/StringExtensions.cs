using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableToClassGenerator
{
    internal static class StringExtensions
    {
        public static bool HasContent(this string? val)
        {
            return !string.IsNullOrEmpty(val?.Trim());
        }

        public static string? ToUpperFirstCharacter(this string? str)
        {
            if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
            {
                return str.Length == 1
                    ? char.ToUpper(str[0]).ToString()
                    : char.ToUpper(str[0]) + str[1..];
            }
            return str;
        }

        public static string? ToLowerFirstCharacter(this string? str)
        {
            if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
            {
                return str.Length == 1
                    ? char.ToLower(str[0]).ToString()
                    : char.ToLower(str[0]) + str[1..];
            }
            return str;
        }

        public static string GetSqlType(this string? typeName)
        {
            return typeName switch
            {
                "image" => "SqlDbType.Image",
                "text" => "SqlDbType.Text",
                "uniqueidentifier" => "SqlDbType.UniqueIdentifier",
                "date" => "SqlDbType.Date",
                "time" => "SqlDbType.Time",
                "datetime2" => "SqlDbType.DateTime2",
                "datetimeoffset" => "SqlDbType.DateTimeOffset",
                "tinyint" => "SqlDbType.TinyInt",
                "smallint" => "SqlDbType.SmallInt",
                "int" => "SqlDbType.Int",
                "smalldatetime" => "SqlDbType.SmallDateTime",
                "real" => "SqlDbType.Real",
                "money" => "SqlDbType.Money",
                "datetime" => "SqlDbType.DateTime",
                "float" => "SqlDbType.Float",
                "sql_variant" => "SqlDbType.Variant",
                "ntext" => "SqlDbType.NText",
                "bit" => "SqlDbType.Bit",
                "decimal" => "SqlDbType.Decimal",
                "smallmoney" => "SqlDbType.SmallMoney",
                "bigint" => "SqlDbType.BigInt",
                "varbinary" => "SqlDbType.VarBinary",
                "varchar" => "SqlDbType.VarChar",
                "binary" => "SqlDbType.Binary",
                "char" => "SqlDbType.Char",
                "timestamp" => "SqlDbType.Timestamp",
                "nvarchar" => "SqlDbType.NVarChar",
                "nchar" => "SqlDbType.NChar",
                "xml" => "SqlDbType.Xml",
                _ => "SqlDbType.Udt",
            };
        }

        public static string GetDbType(this string? typeName)
        {
            return typeName switch
            {
                "uniqueidentifier" => "DbType.Guid",
                "date" => "DbType.Date",
                "time" => "DbType.Time",
                "smalldatetime" => "DbType.DateTime",
                "datetime2" => "DbType.DateTime2",
                "datetimeoffset" => "DbType.DateTimeOffset",
                "tinyint" => "DbType.UInt16",
                "smallint" => "DbType.Int16",
                "int" => "DbType.Int32",
                "bigint" => "DbType.Int64",
                "money" or "smallmoney" => "DbType.Currency",
                "datetime" => "DbType.DateTime",
                "float" => "DbType.Double",
                "bit" => "DbType.Boolean",
                "decimal" => "DbType.Decimal",
                "varbinary" => "DbType.Binary",
                "varchar" or "ntext" or "text" or "char" or "nvarchar" or "nchar" => "DbType.String",
                "binary" => "DbType.Binary",
                "xml" => "DbType.Xml",
                _ => "DbType.Object",
            };
        }

        public static string GetCSharpType(this string? typeName, bool nullability = true)
        {
            return typeName switch
            {
                "date" or "datetime" or "smalldatetime" or "datetime2" => "DateTime?",
                "time" => "TimeSpan?",
                "datetimeoffset" => "DateTimeOffset?",
                "tinyint" => "byte?",
                "smallint" => "short?",
                "int" => "int?",
                "bigint" => "long?",
                "real" => "float?",
                "float" => "double?",
                "bit" => "bool?",
                "decimal" or "numeric" or "money" or "smallmoney" => "decimal?",
                "image" or "binary" or "varbinary" or "timestamp" => nullability ? "byte[]?" : "byte[]",
                "text" or "ntext" or "char" or "nchar" or "varchar" or "nvarchar" => nullability ? "string?" : "string",
                _ => nullability ? "object?" : "object",
            };
        }
        
        public static string FormatValueForSql(this string value, string csharpTypeName)
        {
            switch (csharpTypeName)
            {
                case "string":
                case "string?":
                case "DateTime?":
                case "DateTimeOffset?":
                case "TimeSpan?":
                case "bool":
                case "bool?":
                    return $"{{{value}.FormatForSql()}}"; // escape anything needed for SQL text
                default:
                    // Fallback for types not explicitly handled
                    return $"{{{value}}}";
            }
        }

        /// <summary>
        /// If conversion fails, the default value is returned.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool? ToNullableBool(this string? source, bool? defaultValue = default)
        {
            if (string.IsNullOrEmpty(source))
            {
                return defaultValue;
            }

            switch (source)
            {
                case "1":
                case { } a when a.StartsWith("t", StringComparison.OrdinalIgnoreCase): // true/t/etc.
                case { } b when b.StartsWith("y", StringComparison.OrdinalIgnoreCase): // yes/y/etc.
                    return true;
                case "0":
                case { } a when a.StartsWith("f", StringComparison.OrdinalIgnoreCase): // false/f/fal/etc.
                case { } b when b.StartsWith("n", StringComparison.OrdinalIgnoreCase): // no/n/niet/etc.
                    return false;
            }

            return defaultValue;
        }

        /// <summary>
        /// If conversion fails, the default value is returned.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ToBool(this string? source, bool defaultValue = default)
        {
            if (string.IsNullOrEmpty(source))
            {
                return defaultValue;
            }

            switch (source)
            {
                case "1":
                case { } a when a.StartsWith("t", StringComparison.OrdinalIgnoreCase): // true/t/etc.
                case { } b when b.StartsWith("y", StringComparison.OrdinalIgnoreCase): // yes/y/etc.
                    return true;
                case "0":
                case { } a when a.StartsWith("f", StringComparison.OrdinalIgnoreCase): // false/f/fal/etc.
                case { } b when b.StartsWith("n", StringComparison.OrdinalIgnoreCase): // no/n/niet/etc.
                    return false;
            }

            return defaultValue;
        }
        
        public static string RemoveNonAlphaNumericUnderscore(this string source)
        {
            source = source.Trim();

            if (!source.HasContent())
            {
                return source;
            }

            var sb = new StringBuilder();
            for (var i = 0; i < source.Length; i++)
            {
                var t = source[i];

                if (char.IsLetterOrDigit(t)
                    || t == '_')
                {
                    sb.Append(t);
                }
            }
            return sb.ToString();
        }
    }
}
