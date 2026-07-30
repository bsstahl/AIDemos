using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.ShortestPath
{
    internal sealed class Gameboard
    {
        List<Gamespace> _spaces = new List<Gamespace>();

        public Gameboard(int size, IEnumerable<KeyValuePair<int, int>> pathways)
        {
            var pathwayList = pathways.ToArray();
            _spaces.Clear();
            for (int i = 1; i <= size; i++)
            {
                var newSpace = new Gamespace() { Index = i };
                var pathway = pathwayList.FirstOrDefault(p => p.Key == i);
                if (!pathway.Equals(default(KeyValuePair<int, int>)))
                    newSpace.PathTo = pathway.Value;
                _spaces.Add(newSpace);
            }
        }

        public Gamespace this[int index]
        {
            get { return _spaces.Single(s => s.Index == index); }
        }

        public int LastIndex
        {
            get { return _spaces.Count; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = _spaces.Count; i > 0; i = i - 10)
            {
                var spacesInRow = _spaces.Where(s => s.Index <= i && s.Index > (i - 10));
                bool rowIsOdd = ((i / 10) % 2 == 1);
                if (rowIsOdd)
                    for (int j = 9; j >= 0; j--)
                        sb.AppendGamespaceDistance(this[i - j]);
                else
                    for (int j = 0; j < 10; j++)
                        sb.AppendGamespaceDistance(this[i - j]);

                sb.AppendLine();
            }

            sb.AppendLine();

            return sb.ToString();
        }

        public Gamespace PathFrom(int spaceIndex)
        {
            return _spaces.SingleOrDefault(s => s.PathTo.HasValue && s.PathTo.Value == spaceIndex);
        }

        public IEnumerable<Gamespace> SpacesAtDistance(int distance)
        {
            return _spaces.Where(s => s.DistanceFromEnd == distance);
        }

    }
}
