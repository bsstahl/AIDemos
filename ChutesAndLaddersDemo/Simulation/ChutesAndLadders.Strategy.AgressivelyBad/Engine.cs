using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.Entities;

namespace ChutesAndLadders.Strategy.AgressivelyBad
{
    // WARNING: This strategy will never complete a game
    // If it is the only strategy employed in the game, the game will never end
    // It should be used as an additional (losing) player only
    public class Engine : Interfaces.IGameStrategy
    {
        public string Name { get => "Aggressively Bad"; }

        public int WinCount { get; set; }


        public int GetMove(GameSituation situation)
        {
            return situation.LegalMoves.Min();
        }
    }
}
