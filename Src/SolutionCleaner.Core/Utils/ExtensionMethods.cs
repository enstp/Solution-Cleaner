using System;
using System.Collections.Generic;
using System.Linq;

namespace SolutionCleaner.Core.Utils
{
    public static class ExtensionMethods
    {
        public static IEnumerable<string> SplitBy(this string str, string separator)
        {
            return str?.Split(";", StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).
                Select(x => x.Trim());
        }
    }
}
