using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Copy<T>(this IEnumerable<T> list)
        {
            return list.Select(t => t).ToArray();
        }

    }
}
