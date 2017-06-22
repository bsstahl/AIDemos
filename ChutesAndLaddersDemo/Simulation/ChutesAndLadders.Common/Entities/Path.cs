using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Entities
{
    public class Path: IEnumerable<GameSpace>
    {
        List<GameSpace> _path = new List<GameSpace>();

        public Path() { }

        public Path(GameSpace space)
        {
            _path.Add(space);
        }

        public int Length { get => _path.Count(); }

        public void AddInFront(GameSpace space)
        {
            var newPath = new List<GameSpace>() { space };
            foreach (var oldSpace in _path)
                newPath.Add(oldSpace);
            _path = newPath;
        }

        public override string ToString()
        {
            return $"Path: {string.Join(",", _path.Select(s => s.ToString()))}";
        }

        public IEnumerator<GameSpace> GetEnumerator()
        {
            return _path.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _path.GetEnumerator();
        }
    }
}
