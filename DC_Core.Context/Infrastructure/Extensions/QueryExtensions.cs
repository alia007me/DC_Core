using DC_Core.Interceptor.Infrastructure.Enums;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace DC_Core.Context.Infrastructure.Extensions
{
    internal static class QueryExtensions
    {
        private static Dictionary<Type, QueryModifierTypes> QueryStatementMatcher = new Dictionary<Type, QueryModifierTypes>()
        {
            {typeof(InsertStatement), QueryModifierTypes.InsertQueryModifier},
            {typeof(UpdateStatement), QueryModifierTypes.UpdateQueryModifier},
            {typeof(DeleteStatement), QueryModifierTypes.DeleteQueryModifier},
            {typeof(SelectStatement), QueryModifierTypes.SelectQueryModifier}
        };


        internal static IEnumerable<TSqlStatement> GetQueryStatements(this string query)
        {
            var parser = new TSql100Parser(true);
            IList<ParseError> errors = new List<ParseError>();

            TSqlScript script = (TSqlScript)parser.Parse(new StringReader(query), out errors);

            if (errors.Any())
                throw new Exception(String.Join(" | ", errors.Select(c => c.Message)));

            foreach (var batch in script.Batches)
                foreach (var statement in batch.Statements)
                    yield return statement;
        }

        internal static bool IsQueryStatementMatchedByModifier<T>(this T queryStatement, QueryModifierTypes queryModifierType)
                 where T : TSqlStatement
        {
            if (queryModifierType is QueryModifierTypes.CommonQueryModifier)
                return true;

            return QueryStatementMatcher[queryStatement.GetType()] == queryModifierType;
        }
    }
}
