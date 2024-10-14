using System.ComponentModel;

namespace DC_Core.Interceptor.Infrastructure.Enums
{
    public enum QueryModifierTypes
    {
        /// <summary>
        /// Modify All Queries
        /// </summary>
        [Description("Modify All Queries")]
        CommonQueryModifier = 0,

        /// <summary>
        /// Modify Insert Queries
        /// </summary>
        [Description("Modify Insert Queries")]
        InsertQueryModifier = 1,

        /// <summary>
        /// Modify Update Queries
        /// </summary>
        [Description("Modify Update Queries")]
        UpdateQueryModifier = 2,

        /// <summary>
        /// Modify Delete Queries
        /// </summary>
        [Description("Modify Delete Queries")]
        DeleteQueryModifier = 3,

        /// <summary>
        /// Modify Select Queries
        /// </summary>
        [Description("Modify Select Queries")]
        SelectQueryModifier = 4
    }
}
