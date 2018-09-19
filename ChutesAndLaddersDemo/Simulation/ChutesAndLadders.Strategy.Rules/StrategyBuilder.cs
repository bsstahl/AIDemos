using System;
using System.Collections.Generic;
using System.Text;

namespace ChutesAndLadders.Strategy.Rules
{
    public class StrategyBuilder: Engine
    {
        public Engine Build()
        {
            return this;
        }

        public new StrategyBuilder Name(string name)
        {
            base.Name = name;
            return this;
        }

        public StrategyBuilder TakeLadder(int ladderOrigin, int ladderTerminus)
        {
            base.AddTakeLadderRules(ladderOrigin, ladderTerminus);
            return this;
        }

        public StrategyBuilder TakeChute(int chuteOrigin, int chuteTerminus)
        {
            base.AddTakeChuteRules(chuteOrigin, chuteTerminus);
            return this;
        }
    }
}
