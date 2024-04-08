namespace SqlTableToClassGenerator;

public class TableDefinition
{
    public string TABLE_SCHEMA { get; set; }
    public string TABLE_NAME { get; set; }
    public string COLUMN_NAME { get; set; }
    public string DATA_TYPE { get; set; }
    public int CHARACTER_MAXIMUM_LENGTH { get; set; }
    public string IS_NULLABLE { get; set; }
}