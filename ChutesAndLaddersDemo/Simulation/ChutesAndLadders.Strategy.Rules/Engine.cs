using ChutesAndLadders.Entities;
using ChutesAndLadders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.Rules
{
    public class Engine : Interfaces.IGameStrategy
    {
        RulesCollection _rules = new RulesCollection();

        public string Name { get; protected set; }

        public int WinCount { get; set; }

        public Engine(string name = "Default Rules")
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return _rules.ToString();
        }

        public int GetMove(GameSituation situation)
        {
            var result = _rules.Process(situation.BoardLocation, situation.Spin);
            if (!situation.LegalMoves.Contains(result))
                throw new InvalidOperationException($"Illegal result returned {result}. Legal options: {string.Join(",", situation.LegalMoves)}");
            return result;
        }

        public void AddRule(string name, Func<int, byte, bool> conditionsFunction, Func<int, byte, int> resultsFunction)
        {
            _rules.Add(new StrategyRule(name, conditionsFunction, resultsFunction));
        }

        public void AddRule(int startingPoint, int spin, int bestEndpoint)
        {
            string name = $"Start:{startingPoint} Spin:{spin} End:{bestEndpoint}";
            this.AddRule(name, startingPoint, spin, bestEndpoint);
        }

        public void AddRule(string name, int startingPoint, int spin, int bestEndpoint)
        {
            Func<int, byte, bool> conditionsFunction = (st, sp) => { return (st == startingPoint && sp == spin); };
            Func<int, byte, int> resultsFunction = (st, sp) => { return bestEndpoint; };
            this.AddRule(name, conditionsFunction, resultsFunction);
        }

        public void AddTakeLadderRules(int ladderOrigin, int ladderTerminus)
        {
            for (int s = ladderOrigin - 5; s <= ladderOrigin; s++)
            {
                if (s >= 0)
                {
                    int sCapture = s;
                    this.AddRule(
                        $"Take ladder at {ladderOrigin} when {sCapture}<=startingPoint<={ladderOrigin} and spin>={(ladderOrigin - sCapture + 1)}",
                        (startingPoint, spin) =>
                        {
                            return ((startingPoint >= sCapture)
                                && (startingPoint <= ladderOrigin)
                                && (spin >= (ladderOrigin - sCapture + 1)));
                        },
                        (start, spin) =>
                        {
                            return sCapture + spin + ladderTerminus - ladderOrigin - 1;
                        });
                }
            }
        }

        public void AddTakeChuteRules(int chuteOrigin, int chuteTerminus)
        {
            for (int s = chuteOrigin - 5; s <= chuteOrigin; s++)
            {
                int sCapture = s;
                this.AddRule(
                    $"Take Chute at {chuteOrigin} when {sCapture}<=startingPoint<={chuteOrigin} and spin>={(chuteOrigin - sCapture + 1)}",
                    (startingPoint, spin) =>
                    {
                        return ((startingPoint >= sCapture)
                            && (startingPoint <= chuteOrigin)
                            && (spin >= (chuteOrigin - sCapture + 1)));
                    },
                    (start, spin) =>
                    {
                        return sCapture + spin + chuteTerminus - chuteOrigin - 1;
                    });
            }
        }

    }
}
