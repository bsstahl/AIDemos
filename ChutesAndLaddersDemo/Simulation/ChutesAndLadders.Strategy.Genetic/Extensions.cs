using ChutesAndLadders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.Genetic
{
    public static class Extensions
    {
        public static int GetRandom(this IEnumerable<int> list)
        {
            var random = new Random();
            var itemNumber = random.Next(list.Count());
            int result = list.Skip(itemNumber).First();
            return result;
        }

        public static int ClosestTo(this IEnumerable<int> list, int target)
        {
            return list.OrderBy(t => Math.Abs(t - target)).First();
        }

    }
}
