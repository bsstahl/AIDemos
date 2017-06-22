using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.GreedyRules
{
    public class Engine : Rules.Engine
    {
        public Engine() : base("Greedy Rules")
        {
            Load();
        }

        private void Load()
        {
            var board = new Entities.GameBoard();
            for (int startingPoint = 0; startingPoint < 100; startingPoint++)
            {
                for (byte spin = 1; spin <= 6; spin++)
                {
                    var legalEndpoints = board.GetLegalEndpoints(startingPoint, spin);
                    if (legalEndpoints.Count() > 1)
                    {
                        // Capture variables for lambda
                        int sPoint = startingPoint;
                        int sValue = spin;
                        int bestEndpoint = legalEndpoints.Max();
                        string name = $"Start:{sPoint} Spin:{sValue} End:{bestEndpoint}";
                        Func<int, byte, bool> conditionsFunction = (st, sp) => { return (st == sPoint && sp == sValue); };
                        Func<int, byte, int> resultsFunction = (st, sp) => { return bestEndpoint; };
                        base.AddRule(name, conditionsFunction, resultsFunction);
                    }
                }
            }
        }
    }
}
