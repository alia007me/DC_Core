using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace DC_Core.Context.Infrastructure.Extensions
{
    internal static class QueryExtensions
    {
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
    }
}
