using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Entities
{
    public class SimulationResults
    {
        public IEnumerable<Player> Players { get; set; }
        public IEnumerable<GameAction> GameActions { get; set; }
    }
}
