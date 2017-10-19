using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Entities
{
    public class GameAction
    {
        public Guid UniqueGameId { get; set; }
        public Guid PlayerId { get; set; }
        public int PlayerNumber { get; set; }
        public int BoardLocation { get; set; }
        public byte Spin { get; set; }
        public IEnumerable<int> PlayerLocations { get; set; }
        public int SelectedMove { get; set; }
        public bool? PlayerWonGame { get; set; }
    }
}
