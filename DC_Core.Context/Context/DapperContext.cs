using Dapper;
using DC_Core.Context.Infrastructure.Constants;
using DC_Core.Context.Infrastructure.Enums;
using DC_Core.Context.Infrastructure.Extensions;
using DC_Core.Interceptor.Contracts;
using DC_Core.Interceptor.Infrastructure.Enums;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;

namespace DC_Core.Context.Context
{
    /// <summary>
    /// DapperContext is a countaier of EntityAgents that represent a table , view or function's result structure.
    /// With DapperContext you can connect to a database using a connection string that passed in constructor.
    /// With DapperContext you can manage you queries execution as a transaction or not.
    /// </summary>
    public abstract class DapperContext
    {
        /// <summary>
        /// A connection to specified database
        /// </summary>
        protected readonly IDbConnection _connection;
        protected readonly IEnumerable<IQueryModifier>? _queryModifiers;

        /// <summary>
        /// Inject dbConnection as database connection
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        protected DapperContext(IDbConnection dbConnection, IEnumerable<IQueryModifier>? queryModifiers = default)
        {
            this._connection = dbConnection;
            this._queryModifiers = queryModifiers;
        }

        /// <summary>
        /// Set connection string and database type, costructir will create the connection by itself.
        /// </summary>
        /// <param name="connectionString">Connectio string</param>
        /// <param name="databaseType">Database type</param>
        /// <exception cref="Exception">If database type is not valid, costrcutor throws exception. Only SqlServer and MySql are supported</exception>
        protected DapperContext(string connectionString, DatabaseTypes databaseType, IEnumerable<IQueryModifier>? queryModifiers = default)
        {
            switch (databaseType)
            {
                case DatabaseTypes.SqlServer:
                    this._connection = new SqlConnection(connectionString);
                    break;
                case DatabaseTypes.MySql:
                    this._connection = new MySqlConnection(connectionString);
                    break;
                default:
                    throw new Exception(ExceptionMessages.InvalidConnectionType);
            }

            this._queryModifiers = queryModifiers;
        }

        public string DatabaseName => _connection.Database;
        public string ConnectionString => _connection.ConnectionString;

        ///// <summary>
        ///// Execute query with optional parameters and return collection of result type.
        ///// </summary>
        ///// <typeparam name="TResult">The result type</typeparam>
        ///// <param name="query">Query needs to execute</param>
        ///// <param name="parameters">Query parameters</param>
        ///// <param name="cancellationToken">Cancellation token</param>
        ///// <returns>A collection of result type</returns>
        //public virtual async Task<IEnumerable<TResult>> ExecuteAndReadAll<TResult>(string query, object? parameters = default, IQueryModifier? useCustomQueryModifier = default, CancellationToken cancellationToken = default)
        //{
        //    // ToDo: Befor query execution interceptor
        //    query = ModifyQueryIfNeeded(query, useCustomQueryModifier);

        //    var result = await _connection.QueryAsync<TResult>(
        //                        new CommandDefinition(
        //                            commandText: query,
        //                            parameters: parameters,
        //                            cancellationToken: cancellationToken
        //                        ));

        //    // ToDo: After query execution interceptor

        //    return result ?? Enumerable.Empty<TResult>();
        //}

        ///// <summary>
        ///// Execute query with optional parameters and return collection of result type.
        ///// </summary>
        ///// <typeparam name="TResult">The result type</typeparam>
        ///// <param name="query">Query needs to execute</param>
        ///// <param name="parameters">Query parameters</param>
        ///// <param name="cancellationToken">Cancellation token</param>
        ///// <returns>A collection of result type</returns>
        //public virtual async Task<TResult> ExecuteAndReadFirst<TResult>(string query, object? parameters = default, CancellationToken cancellationToken = default)
        //{
        //    // ToDo: Befor query execution interceptor
        //    // ToDo: Query modifier call

        //    var result = await _connection.QueryFirstAsync<TResult>(
        //                        new CommandDefinition(
        //                            commandText: query,
        //                            parameters: parameters,
        //                            cancellationToken: cancellationToken
        //                        ));

        //    // ToDo: After query execution interceptor

        //    return result;
        //}

        /// <summary>
        /// Modify the query if query modfier is set and query type is match
        /// </summary>
        /// <param name="query">Base query</param>
        /// <param name="useCustomQueryModifier">Custom query modifier</param>
        /// <returns>Modified query</returns>
        protected string ModifyQueryIfNeeded(string query, IEnumerable<IQueryModifier>? useCustomQueryModifiers)
        {
            if (useCustomQueryModifiers is not null && useCustomQueryModifiers.Any())
                foreach (var queryModifier in useCustomQueryModifiers)
                    query = ModifyQueryIfMatched(query, queryModifier);

            else if (_queryModifiers is not null && _queryModifiers.Any())
                foreach (var queryModifier in _queryModifiers)
                    query = ModifyQueryIfMatched(query, queryModifier);

            return query;
        }

        /// <summary>
        /// Use modifier if query matched with modifier
        /// </summary>
        /// <param name="query">Base query</param>
        /// <param name="queryModifier">Query modifier</param>
        /// <returns>Modified query</returns>
        private string ModifyQueryIfMatched(string query, IQueryModifier queryModifier)
        {
            var isQueryTypeMatch = IsQueryTypeMatch(query, queryModifier.QueryModifierType);

            if (isQueryTypeMatch)
                return queryModifier.ModifyQuery(query);

            return query;
        }

        /// <summary>
        /// Chech that query has the modifier type related statement
        /// </summary>
        /// <param name="query">Base query</param>
        /// <param name="queryModifierType">Query modifier type</param>
        /// <returns>Matching of query statements with query modifier type</returns>
        /// <exception cref="Exception">If query modifier is invalid, the method throw exception.</exception>
        protected bool IsQueryTypeMatch(string query, QueryModifierTypes queryModifierType)
        {
            var queryStatements = query.GetQueryStatements() ?? Enumerable.Empty<TSqlStatement>();

            switch (queryModifierType)
            {
                // All queries are supported
                case QueryModifierTypes.CommonQueryModifier:
                    return true;

                case QueryModifierTypes.InsertQueryModifier:
                    return queryStatements.Any(c => c.GetType().IsAssignableFrom(typeof(InsertStatement)));

                case QueryModifierTypes.UpdateQueryModifier:
                    return queryStatements.Any(c => c.GetType().IsAssignableFrom(typeof(UpdateStatement)));

                case QueryModifierTypes.DeleteQueryModifier:
                    return queryStatements.Any(c => c.GetType().IsAssignableFrom(typeof(DeleteStatement)));

                case QueryModifierTypes.SelectQueryModifier:
                    return queryStatements.Any(c => c.GetType().IsAssignableFrom(typeof(SelectStatement)));

                default:
                    throw new Exception(ExceptionMessages.InvalidQueryModifierType);
            }
        }
    }
}
