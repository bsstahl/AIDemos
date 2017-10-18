using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.Random
{
    public static class Extensions
    {
        public static int GetRandom(this IEnumerable<int> moves)
        {
            int result;

            if (moves == null)
                throw new ArgumentNullException(nameof(moves));

            if (moves.Count() < 1)
                throw new ArgumentException($"{nameof(moves)} must have at least 1 element");

            if (moves.Count() == 1)
                result = moves.First();
            else
            {
                var rnd = new System.Random();
                int index = rnd.Next(moves.Count());
                result = moves.ToArray()[index];
            }

            return result;
        }
    }
}
