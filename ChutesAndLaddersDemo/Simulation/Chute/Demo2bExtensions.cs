using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.Entities;
using ChutesAndLadders.GamePlay;

namespace Chute
{
    public static class Demo2bExtensions
    {
        public static void Demo2b_ImprovedLinear(this GameBoard board)
        {
            var linearStrategy = new ChutesAndLadders.Strategy.Linear.Engine();

            var improvedLinearStrategy1 = new ChutesAndLadders.Strategy.Rules.Engine("Improved Linear 1");
            improvedLinearStrategy1.AddTakeLadderRules(28, 84);
            improvedLinearStrategy1.AddTakeLadderRules(80, 100);

            var improvedLinearStrategy2 = new ChutesAndLadders.Strategy.Rules.Engine("Improved Linear 2");
            improvedLinearStrategy2.AddTakeLadderRules(28, 84);
            improvedLinearStrategy2.AddTakeLadderRules(80, 100);
            improvedLinearStrategy2.AddTakeChuteRules(98, 78);

            // This is the shortest path, which should give the same results
            // as "Take All Ladders" if all games started from 0
            var improvedLinearStrategy3 = new ChutesAndLadders.Strategy.Rules.Engine("Improved Linear 3");
            improvedLinearStrategy3.AddTakeLadderRules(1, 38);
            improvedLinearStrategy3.AddTakeLadderRules(51, 67);
            improvedLinearStrategy3.AddTakeLadderRules(71, 91);


            var player1 = new Player("Player 1", linearStrategy);
            var player2 = new Player("Player 2", improvedLinearStrategy1);
            var player3 = new Player("Player 3", improvedLinearStrategy2);
            var player4 = new Player("Player 4", improvedLinearStrategy3);
            var players = new Player[] { player1, player2, player3, player4 };

            var engine = new SimulationCollection();
            engine.RunSimulations(players, 30000, true);
        }
    }
}
