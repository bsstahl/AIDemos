using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.GamePlay;
using ChutesAndLadders.Strategy.Rules;
using ChutesAndLadders.Entities;
using ChutesAndLadders.Demos;
using ChutesAndLadders.Builders;
using System.Diagnostics;

namespace Chute
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Demo 1 - Greedy Algorithm

            // This demo shows the starting point for most AI projects, a 
            // greedy algorithm. In this case, we 
            // "grab for as much value as we can, as simply as we can"
            // by selecting the space with the largest index. That is,
            // we try to move as high up on the board as possible. This is not
            // likely to be the best strategy, but might be a good 
            // heuristic for the best strategy.

            //new SimulationCollectionBuilder()
            //    .AddPlayer("Player1", new ChutesAndLadders.Strategy.Greedy.Engine())
            //    .AddPlayer("Player2", new ChutesAndLadders.Strategy.AgressivelyBad.Engine())
            //    .AddPlayer("Player3", new ChutesAndLadders.Strategy.Linear.Engine())
            //    .MaxExecutionCount(30000)
            //    .OutputResults(true)
            //    .Run();

            #endregion

            #region Demo 2a -- Rules Engine

            // In this demo, we implement much of the same logic but
            // using a rules engine. While this is not strictly necessary
            // here, having the rules engine and knowing how to use it
            // will enable a number of the demos below

            //new SimulationCollectionBuilder()
            //    .AddPlayer("Player1", new ChutesAndLadders.Strategy.Rules.Engine())
            //    .AddPlayer("Player2", new ChutesAndLadders.Strategy.GreedyRules.Engine())
            //    .AddPlayer("Player3", new ChutesAndLadders.Strategy.TakeAllLadders.Engine())
            //    .MaxExecutionCount(30000)
            //    .OutputResults(true)
            //    .Run();

            #endregion

            #region Demo 2b -- Improved Linear Strategies in the Rules Engine

            // TODO: Verify there is no bug in the simulation engine. 
            // These results don't look right to me.  It seems like
            // Improved Linear 2 should be better than Improved Linear 1

            //new SimulationCollectionBuilder()
            //    .AddPlayer("Player1", new ChutesAndLadders.Strategy.Linear.Engine())
            //    .AddPlayer("Player2", new StrategyBuilder()
            //        .Name("Improved Linear 1")
            //        .TakeLadder(28, 84)
            //        .TakeLadder(80, 100)
            //        .Build())
            //    .AddPlayer("Player3", new StrategyBuilder()
            //        .Name("Improved Linear 2")
            //        .TakeLadder(28, 84)
            //        .TakeLadder(80, 100)
            //        .TakeChute(98, 78)
            //        .Build())
            //    .AddPlayer("Player4", new StrategyBuilder()
            //        .Name("Improved Linear 3")
            //        .TakeLadder(1, 38)
            //        .TakeLadder(51, 67)
            //        .TakeLadder(71, 91)
            //        .Build())
            //    .MaxExecutionCount(30000)
            //    .OutputResults(true)
            //    .Run();

            #endregion

            #region Demo 3a - Genetic Analysis

            // Only show demos 3a and 3c if there is time available at the end of the presentation
            // Genetics.Analysis();

            #endregion

            #region Demo 3b - Genetic Evolution

            // Demo 3b is generally the only one that needs to be shown, since
            // it is the actual primary evolution demo.
            Genetics.Evolution(500, 500, 0.1);

            #endregion

            #region Demo 3c - Genetic Superiority

            // Only show demos 3a and 3c if there is time available at the end of the presentation
            //new SimulationCollectionBuilder()
            //    .AddPlayer("Player 1", new ChutesAndLadders.Strategy.Greedy.Engine())
            //    .AddPlayer("Player 2", new ChutesAndLadders.Strategy.Linear.Engine())
            //    .AddPlayer("Player 3", Genetics.GetBestStrategy())
            //    .MaxExecutionCount(100000)
            //    .OutputResults(true)
            //    .Run();

            #endregion

            #region Demo 4 - Shortest Path

            //new SimulationCollectionBuilder()
            //    .AddPlayer("Player 1", new ChutesAndLadders.Strategy.ShortestPath.Engine())
            //    .AddPlayer("Player 2", new ChutesAndLadders.Strategy.Linear.Engine())
            //    .AddPlayer("Player 3", Genetics.GetBestStrategy())
            //    .AddPlayer("Player 4", new ChutesAndLadders.Strategy.Greedy.Engine())
            //    .MaxExecutionCount(100000)
            //    .OutputResults(true)
            //    .Run();

            #endregion

            #region Supplemental Demo 1 - Single Game

            //// Remember that player 1 has a significant advantage
            //var players = new PlayerCollectionBuilder()
            //            .Add("Player 1", new ChutesAndLadders.Strategy.Greedy.Engine())
            //            .Add("Player 2", new ChutesAndLadders.Strategy.Linear.Engine())
            //            .Add("Player 3", Genetics.GetBestStrategy())
            //            .Add("Player 4", new ChutesAndLadders.Strategy.ShortestPath.Engine())
            //            .Build();

            //var gameEngine = new Game(new GameBoard());

            //// Set the last parameter to a value 1-6 to have all rolls be that value
            //// This allow us to compare the strategies when all use the same roll
            //// It would be a better simulation if the roll varied by round but all players 
            //// got the same roll in a given round
            //var results = gameEngine.Play(players, 1, 16, true, 0);

            //Console.WriteLine(results.ToString());

            #endregion

            #region Supplemental Demo 2 - Player 1 Advantage

            //const int gameCount = 36000;
            //const int maxStartingLocation = 33;

            //var gameStrategy = new ChutesAndLadders.Strategy.Greedy.Engine();
            //// var gameStrategy = new ChutesAndLadders.Strategy.TakeAllLadders.Engine();
            //// var gameStrategy = new ChutesAndLadders.Strategy.Linear.Engine();

            //var players = new PlayerCollectionBuilder()
            //    .Add("Player 1", gameStrategy)
            //    .Add("Player 2", gameStrategy)
            //    .Add("Player 3", gameStrategy)
            //    .Add("Player 4", gameStrategy)
            //    .Add("Player 5", gameStrategy)
            //    .Add("Player 6", gameStrategy)
            //    .Build();

            //var engine = new Simulation(maxStartingLocation);
            //var results = engine.Run(players, gameCount);

            //foreach (var player in results.Players)
            //    Console.WriteLine($"{player.Name} ({player.Strategy.Name}) won {player.WinCount} of the {gameCount} games.");

            #endregion

            #region Data Generation - Game Action Output (small sample)

            // DataGeneration.CreateRandom(60000, @".\");

            #endregion
        }
    }
}
