using ChutesAndLadders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.GamePlay;

namespace Chute
{
    public static class Demo3bExtensions
    {
        public static void Demo3b_GeneticEvolution(this GameBoard board)
        {
            const int maxGenerations = 300;
            const int simulationsPerGeneration = 1200;
            const double misspellingRate = 0.05;

            var basicGeneticStrategy = new ChutesAndLadders.Strategy.Genetic.Engine();

            var player1 = new Player("Player 1", basicGeneticStrategy.Evolve(misspellingRate));
            var player2 = new Player("Player 2", basicGeneticStrategy.Evolve(misspellingRate));
            var player3 = new Player("Player 3", basicGeneticStrategy.Evolve(misspellingRate));
            var player4 = new Player("Player 4", basicGeneticStrategy.Evolve(misspellingRate));
            var player5 = new Player("Player 5", basicGeneticStrategy.Evolve(misspellingRate));
            var player6 = new Player("Player 6", basicGeneticStrategy.Evolve(misspellingRate));
            var players = new Player[] { player1, player2, player3, player4, player5, player6 };

            var engine = new SimulationCollection();

            int mostWins = 0;
            int generationCount = 0;
            while (generationCount < maxGenerations)
            {
                players = engine.RunSimulations(players, simulationsPerGeneration).Players.ToArray();

                // Create the next generation of Players/Strategies
                // by evolving based on the strategies of the top 2 players
                var bestPlayers = players.OrderByDescending(p => p.WinCount).Take(2);
                if (bestPlayers.First().WinCount > mostWins)
                {
                    mostWins = bestPlayers.First().WinCount;
                    Console.WriteLine($"Generation {generationCount} (most wins {mostWins}):");
                }

                player1 = new Player("Player 1", bestPlayers.First().Strategy);
                player2 = new Player("Player 2", bestPlayers.Last().Strategy);
                player3 = new Player("Player 3", (player1.Strategy as ChutesAndLadders.Strategy.Genetic.Engine).Evolve());
                player4 = new Player("Player 4", (player1.Strategy as ChutesAndLadders.Strategy.Genetic.Engine).Evolve());
                player5 = new Player("Player 5", (player1.Strategy as ChutesAndLadders.Strategy.Genetic.Engine).Evolve());
                player6 = new Player("Player 6", (player2.Strategy as ChutesAndLadders.Strategy.Genetic.Engine).Evolve());
                players = new Player[] { player1, player2, player3, player4, player5, player6 };

                generationCount++;
            }


            // Run the original (linear) strategy against the best one found genetically
            var rootStrategy = new ChutesAndLadders.Strategy.Genetic.Engine("GeneticRoot");
            var finalStrategy = players.OrderByDescending(p => p.WinCount).First().Strategy as ChutesAndLadders.Strategy.Genetic.Engine;

            var p1 = new Player("Player 1", finalStrategy);
            var p2 = new Player("Player 2", finalStrategy);
            var p3 = new Player("Player 3", finalStrategy);
            var p4 = new Player("Player 4", rootStrategy);
            var p5 = new Player("Player 5", rootStrategy);
            var p6 = new Player("Player 6", rootStrategy);
            var finalPlayers = new Player[] { p1, p2, p3, p4, p5, p6 };
            engine.RunSimulations(finalPlayers, simulationsPerGeneration, true);

            Console.WriteLine($"{finalStrategy.Name} rules:");
            Console.WriteLine(finalStrategy.ContrastWith(rootStrategy));
        }

    }
}
