using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.ShortestPath
{
    public static class Extensions
    {
        internal static IEnumerable<Gamespace> PathOrigins(this IEnumerable<Gamespace> list)
        {
            return list.Where(s => s.PathTo.HasValue);
        }

        public static KeyValuePair<int, int> AddPair(this IList<KeyValuePair<int, int>> list, int start, int end)
        {
            var newPair = new KeyValuePair<int, int>(start, end);
            list.Add(newPair);
            return newPair;
        }

        internal static void AppendGamespaceDistance(this StringBuilder sb, Gamespace s)
        {
            if (s.DistanceFromEnd.HasValue)
                sb.Append(s.DistanceFromEnd.Value.ToString("00 "));
            else
                sb.Append("?? ");
        }

    }
}
