using ChutesAndLadders.GamePlay;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChutesAndLadders.Demos
{
    public class DataGeneration
    {
        public static void CreateRandom(int maxExecutionCount, string outputGameActionsFolder)
        {
            var randomStrategy = new ChutesAndLadders.Strategy.Random.Engine();
            new SimulationCollectionBuilder()
            .AddPlayer("Player 1", randomStrategy)
            .AddPlayer("Player 2", randomStrategy)
            .AddPlayer("Player 3", randomStrategy)
            .AddPlayer("Player 4", randomStrategy)
            .AddPlayer("Player 5", randomStrategy)
            .AddPlayer("Player 6", randomStrategy)
            .MaxExecutionCount(maxExecutionCount)
            .OutputResults(false)
            .OutputGameActionsFolder(outputGameActionsFolder)
            .Run();
        }
    }
}