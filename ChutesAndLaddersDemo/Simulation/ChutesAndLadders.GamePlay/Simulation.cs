using ChutesAndLadders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.GamePlay
{
    /// <summary>
    /// Runs multiple games to determine which strategy & player-position 
    /// combination results in the most wins for that player.
    /// </summary>
    public class Simulation
    {

        int _maxStartingLocation;
        GameBoard _board;


        public Simulation(int maxStartingLocation) : this(new GameBoard(), maxStartingLocation) { }

        internal Simulation(GameBoard board, int maxStartingLocation)
        {
            _maxStartingLocation = maxStartingLocation;
            _board = board;
        }

        public SimulationResults Run(Player[] players, int executionCount)
        {
            int tryCount = 0;
            var gameActions = new List<GameAction>();
            while (tryCount < executionCount)
            {
                int startAt = (new Random()).Next(_maxStartingLocation + 1);
                var results = new Game(_board).Play(players, tryCount, startAt);

                tryCount++;
                results.Winner.WinCount++;
                results.Winner.Strategy.WinCount++;
                gameActions.AddRange(results.GameActions);
            }

            return new SimulationResults()
            {
                Players = players,
                GameActions = gameActions
            };
        }


    }
}
