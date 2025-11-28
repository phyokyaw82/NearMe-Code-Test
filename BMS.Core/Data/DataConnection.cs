namespace BMS.Core.Data;

public sealed class DataConnection
{
    public string? ConnectionString { get; set; }

    public DbProvider Provider { get; set; }
}

