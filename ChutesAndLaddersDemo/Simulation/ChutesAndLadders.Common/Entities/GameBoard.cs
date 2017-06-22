using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Entities
{
    public class GameBoard : List<GameSpace>
    {
        public GameBoard()
        {
            Load();
        }

        public IEnumerable<GameSpace> NextSpaces(int index)
        {
            if (index < 0)
                throw new ArgumentException("Space must be on the gameboard or at the starting point", nameof(index));

            var result = new List<GameSpace>();
            if (index <= 99)
            {
                var nextSpace = this[index + 1];
                result.Add(nextSpace);
                if (index > 0 && this[index].PathTo.HasValue)
                    result.Add(this[this[index].PathTo.Value]);
            }

            return result;
        }

        public IEnumerable<int> GetLegalEndpoints(int startingPoint, byte spin)
        {
            var paths = this.GetPaths(startingPoint, spin);
            return paths.Select(p => p.Last().Index).Distinct();
        }

        public IEnumerable<Path> GetPaths(int startingPoint, byte spin)
        {
            byte lowestSpinValue = 1;

            if (spin < 0)
                throw new ArgumentException("Spin must be non-negative", nameof(spin));

            var paths = new List<Path>();

            var nextSpaces = NextSpaces(startingPoint);
            if (spin == lowestSpinValue)
            {
                foreach (var nextSpace in nextSpaces)
                    paths.Add(new Path(nextSpace));
            }
            else
            {
                byte nextSpin = Convert.ToByte(spin - 1);
                foreach (var nextSpace in nextSpaces)
                {
                    var nextPaths = GetPaths(nextSpace.Index, nextSpin);
                    foreach (var nextPath in nextPaths)
                        nextPath.AddInFront(nextSpace);
                    paths.AddRange(nextPaths);
                }
            }

            return paths;
        }

        public new GameSpace this[int index]
        {
            get { return base[index - 1]; }
        }

        private void Load()
        {
            for (int i = 1; i <= 100; i++)
                this.Add(new GameSpace(i));
        }
    }
}
