/* Unless otherwise noted, this source code is licensed
   under the GNU Public License V3.

   See the LICENSE file in the root folder for details. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cs_display_tsql_go_bugs
{
  public class Program
  {
    public static void Main(String[] args)
    {
      const String connectionString = "Server=(local);Database=master;Trusted_Connection=True;";

      var shouldShowVerboseOutput = args.Any(arg => arg.Equals("/verbose", StringComparison.CurrentCultureIgnoreCase));
      var (isSqlCmdAvailable, version) = SqlCmd.GetSqlCmdInfo();

      List<String> allTestResults = new();
      List<String> testResult = new();

      if (isSqlCmdAvailable)
        allTestResults.Add($"sqlcmd found on path.  Version '{version ?? "<unknown>"}'.");
      else
        allTestResults.Add("sqlcmd not found on path.");

      /* A post-build event copies the "Test Cases" folder to this executable's folder,
         so the relative path to "Test Cases" used in Path.GetFullPath() works at runtime. */
      foreach (var testCaseFilename in Directory.GetFiles(Path.GetFullPath("Test Cases"), "*.sql", SearchOption.TopDirectoryOnly))
      {
        var testCase = File.ReadAllText(testCaseFilename);

        testResult.Clear();
        testResult.Add(GetTitle(testCaseFilename));
        testResult.Add(testCase);

        var errors = SMO.Run(connectionString, testCase);
        testResult.Add(GetTestCompletionStatus("SMO", errors, shouldShowVerboseOutput));

        errors = ScriptDom.Run(testCase);
        testResult.Add(GetTestCompletionStatus("ScriptDom", errors, shouldShowVerboseOutput));

        if (isSqlCmdAvailable)
        {
          errors = SqlCmd.Run(testCaseFilename);
          testResult.Add(GetTestCompletionStatus("SqlCmd", errors, shouldShowVerboseOutput));
        }

        allTestResults.Add(testResult.Join("\n"));
      }

      Console.WriteLine(allTestResults.Join($"\n{"-".Repeat(50)}\n"));
    }

    private static String GetTitle(String testCaseFilename)
    {
      List<String> result = new();

      var filename = Path.GetFileNameWithoutExtension(testCaseFilename);
      var title = $"=  {filename}  =";
      var border = "=".Repeat(title.Length);

      result.Add(border);
      result.Add(title);
      result.Add(border);

      return result.Join("\n");
    }

    private static String GetTestCompletionStatus(String title, String errors, Boolean shouldShowVerboseOutput) =>
      errors.Any()
      ? $"{title}: FAIL!{(shouldShowVerboseOutput ? $"\n{errors}" : "")}"
      : $"{title}: Pass";
  }
}
