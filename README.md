# DC_Core

An abstraction on dapper to handle these features:

1. Transactions: With DapperContext you can easily handle and manage solo and multi transactions
2. QueryModifier: With DapperContext you have access to a feature called QueryModifier, that allows you to add a modification on whole query statemens with a specific type(also for all type of query statements)
3. Interceptor: With DapperContext you have access to a feature called Interceptor, that allows you to manage something before of after executing some queries. Also Interceptor gives you query parameters and result as well.

# Supported Databases
By now supported Databases are:
1. SQL Server(Microsoft): You can access your SQL Server database by using string connection or DbConnection object.
2. MySQL: You can access your SQL Server database by using string connection or DbConnection object.
