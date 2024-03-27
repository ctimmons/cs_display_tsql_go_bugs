### cs_display_tsql_go_bugs

##### A C# app that displays bugs related to SQL Server's GO statement.

0. Overview

   Sometimes a .Net app needs to execute a script that contains multiple batches of T-SQL statements, where the batches are separated from one another by `GO` statements.

   The NuGet package `Microsoft.Data.SqlClient` (or `System.Data.SqlClient` included with .Net Framework) provides basic ADO.Net functionality via classes like `SqlConnection`, `SqlCommand`, etc.  However, none of those classes understand the `GO` statement.

   Fortunately, Microsoft offers at least three ways to programmatically execute or parse a multi-batch T-SQL script:

   - The `TSqlParser` class, found in the `Microsoft.SqlServer.DacFx` NuGet package (parse only)
   - SQL Server Management Objects (`SMO`), found in the `Microsoft.SqlServer.SqlManagementObject` NuGet package
   - And, if it's installed, the `sqlcmd.exe` command line utility

   Unfortunately, they all have bugs related to parsing the `GO` statement, especially with regard to comments.  (`Test Case 09` is illuminating in this regard, where the `GO` statement is entirely contained within a block comment, yet `sqlcmd` tries to parse the `GO` statement anyways.)

   I couldn't keep track of all the `GO`-related bugs present in these three systems, so I wrote this program to do it for me.  This app simply runs a series of T-SQL test cases thru `TSqlParser` and `SMO` (always), and `sqlcmd.exe` (if it's installed).

   The number of ways a `GO` statement can fail is both surprising and a bit depressing.  All three technologies fail in different test cases.  `TSqlParser` is the best of the lot, only failing to parse the most fundamental `GO` statement where a `count` parameter is given (see `Test Case 11`).

   If you want to examine this program's output without having to install and run it, see the `example_output.txt` file.

1. Building

   This is a .Net 8.0 app.  It has two NuGet dependencies:

   - `Microsoft.SqlServer.DacFx` (containing the `TSqlParser` class)
   - `Microsoft.SqlServer.SqlManagementObject` (aka `SMO`)

   All that one should need to do is load the project in Visual Studio and build the solution.

2. Usage

   This app is a command line program and takes one command line parameter `/verbose`.  When run with this parameter, any error messages will be included in the generated output.  The `example_output.txt` file was generated using the `/verbose` flag.

   If `sqlcmd.exe` is on the computer's path, the program will execute all of the tests with it, in addition to `SMO` and `TSqlParser`.

   All of the generated output is sent to stdout.

   The connection string used by the `SMO`-related code is hardcoded in `Program.cs`.  The provided connection string uses a Windows logon to connect to the master database on the default SQL Server instance on the local machine.  Change this and re-compile if you want to connect to a different instance.

