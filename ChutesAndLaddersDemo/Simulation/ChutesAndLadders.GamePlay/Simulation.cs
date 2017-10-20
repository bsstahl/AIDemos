using ChutesAndLadders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.GamePlay
{
    /// <summary>
    /// Runs multiple games to determine which strategy and player-position 
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

        public SimulationResults Run(Player[] players, int executionCount, string gameActionFilePath = null)
        {
            int tryCount = 0;
            while (tryCount < executionCount)
            {
                int startAt = (new Random()).Next(_maxStartingLocation + 1);
                var results = new Game(_board).Play(players, tryCount, startAt);

                tryCount++;
                results.Winner.WinCount++;
                results.Winner.Strategy.WinCount++;

                // Write-out GameActions to the appropriate file
                if (!string.IsNullOrWhiteSpace(gameActionFilePath))
                    System.IO.File.AppendAllText(gameActionFilePath, results.GameActions.Output());
            }

            return new SimulationResults()
            {
                Players = players
            };
        }


    }
}
