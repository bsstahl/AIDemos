using ChutesAndLadders.Builders;
using ChutesAndLadders.Entities;
using ChutesAndLadders.GamePlay;
using ChutesAndLadders.Interfaces;
using System;
using System.Linq;

namespace ChutesAndLadders.Demos
{
    public class Genetics
    {

        public static void Evolution(int maxGenerations, int simulationsPerGeneration, double misspellingRate)
        {
            var basicGeneticStrategy = new ChutesAndLadders.Strategy.Genetic.Engine();

            var players = new PlayerCollectionBuilder()
                .Add("Player 1", basicGeneticStrategy.Evolve(misspellingRate))
                .Add("Player 2", basicGeneticStrategy.Evolve(misspellingRate))
                .Add("Player 3", basicGeneticStrategy.Evolve(misspellingRate))
                .Add("Player 4", basicGeneticStrategy.Evolve(misspellingRate))
                .Add("Player 5", basicGeneticStrategy.Evolve(misspellingRate))
                .Add("Player 6", basicGeneticStrategy.Evolve(misspellingRate))
                .Build();

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

                var bestStrategy = bestPlayers.First().Strategy;
                var runnerUpStrategy = bestPlayers.Last().Strategy;

                players = new PlayerCollectionBuilder()
                    .Add("Player 1", bestStrategy)
                    .Add("Player 2", runnerUpStrategy)
                    .Add("Player 3", (bestStrategy as Strategy.Genetic.Engine).Evolve())
                    .Add("Player 4", (bestStrategy as Strategy.Genetic.Engine).Evolve())
                    .Add("Player 5", (bestStrategy as Strategy.Genetic.Engine).Evolve())
                    .Add("Player 6", (runnerUpStrategy as Strategy.Genetic.Engine).Evolve())
                    .Build();

                generationCount++;
            }


            // Run the original (linear) strategy against the best one found genetically
            var rootStrategy = new ChutesAndLadders.Strategy.Genetic.Engine("GeneticRoot");
            var finalStrategy = players.OrderByDescending(p => p.WinCount).First().Strategy as Strategy.Genetic.Engine;

            new SimulationCollectionBuilder()
                .AddPlayer("Player 1", finalStrategy)
                .AddPlayer("Player 2", finalStrategy)
                .AddPlayer("Player 3", finalStrategy)
                .AddPlayer("Player 4", rootStrategy)
                .AddPlayer("Player 5", rootStrategy)
                .AddPlayer("Player 6", rootStrategy)
                .MaxExecutionCount(simulationsPerGeneration)
                .OutputResults(true)
                .Run();

            Console.WriteLine($"{finalStrategy.Name} rules:");
            Console.WriteLine(finalStrategy.ContrastWith(rootStrategy));
        }

        public static ChutesAndLadders.Strategy.Rules.Engine GetBestStrategy()
        {
            var strategy = new ChutesAndLadders.Strategy.Rules.Engine("Best Found");
            strategy.AddRule(0, 3, 39);
            strategy.AddRule(0, 4, 40);
            strategy.AddRule(0, 5, 14);
            strategy.AddRule(0, 6, 42);
            strategy.AddRule(1, 1, 38);
            strategy.AddRule(1, 3, 40);
            strategy.AddRule(1, 4, 14);
            strategy.AddRule(1, 5, 42);
            strategy.AddRule(1, 6, 16);
            strategy.AddRule(2, 3, 14);
            strategy.AddRule(2, 4, 15);
            strategy.AddRule(2, 5, 16);
            strategy.AddRule(2, 6, 17);
            strategy.AddRule(3, 2, 14);
            strategy.AddRule(3, 4, 16);
            strategy.AddRule(3, 5, 17);
            strategy.AddRule(3, 6, 18);
            strategy.AddRule(4, 1, 14);
            strategy.AddRule(4, 3, 16);
            strategy.AddRule(4, 4, 17);
            strategy.AddRule(4, 5, 18);
            strategy.AddRule(4, 6, 31);
            strategy.AddRule(6, 4, 31);
            strategy.AddRule(7, 4, 32);
            strategy.AddRule(8, 2, 31);
            strategy.AddRule(8, 3, 32);
            strategy.AddRule(8, 5, 34);
            strategy.AddRule(9, 1, 31);
            strategy.AddRule(9, 4, 34);
            strategy.AddRule(9, 6, 36);
            strategy.AddRule(11, 6, 6);
            strategy.AddRule(12, 5, 6);
            strategy.AddRule(13, 6, 8);
            strategy.AddRule(14, 3, 6);
            strategy.AddRule(14, 5, 8);
            strategy.AddRule(14, 6, 9);
            strategy.AddRule(15, 4, 8);
            strategy.AddRule(15, 6, 10);
            strategy.AddRule(16, 2, 7);
            strategy.AddRule(16, 4, 9);
            strategy.AddRule(16, 5, 10);
            strategy.AddRule(16, 6, 42);
            strategy.AddRule(17, 5, 42);
            strategy.AddRule(19, 4, 43);
            strategy.AddRule(19, 5, 44);
            strategy.AddRule(19, 6, 45);
            strategy.AddRule(20, 2, 42);
            strategy.AddRule(20, 5, 45);
            strategy.AddRule(20, 6, 46);
            strategy.AddRule(21, 1, 42);
            strategy.AddRule(21, 2, 43);
            strategy.AddRule(21, 3, 44);
            strategy.AddRule(21, 5, 46);
            strategy.AddRule(23, 6, 84);
            strategy.AddRule(24, 5, 84);
            strategy.AddRule(24, 6, 85);
            strategy.AddRule(25, 4, 84);
            strategy.AddRule(25, 5, 85);
            strategy.AddRule(25, 6, 86);
            strategy.AddRule(26, 3, 84);
            strategy.AddRule(26, 4, 85);
            strategy.AddRule(26, 5, 86);
            strategy.AddRule(26, 6, 87);
            strategy.AddRule(27, 2, 84);
            strategy.AddRule(27, 3, 85);
            strategy.AddRule(27, 4, 86);
            strategy.AddRule(27, 5, 87);
            strategy.AddRule(27, 6, 24);
            strategy.AddRule(28, 1, 84);
            strategy.AddRule(28, 2, 85);
            strategy.AddRule(28, 3, 86);
            strategy.AddRule(28, 4, 87);
            strategy.AddRule(28, 5, 88);
            strategy.AddRule(28, 6, 89);
            strategy.AddRule(31, 6, 44);
            strategy.AddRule(34, 3, 44);
            strategy.AddRule(34, 5, 46);
            strategy.AddRule(34, 6, 47);
            strategy.AddRule(35, 2, 44);
            strategy.AddRule(35, 3, 45);
            strategy.AddRule(36, 1, 44);
            strategy.AddRule(36, 5, 48);
            strategy.AddRule(36, 6, 27);
            strategy.AddRule(43, 5, 26);
            strategy.AddRule(43, 6, 27);
            strategy.AddRule(44, 4, 26);
            strategy.AddRule(44, 6, 11);
            strategy.AddRule(45, 3, 26);
            strategy.AddRule(45, 4, 27);
            strategy.AddRule(45, 5, 28);
            strategy.AddRule(45, 6, 29);
            strategy.AddRule(46, 4, 28);
            strategy.AddRule(46, 5, 84);
            strategy.AddRule(46, 6, 30);
            strategy.AddRule(47, 2, 27);
            strategy.AddRule(47, 3, 11);
            strategy.AddRule(47, 4, 12);
            strategy.AddRule(47, 5, 85);
            strategy.AddRule(47, 6, 31);
            strategy.AddRule(48, 2, 11);
            strategy.AddRule(48, 3, 12);
            strategy.AddRule(48, 4, 67);
            strategy.AddRule(48, 5, 14);
            strategy.AddRule(48, 6, 69);
            strategy.AddRule(49, 1, 11);
            strategy.AddRule(49, 3, 13);
            strategy.AddRule(49, 4, 68);
            strategy.AddRule(49, 5, 15);
            strategy.AddRule(49, 6, 16);
            strategy.AddRule(50, 2, 67);
            strategy.AddRule(50, 5, 70);
            strategy.AddRule(51, 1, 67);
            strategy.AddRule(51, 6, 53);
            strategy.AddRule(52, 5, 53);
            strategy.AddRule(52, 6, 54);
            strategy.AddRule(53, 6, 55);
            strategy.AddRule(54, 3, 53);
            strategy.AddRule(55, 5, 56);
            strategy.AddRule(55, 6, 53);
            strategy.AddRule(56, 3, 55);
            strategy.AddRule(56, 5, 57);
            strategy.AddRule(56, 6, 58);
            strategy.AddRule(57, 6, 19);
            strategy.AddRule(58, 6, 20);
            strategy.AddRule(59, 4, 19);
            strategy.AddRule(59, 5, 20);
            strategy.AddRule(59, 6, 60);
            strategy.AddRule(60, 5, 60);
            strategy.AddRule(60, 6, 22);
            strategy.AddRule(61, 3, 20);
            strategy.AddRule(61, 4, 60);
            strategy.AddRule(61, 5, 61);
            strategy.AddRule(61, 6, 62);
            strategy.AddRule(62, 3, 21);
            strategy.AddRule(62, 4, 42);
            strategy.AddRule(62, 5, 23);
            strategy.AddRule(62, 6, 63);
            strategy.AddRule(63, 2, 60);
            strategy.AddRule(63, 5, 63);
            strategy.AddRule(64, 2, 61);
            strategy.AddRule(64, 3, 62);
            strategy.AddRule(64, 4, 63);
            strategy.AddRule(64, 5, 20);
            strategy.AddRule(64, 6, 60);
            strategy.AddRule(68, 4, 91);
            strategy.AddRule(69, 3, 91);
            strategy.AddRule(69, 5, 93);
            strategy.AddRule(69, 6, 73);
            strategy.AddRule(70, 4, 93);
            strategy.AddRule(70, 5, 94);
            strategy.AddRule(70, 6, 74);
            strategy.AddRule(71, 3, 93);
            strategy.AddRule(71, 5, 74);
            strategy.AddRule(71, 6, 96);
            strategy.AddRule(83, 6, 25);
            strategy.AddRule(86, 2, 24);
            strategy.AddRule(86, 5, 27);
            strategy.AddRule(87, 1, 24);
            strategy.AddRule(87, 4, 27);
            strategy.AddRule(87, 6, 29);
            strategy.AddRule(91, 5, 75);
            strategy.AddRule(92, 5, 76);
            strategy.AddRule(93, 2, 74);
            strategy.AddRule(93, 4, 76);
            strategy.AddRule(93, 6, 78);
            strategy.AddRule(94, 2, 75);
            strategy.AddRule(94, 4, 77);
            strategy.AddRule(98, 1, 78);
            return strategy;
        }

        public static void Analysis()
        {
            int maxOptions = 0;
            int totalConditions = 0;
            int totalConditionalOptions = 0;

            var board = new GameBoard();
            for (int startingPoint = 0; startingPoint < 100; startingPoint++)
            {
                for (byte spin = 1; spin < 7; spin++)
                {
                    var endPoints = board.GetLegalEndpoints(startingPoint, spin);
                    int currentOptions = endPoints.Count();
                    if ((currentOptions > 1) && (!endPoints.Contains(100)))
                    {
                        totalConditions++;
                        totalConditionalOptions += currentOptions;
                        if (currentOptions > maxOptions)
                            maxOptions = currentOptions;
                        Console.WriteLine($"{startingPoint},{spin},{currentOptions}");
                    }
                }
            }

            Console.WriteLine($"Max Options: {maxOptions}");
            Console.WriteLine($"Total Conditions: {totalConditions}");
            Console.WriteLine($"Total Conditional Options: {totalConditionalOptions}");
        }


    }
}
