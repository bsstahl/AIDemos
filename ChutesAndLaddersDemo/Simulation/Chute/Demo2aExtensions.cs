using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.Entities;
using ChutesAndLadders.GamePlay;

namespace Chute
{
    public static class Demo2aExtensions
    {
        public static void Demo2a_RulesEngine(this GameBoard board)
        {
            var linearStrategy = new ChutesAndLadders.Strategy.Rules.Engine();
            var greedyRulesStrategy = new ChutesAndLadders.Strategy.GreedyRules.Engine();
            var ladderRulesStrategy = new ChutesAndLadders.Strategy.TakeAllLadders.Engine();

            var player1 = new Player("Player 1", linearStrategy);
            var player2 = new Player("Player 2", greedyRulesStrategy);
            var player3 = new Player("Player 3", ladderRulesStrategy);
            var players = new Player[] { player1, player2, player3 };

            var engine = new SimulationCollection();
            engine.RunSimulations(players, 30000, true);
        }
    }
}
