using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace SqlTableToClassGenerator;

public static class SqlTableGenerator
{
    public static async Task GenerateTableClasses(string connectionString, string targetPath, string desiredNamespace,
        bool enableForDotNetStandard2, string? classPrefix, string? classPostfix, string[] ignoredSchemas)
    {
        using IDbConnection connection = new SqlConnection(connectionString);

        targetPath = Path.Combine(targetPath, "Tables");
        
        if(Directory.Exists(targetPath))
        {
            Directory.Delete(targetPath, true);
        }
        
        Directory.CreateDirectory(targetPath);
        
        var storedProcedureDefinitions =
            await connection.QueryAsync<TableDefinition>(StaticStrings.Query);
        
        // Group by schema, ignore the `temp` schema.
        var schemaRecordsGroup = storedProcedureDefinitions
            .GroupBy(g => g.TABLE_SCHEMA)
            .Where(w => !ignoredSchemas.Contains(w.Key, StringComparer.OrdinalIgnoreCase));

        foreach (var schemaTableRecord in schemaRecordsGroup)
        {
            var tableGroups = schemaTableRecord.GroupBy(g => g.TABLE_NAME, k => k);

            foreach (var tableGroup in tableGroups)
            {
                BuildFiles(schema: schemaTableRecord.Key, 
                    tableColumns: tableGroup, 
                    targetPath: targetPath, 
                    classPrefix: classPrefix, 
                    classPostfix: classPostfix, 
                    desiredNamespace: desiredNamespace,
                    enableForDotNetStandard2: enableForDotNetStandard2);
            }
        }
    }

    private static void BuildFiles(string schema, IGrouping<string, TableDefinition> tableColumns,
        string targetPath, string? classPrefix, string? classPostfix, string desiredNamespace, bool enableForDotNetStandard2)
    {
        var table = tableColumns.Key;
        var className = classPrefix + CleanName(char.ToUpper(table[0]) + table.Substring(1)) + classPostfix;
        var schemaProper = char.ToUpper(schema[0]) + schema.Substring(1);

        var primaryClass = new StringBuilder();
        primaryClass.AppendLine($@"/*
 *         _   _   _ _____ ___     ____ _____ _   _ _____ ____      _  _____ _____ ____  
 *        / \ | | | |_   _/ _ \   / ___| ____| \ | | ____|  _ \    / \|_   _| ____|  _ \ 
 *       / _ \| | | | | || | | | | |  _|  _| |  \| |  _| | |_) |  / _ \ | | |  _| | | | |
 *      / ___ \ |_| | | || |_| | | |_| | |___| |\  | |___|  _ <  / ___ \| | | |___| |_| |
 *     /_/   \_\___/  |_| \___/   \____|_____|_| \_|_____|_| \_\/_/   \_\_| |_____|____/ 
 *    This file has been automatically generated. Any modification will get overwritten.
 * If you need to create a similar model, please inherit from this class as this matches the table.
 */

using System;

namespace {desiredNamespace}.Tables.{schemaProper}
{{
    public class {className}
    {{");
        foreach (var tableColumn in tableColumns)
        {
            var dataType = tableColumn.DATA_TYPE;
            primaryClass.Append($@"
        public {GetCSharpType(dataType)}{(tableColumn.IS_NULLABLE == "YES" && IsNullable(enableForDotNetStandard2, dataType) ? "?" : string.Empty)} {CleanName(tableColumn.COLUMN_NAME)} {{ get; set; }}");
        }

        primaryClass.AppendLine(@"
    }
}");

        if (!Directory.Exists(Path.Combine(targetPath, schemaProper)))
        {
            Directory.CreateDirectory(Path.Combine(targetPath, schemaProper));
        }

        var schemaFilepath = Path.Combine(schemaProper, $"{className}.cs");

        if (File.Exists(Path.Combine(targetPath, schemaFilepath)))
        {
            File.Delete(Path.Combine(targetPath, schemaFilepath));
        }

        File.WriteAllText(Path.Combine(targetPath, schemaFilepath), primaryClass.ToString(), Encoding.UTF8);
    }

    private static string CleanName(string field)
    {
        field = field.Replace(' ', '_');

        var cleanName = field.RemoveNonAlphaNumericUnderscore();

        if (char.IsDigit(cleanName[0]))
        {
            return "_" + cleanName;
        }

        return cleanName;
    }

    private static string GetCSharpType(string datatype)
    {
        switch (datatype)
        {
            case "int":
            case "smallint":
            case "tinyint":
                return "int";
            case "bigint":
                return "long";
            case "bit":
                return "bool";
            case "float":
                return "double";
            case "decimal":
            case "money":
            case "numeric":
            case "real":
            case "smallmoney":
                return "decimal";
            case "date":
            case "datetime":
            case "datetime2":
            case "smalldatetime":
                return "DateTime";
            case "datetimeoffset":
                return "DateTimeOffset";
            case "time":
                return "TimeSpan";
            case "char":
            case "nchar":
            case "ntext":
            case "nvarchar":
            case "text":
            case "varchar":
                return "string";
            case "binary":
            case "image":
            case "varbinary":
            case "timestamp":
                return "byte[]";
            case "uniqueidentifier":
                return "Guid";
            default:
                throw new Exception($"Unknown data type: {datatype}");
        }
    }

    private static string[] NullableList =
    {
        "int", "bigint", "smallint", "tinyint", "bit", "float", "decimal",
        "money", "numeric", "real", "smallmoney", "date", "datetime",
        "datetime2", "datetimeoffset", "smalldatetime", "time",
        "uniqueidentifier"
    };

    private static bool IsNullable(bool enableForDotNetStandard2, string datatype)
    {
        return !enableForDotNetStandard2 || NullableList.Contains(datatype, StringComparer.OrdinalIgnoreCase);
    }
}