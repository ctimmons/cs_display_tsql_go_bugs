/* Unless otherwise noted, this source code is licensed
   under the GNU Public License V3.

   See the LICENSE file in the root folder for details. */

using System;

using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace cs_display_tsql_go_bugs
{
  public class SMO
  {
    public static String Run(String connectionString, String testCase)
    {
      using (var sqlConnection = new SqlConnection(connectionString))
      {
        var serverConnection = new ServerConnection(sqlConnection);
        var server = new Server(serverConnection);

        try
        {
          server.ConnectionContext.ExecuteNonQuery(testCase);
          return "";
        }
        catch (Exception ex)
        {
          return Exceptions.GetAllExceptionMessages(ex);
        }
      }
    }
  }
}
