using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.ShortestPath
{
    internal class Gamespace
    {
        public int Index { get; set; }

        public int? PathTo { get; set; }

        public int? DistanceFromEnd { get; set; }
    }
}
