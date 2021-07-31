/* Unless otherwise noted, this source code is licensed
   under the GNU Public License V3.

   See the LICENSE file in the root folder for details. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace cs_display_tsql_go_bugs
{
  public class ScriptDom
  {
    /* ScriptDom provides several T-SQL parsers.
       Get instances of each one to run the test cases thru. */
    private static readonly IEnumerable<TSqlParser> _allTSqlParsers =
      Assembly
      .Load(System.Reflection.AssemblyName.GetAssemblyName("Microsoft.SqlServer.TransactSql.ScriptDom.dll").FullName)
      .ExportedTypes
      .Where(type => type.IsSubclassOf(typeof(TSqlParser)))
      .Select(type => (TSqlParser) Activator.CreateInstance(type, new Object[] { false }));

    public static String Run(String testCase) =>
      _allTSqlParsers
      .Select(parser => Parse(parser, testCase))
      .Where(s => !String.IsNullOrWhiteSpace(s))
      .OrderByNatural(s => s)
      .Join("\n");

    private static String Parse(TSqlParser parser, String testCase)
    {
      var parsername = parser.GetType().Name;

      try
      {
        using (StringReader sr = new(testCase))
        {
          TSqlFragment fragment = parser.Parse(sr, out IList<ParseError> errors);

          return
            errors.Any()
            ? $"{parsername}: {errors.Select(error => $"{error.Number}: {error.Message}").Join("\n")}"
            : "";
        }
      }
      catch (Exception ex)
      {
        return $"{parsername}: {Exceptions.GetAllExceptionMessages(ex)}";
      }
    }
  }
}
