using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CloudSpeed.Settings
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DbServerType
    {
        /// <summary>
        /// SQL Server
        /// </summary>
        [Description("SQL Server")]
        SQLServer = 1,

        /// <summary>
        /// MySQL
        /// </summary>
        [Description("MySQL")]
        MySQL = 2,

        /// <summary>
        /// Sqlite
        /// </summary>
        [Description("Sqlite")]
        Sqlite = 3,

        /// <summary>
        /// InMemory Database for test
        /// </summary>
        [Description("InMemory")]
        InMemory = 99
    }
}
