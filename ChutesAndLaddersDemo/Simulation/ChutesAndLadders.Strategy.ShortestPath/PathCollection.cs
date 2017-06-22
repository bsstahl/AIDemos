using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.ShortestPath
{
    internal class PathCollection
    {
        Gameboard _board;

        public PathCollection()
        {
            _board = CreateGameboard();
            DetermineDistance(100, 0);
        }

        public int FindClosestToEnd(IEnumerable<int> spaces)
        {
            // Returns the space that has the shortest distance
            // to the end of the board
            var dict = new Dictionary<int, int>();
            foreach (var space in spaces)
                dict.Add(space, _board[space].DistanceFromEnd.Value);
            var minValue = dict.Min(s => s.Value);
            return dict.First(s => s.Value == minValue).Key;
        }

        public int? this [int index]
        {
            get
            {
                return _board[index].DistanceFromEnd;
            }
        }

        public override string ToString()
        {
            return _board.ToString();
        }

        private Gameboard CreateGameboard()
        {
            var pathways = new List<KeyValuePair<int, int>>();
            pathways.AddPair(1, 38);
            pathways.AddPair(4, 14);
            pathways.AddPair(9, 31);
            pathways.AddPair(16, 6);
            pathways.AddPair(21, 42);
            pathways.AddPair(28, 84);
            pathways.AddPair(36, 44);
            pathways.AddPair(47, 26);
            pathways.AddPair(49, 11);
            pathways.AddPair(51, 67);
            pathways.AddPair(56, 53);
            pathways.AddPair(62, 19);
            pathways.AddPair(64, 60);
            pathways.AddPair(71, 91);
            pathways.AddPair(80, 100);
            pathways.AddPair(87, 24);
            pathways.AddPair(93, 73);
            pathways.AddPair(95, 75);
            pathways.AddPair(98, 78);

            return new Gameboard(100, pathways);
        }

        private void DetermineDistance(int spaceIndex, int distanceFromEnd)
        {
            var space = _board[spaceIndex];
            if (!space.DistanceFromEnd.HasValue ||
                space.DistanceFromEnd.Value > distanceFromEnd)
            {
                space.DistanceFromEnd = distanceFromEnd;
                var pathFromSpace = _board.PathFrom(space.Index);
                if (pathFromSpace != null)
                    DetermineDistance(pathFromSpace.Index, distanceFromEnd + 1);
                if (space.Index > 1)
                    DetermineDistance(space.Index - 1, distanceFromEnd + 1);
            }
        }

    }
}
