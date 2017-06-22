using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.Rules
{
    internal class StrategyRule
    {
        public Func<int, byte, bool> MeetsConditions { get; set; }

        public Func<int, byte, int> Result { get; set; }

        public string Name { get; set; }

        public StrategyRule(string name, Func<int, byte, bool> conditionsFunction, Func<int, byte, int> resultsFunction)
        {
            this.Name = name;
            this.MeetsConditions = conditionsFunction;
            this.Result = resultsFunction;
        }

        public override string ToString()
        {
            return this.Name;
        }

    }
}
