using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.Entities;
using ChutesAndLadders.GamePlay;

namespace Chute
{
    public static class DataGenerationExtensions
    {
        public static void GameActionOutput_SmallSample(this GameBoard board)
        {
            var randomStrategy = new ChutesAndLadders.Strategy.Random.Engine();

            var player1 = new Player("Player 1", randomStrategy);
            var player2 = new Player("Player 2", randomStrategy);
            var player3 = new Player("Player 3", randomStrategy);
            var player4 = new Player("Player 4", randomStrategy);
            var player5 = new Player("Player 5", randomStrategy);
            var player6 = new Player("Player 6", randomStrategy);
            var players = new Player[] { player1, player2, player3, player4, player5, player6 };

            var engine = new SimulationCollection();
            var results = engine.RunSimulations(players, 60000, false, @".\");
        }

    }
}
