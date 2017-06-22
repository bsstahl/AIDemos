using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Entities
{
    public class GameResults
    {
        public int GameId { get; set; }
        public Player Winner { get; set; }
        public IEnumerable<Player> Players { get; set; }
        public int Turns { get; set; }


        public override string ToString()
        {
            return $"{this.GameId:0000000000} Winner is {this.Winner.Name} using {this.Winner.Strategy.Name} strategy in {this.Turns} turns";
        }
    }
}
