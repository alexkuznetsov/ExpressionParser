# Linq Expression parser

Parses the Linq-expression tree and translates it into a string representation according to mapping.

The main goal of this project is to create simple and lightweight criterion-like filters 
filters in LINQ, translate them into SQL and add them to a SQL query in DbCommand. 

## Minimal  example

The `ExpressionParser.WebApiExample` project provides an incompletely functional use case.
See the stuff in the `Modules/Cities` folder.

There are no migrations for the database, and there are probably typos in the SQL.