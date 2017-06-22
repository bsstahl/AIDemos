using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.Entities;
using ChutesAndLadders.GamePlay;

namespace Chute
{
    public static class Demo4aExtensions
    {
        public static void Demo4a_ShortestPath(this GameBoard board)
        {
            var greedyStrategy = new ChutesAndLadders.Strategy.Greedy.Engine();
            var linearStrategy = new ChutesAndLadders.Strategy.Linear.Engine();

            var geneticStrategy = new ChutesAndLadders.Strategy.Rules.Engine("Best Found");
            geneticStrategy.AddBestStrategyRules();

            var shortestPathStrategy = new ChutesAndLadders.Strategy.ShortestPath.Engine();

            var player1 = new Player("Player 1", shortestPathStrategy);
            var player2 = new Player("Player 2", linearStrategy);
            var player3 = new Player("Player 3", geneticStrategy);
            var player4 = new Player("Player 4", greedyStrategy);
            var players = new Player[] { player1, player2, player3, player4 };

            var engine = new SimulationCollection();
            engine.RunSimulations(players, 100000, true);
        }

    }
}
