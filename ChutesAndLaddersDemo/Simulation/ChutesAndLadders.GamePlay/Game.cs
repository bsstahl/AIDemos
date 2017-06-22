using ChutesAndLadders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.GamePlay
{
    public class Game
    {
        GameBoard _board;

        public Game(GameBoard board)
        {
            _board = board;
        }

        public GameResults Play(IEnumerable<Player> players, int gameId, int startingPoint, bool outputResults = false, byte constantSpinValue = 0)
        {
            bool gameOver = false;
            int turns = 0;

            var spinner = new Spinner(6, constantSpinValue);

            // Setup the players on the board
            foreach (var player in players)
                player.BoardLocation = startingPoint;

            // Start the game loop
            while (!gameOver)
            {
                foreach (var player in players)
                    if (!gameOver)
                    {
                        var startingLocation = player.BoardLocation;
                        var spin = spinner.Spin();
                        int result = 0;

                        var legalEndpoints = _board.GetLegalEndpoints(player.BoardLocation, spin);
                        var situation = new GameSituation()
                        {
                            BoardLocation = player.BoardLocation,
                            Spin = spin,
                            LegalMoves = legalEndpoints,
                            PlayerLocations = players.Select(p => p.BoardLocation)
                        };

                        if (_board.DecisionNeeded(situation))
                        {
                            result = player.Strategy.GetMove(situation);
                            if (!legalEndpoints.Contains(result))
                                throw new InvalidOperationException();
                            player.BoardLocation = result;
                        }
                        else if (legalEndpoints.ResultsInWin())
                        {
                            player.BoardLocation = 100;
                        }
                        else if (legalEndpoints.Any())
                        {
                            result = legalEndpoints.Single();
                            player.BoardLocation = result;
                        }

                        if (outputResults)
                            Console.WriteLine($"{player.Name} ({player.Strategy.Name}) spins {spin} and moves from {startingLocation} to {player.BoardLocation}");

                        gameOver = player.HasWon();
                    }

                turns++;
            }

            return new GameResults()
            {
                GameId = gameId,
                Winner = players.Single(p => p.BoardLocation == 100),
                Players = players,
                Turns = turns
            };
        }


    }
}
