using DC_Core.Interceptor.Infrastructure.Enums;

namespace DC_Core.Interceptor.Contracts
{
    public interface IQueryModifier
    {
        /// <summary>
        /// Modifier Type, modifier availability for type of queries
        /// </summary>
        QueryModifierTypes QueryModifierType { get; }

        /// <summary>
        /// The action that modifies input query
        /// </summary>
        /// <param name="query">Base query</param>
        /// <returns>Modified query</returns>
        string ModifyQuery(string query);
    }
}
