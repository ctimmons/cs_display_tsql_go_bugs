/* Unless otherwise noted, this source code is licensed
   under the GNU Public License V3.

   See the LICENSE file in the root folder for details. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cs_display_tsql_go_bugs
{
  public static class Extensions
  {
    public static String Repeat(this String value, Int32 count) =>
      (count == 1)
      ? value
      : new StringBuilder(value.Length * count).Insert(0, value, count).ToString();

    public static String Join(this IEnumerable<String> values, String separator) => String.Join(separator, values);

    private static readonly Regex _oneOrMoreDigitsRegex = new(@"\d+");

    /* Code for OrderByNatural<T> is from StackOverflow answer https://stackoverflow.com/a/22323356/116198
       posted by Michael Parker (https://stackoverflow.com/users/1554346/michael-parker).

       Modifications: I moved the regex outside of the method and made it static, and changed some identifier names.

       Licensed under CC BY-SA 3.0 (https://creativecommons.org/licenses/by-sa/3.0/)
       See https://stackoverflow.com/help/licensing for more info. */
    public static IEnumerable<T> OrderByNatural<T>(this IEnumerable<T> items, Func<T, String> selector, StringComparer stringComparer = null!)
    {
      var maxDigits =
        items
        .SelectMany(i => _oneOrMoreDigitsRegex.Matches(selector(i)).Cast<Match>().Select(digitChunk => (Int32?) digitChunk.Value.Length))
        .Max() ?? 0;

      return
        items
        .OrderBy(i => _oneOrMoreDigitsRegex.Replace(selector(i), match => match.Value.PadLeft(maxDigits, '0')), stringComparer ?? StringComparer.CurrentCulture);
    }
  }
}
