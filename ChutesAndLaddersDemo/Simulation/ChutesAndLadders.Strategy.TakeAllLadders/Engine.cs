using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.TakeAllLadders
{
    public class Engine : Strategy.Rules.Engine
    {
        public Engine() : base("Take All Ladders")
        {
            this.AddTakeLadderRules(80, 100);
            this.AddTakeLadderRules(71, 91);
            this.AddTakeLadderRules(51, 67);
            this.AddTakeLadderRules(36, 44);
            this.AddTakeLadderRules(28, 84);
            this.AddTakeLadderRules(21, 42);
            this.AddTakeLadderRules(9, 31);
            this.AddTakeLadderRules(4, 14);
            this.AddTakeLadderRules(1, 38);
        }
    }
}
