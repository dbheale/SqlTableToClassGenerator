namespace SqlTableToClassGenerator
{
    internal static class StaticStrings
	{

        public static string ConnectionStringMessage = "Connection String of database: ";
        public static string TargetPathMessage = "Target path for output files: ";
        public static string NamespaceMessage = "Root namespace for output files: ";
        public static string DotNetStandardMessage = "Backwards compatibility for .NET Standard 2.0? ";
        public static string FinishMessage = "Commands generated in {0}ms.";


		// Query to get all stored procedures in every schema.
		public static string Query = @"
    SELECT 
        c.TABLE_SCHEMA, 
        c.TABLE_NAME, 
        c.COLUMN_NAME, 
        c.DATA_TYPE, 
        c.CHARACTER_MAXIMUM_LENGTH, 
        c.IS_NULLABLE
    FROM 
        INFORMATION_SCHEMA.COLUMNS c
        INNER JOIN INFORMATION_SCHEMA.TABLES t
        ON c.TABLE_SCHEMA = t.TABLE_SCHEMA
        AND c.TABLE_NAME = t.TABLE_NAME
    WHERE 
        t.TABLE_TYPE = 'BASE TABLE'
    ORDER BY 
        c.TABLE_SCHEMA, 
        c.TABLE_NAME, 
        c.ORDINAL_POSITION";

        public static string IDatabaseCommand = @"using Dapper;
using System.Data;

namespace |^NAMESPACE^|
{
    /// <summary>
    /// This class is automatically generated.
    /// </summary>
    public interface IDatabaseCommand
    {
        DynamicParameters GetParameters();
        CommandType GetCommandType();
        string GetSqlStatement();
        bool HasOutParameters();
        void SetOutParameters(DynamicParameters parameters);
    }
}";

        
        public static string DapperCommandExtensions = @"
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace |^NAMESPACE^|
{
    /// <summary>
    /// This class is automatically generated, to extend this class, please use a partial class.
    /// </summary>
    public static partial class DapperCommandExtensions
    {
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection dbConnection, IDatabaseCommand command, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var parameters = command.GetParameters();

            var results = await dbConnection.QueryAsync<T>(
                command.GetSqlStatement()
                , parameters
                , transaction
                , commandTimeout
                , command.GetCommandType());

            if (command.HasOutParameters())
            {
                command.SetOutParameters(parameters);
            }

            return results;
        }

        public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection dbConnection, IDatabaseCommand command, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var parameters = command.GetParameters();

            var results = await dbConnection.QueryAsync(
                command.GetSqlStatement()
                , parameters
                , transaction
                , commandTimeout
                , command.GetCommandType());

            if (command.HasOutParameters())
            {
                command.SetOutParameters(parameters);
            }

            return results;
        }

        public static async Task<T> QueryFirstAsync<T>(this IDbConnection dbConnection, IDatabaseCommand command, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var parameters = command.GetParameters();

            var results = await dbConnection.QueryFirstAsync<T>(
                command.GetSqlStatement()
                , parameters
                , transaction
                , commandTimeout
                , command.GetCommandType());

            if (command.HasOutParameters())
            {
                command.SetOutParameters(parameters);
            }

            return results;
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(this IDbConnection dbConnection, IDatabaseCommand command, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var parameters = command.GetParameters();

            var results = await dbConnection.QueryFirstOrDefaultAsync<T>(
                command.GetSqlStatement()
                , parameters
                , transaction
                , commandTimeout
                , command.GetCommandType());

            if (command.HasOutParameters())
            {
                command.SetOutParameters(parameters);
            }

            return results;
        }

        public static async Task<T> QuerySingleAsync<T>(this IDbConnection dbConnection, IDatabaseCommand command, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var parameters = command.GetParameters();

            var results = await dbConnection.QuerySingleAsync<T>(
                command.GetSqlStatement()
                , parameters
                , transaction
                , commandTimeout
                , command.GetCommandType());

            if (command.HasOutParameters())
            {
                command.SetOutParameters(parameters);
            }

            return results;
        }

        public static async Task<T> QuerySingleOrDefaultAsync<T>(this IDbConnection dbConnection, IDatabaseCommand command, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var parameters = command.GetParameters();

            var results = await dbConnection.QuerySingleOrDefaultAsync<T>(
                command.GetSqlStatement()
                , parameters
                , transaction
                , commandTimeout
                , command.GetCommandType());

            if (command.HasOutParameters())
            {
                command.SetOutParameters(parameters);
            }

            return results;
        }

        public static async Task<int> ExecuteAsync(this IDbConnection dbConnection, IDatabaseCommand command, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var parameters = command.GetParameters();

            var results = await dbConnection.ExecuteAsync(
                command.GetSqlStatement()
                , parameters
                , transaction
                , commandTimeout
                , command.GetCommandType());

            if (command.HasOutParameters())
            {
                command.SetOutParameters(parameters);
            }

            return results;
        }

        public static async Task<T> ExecuteScalarAsync<T>(this IDbConnection dbConnection, IDatabaseCommand command, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var parameters = command.GetParameters();

            var results = await dbConnection.ExecuteScalarAsync<T>(
                command.GetSqlStatement()
                , parameters
                , transaction
                , commandTimeout
                , command.GetCommandType());

            if (command.HasOutParameters())
            {
                command.SetOutParameters(parameters);
            }

            return results;
        }

        public static string FormatForSql<T>(this T value) where T : class
        {
            switch (value)
            {
                case null:
                    return ""NULL"";
                case string s:
                    return $""'{s.Replace(""'"", ""''"")}'""; // Handle single quotes in strings
                case char c:
                    return $""'{c.ToString().Replace(""'"", ""''"")}'"";
                case DateTime dt:
                    return $""'{dt:yyyy-MM-ddTHH:mm:ss}'"";
                case DateTimeOffset dto:
                    return $""'{dto:yyyy-MM-ddTHH:mm:ss}'"";
                case bool b:
                    // Convert boolean to 1 or 0; direct cast is safe
                    return b ? ""1"" : ""0"";
                default:
                    return value.ToString(); // Fallback for other reference types
            }
        }

        public static string FormatForSql<T>(this T? value) where T : struct
        {
            switch (value)
            {
                case null:
                    return ""NULL"";
                case char c:
                    return $""'{c.ToString().Replace(""'"", ""''"")}'""; // Handle single quotes in strings
                case DateTime dt:
                    return $""'{dt:yyyy-MM-ddTHH:mm:ss}'"";
                case DateTimeOffset dto:
                    return $""'{dto:yyyy-MM-ddTHH:mm:ss}'"";
                case bool b:
                    return b ? ""1"" : ""0"";
            }

            if (typeof(T).IsEnum)
            {
                return $""'{value.ToString()}'"";
            }

            return value.ToString(); // Fallback for other value types
        }

    }
}";
    }
}
