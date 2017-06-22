using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Entities
{
    public class GameSituation
    {
        public int BoardLocation { get; set; }
        public byte Spin { get; set; }
        public IEnumerable<int> LegalMoves { get; set; }
        public IEnumerable<int> PlayerLocations { get; set; }
    }
}
