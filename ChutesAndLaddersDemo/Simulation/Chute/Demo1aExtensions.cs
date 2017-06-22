using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.Entities;
using ChutesAndLadders.GamePlay;

namespace Chute
{
    public static class Demo1aExtensions
    {
        public static void Demo1a_GreedyAlgorithm(this GameBoard board)
        {
            var greedyStrategy = new ChutesAndLadders.Strategy.Greedy.Engine();
            var badStrategy = new ChutesAndLadders.Strategy.AgressivelyBad.Engine();
            var linearStrategy = new ChutesAndLadders.Strategy.Linear.Engine();

            var player1 = new Player("Player 1", greedyStrategy);
            var player2 = new Player("Player 2", badStrategy);
            var player3 = new Player("Player 3", linearStrategy);
            var players = new Player[] { player1, player2, player3 };

            var engine = new SimulationCollection();
            engine.RunSimulations(players, 30000, true);
        }
    }
}
