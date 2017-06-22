using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.Rules
{
    internal class RulesCollection
    {

        Stack<StrategyRule> _rulesStack = new Stack<StrategyRule>();
        StrategyRule _defaultRule;

        public int Count { get => _rulesStack.Count(); }

        public RulesCollection()
        {
            // Create default rule
            _defaultRule = new StrategyRule(
                "Default Linear Strategy Rule",
                (start, spin) => { return true; },
                (start, spin) => { return start + spin; }
                );
        }

        public StrategyRule Add(StrategyRule rule)
        {
            _rulesStack.Push(rule);
            return rule;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var rule in _rulesStack)
                sb.AppendLine(rule.ToString());
            return sb.ToString();
        }

        internal int Process(int startingPoint, byte spin)
        {
            int result = 0;
            bool ruleProcessed = false;

            foreach (var rule in _rulesStack)
                if (!ruleProcessed)
                {
                    if (rule.MeetsConditions(startingPoint, spin))
                    {
                        ruleProcessed = true;
                        result = rule.Result(startingPoint, spin);
                    }
                }

            if (!ruleProcessed)
                result = _defaultRule.Result(startingPoint, spin);

            return result;
        }
    }
}
