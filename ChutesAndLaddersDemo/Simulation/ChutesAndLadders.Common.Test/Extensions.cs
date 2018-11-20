using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Common.Test
{
    public static class Extensions
    {
        public static bool IsEqualTo(this IEnumerable<int> expected, IEnumerable<int> actual)
        {
            bool result = (expected.Count() == actual.Count());
            var orderedExpected = expected.OrderBy(j => j).ToArray();
            var orderedActual = actual.OrderBy(j => j).ToArray();
            for (int i = 0; i < expected.Count(); i++)
                if (orderedExpected[i] != orderedActual[i])
                    result = false;
            return result;
        }

    }
}
