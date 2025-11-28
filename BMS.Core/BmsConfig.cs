using BMS.Core.Data;

namespace BMS.Core;

public class BmsConfig
{
    //<summary>
    //Get full connection string information eg.Data Source = localhost\SQL;Initial Catalog = MyDb; User ID = sa; Password=********
    //</summary>
    //<remarks>
    //The change will never be effected after one time cofiguration made.
    //</remarks>
    Dictionary<string, DataConnection> DbConnection { get; }
}

