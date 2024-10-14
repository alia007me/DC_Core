using System.ComponentModel;

namespace DC_Core.Context.Infrastructure.Enums
{
    public enum DatabaseTypes
    {
        /// <summary>
        /// SqlServer Database
        /// </summary>
        [Description("SqlServer Database")]
        SqlServer = 0,

        /// <summary>
        /// MySql Database
        /// </summary>
        [Description("MySql Database")]
        MySql = 1
    }
}
