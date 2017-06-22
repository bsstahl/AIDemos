using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.Entities;
using ChutesAndLadders.GamePlay;

namespace Chute
{
    public static class SupplementalDemoExtensions
    {
        public static void SupplementalDemo_SingleGame(this GameBoard board)
        {
            var greedyStrategy = new ChutesAndLadders.Strategy.Greedy.Engine();
            var linearStrategy = new ChutesAndLadders.Strategy.Linear.Engine();

            var geneticStrategy = new ChutesAndLadders.Strategy.Rules.Engine("Best Found");
            geneticStrategy.AddBestStrategyRules();

            var shortestPathStrategy = new ChutesAndLadders.Strategy.ShortestPath.Engine();

            // Remember that player 1 has a big advantage
            var player1 = new Player("Player 1", greedyStrategy);
            var player2 = new Player("Player 2", linearStrategy);
            var player3 = new Player("Player 3", geneticStrategy);
            var player4 = new Player("Player 4", shortestPathStrategy);
            var players = new Player[] { player1, player2, player3, player4 };

            var gameEngine = new Game(board);

            // Set the last parameter to a value 1-6 to have all rolls be that value
            // This allow us to compare the strategies when all use the same roll
            // It would be better if the roll varied by round but all players 
            // got the same roll in a given round
            var results = gameEngine.Play(players, 1, 16, true, 0);

            Console.WriteLine(results.ToString());
            Console.WriteLine();
            Console.WriteLine(player4.Strategy.ToString());
        }

        public static void SupplementalDemo_Player1Advantage(this GameBoard board)
        {
            const int gameCount = 36000;
            const int maxStartingLocation = 33;

            var gameStrategy = new ChutesAndLadders.Strategy.Greedy.Engine();
            // var gameStrategy = new ChutesAndLadders.Strategy.TakeAllLadders.Engine();
            // var gameStrategy = new ChutesAndLadders.Strategy.Linear.Engine();

            var player1 = new Player("Player 1", gameStrategy);
            var player2 = new Player("Player 2", gameStrategy);
            var player3 = new Player("Player 3", gameStrategy);
            var player4 = new Player("Player 4", gameStrategy);
            var player5 = new Player("Player 5", gameStrategy);
            var player6 = new Player("Player 6", gameStrategy);
            var players = new Player[] { player1, player2, player3, player4, player5, player6 };

            var engine = new Simulation(maxStartingLocation);
            var results = engine.Run(players, gameCount);

            foreach (var player in results)
                Console.WriteLine($"{player.Name} ({player.Strategy.Name}) won {player.WinCount} of the {gameCount} games.");
        }
    }
}
