/* Unless otherwise noted, this source code is licensed
   under the GNU Public License V3.

   See the LICENSE file in the root folder for details. */

using System;
using System.Collections;
using System.Text;

namespace cs_display_tsql_go_bugs
{
  public class Exceptions
  {
    public static String GetAllExceptionMessages(Exception ex)
    {
      StringBuilder result = new();

      void rec(Exception currentException)
      {
        if (currentException == null)
          return;

        result.AppendLine(currentException.Message);

        foreach (DictionaryEntry de in currentException.Data)
          result.AppendLine($"  {de.Key}: {de.Value}");

        rec(currentException.InnerException);
      }

      rec(ex);

      return result.ToString();
    }
  }
}
