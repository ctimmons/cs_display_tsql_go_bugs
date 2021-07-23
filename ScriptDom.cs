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
    public static String Run(String testCase) =>
      GetTSqlParsers("Microsoft.SqlServer.TransactSql.ScriptDom.dll")
      .Select(parser => Parse(parser, testCase))
      .Where(s => !String.IsNullOrWhiteSpace(s))
      .OrderByNatural(s => s)
      .Join("\n");

    /* ScriptDom provides several T-SQL parsers.
       Get instances of each one to run the test cases thru. */
    private static IEnumerable<TSqlParser> GetTSqlParsers(String assemblyName) =>
      Assembly
      .Load(System.Reflection.AssemblyName.GetAssemblyName(assemblyName).FullName)
      .ExportedTypes
      .Where(t => t.IsSubclassOf(typeof(TSqlParser)))
      .Select(t => (TSqlParser) Activator.CreateInstance(t, new Object[] { false }));

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
