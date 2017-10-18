using ChutesAndLadders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.Random
{
    public class Engine : Interfaces.IGameStrategy
    {
        public string Name { get => "Random"; }

        public int WinCount { get; set; }


        public int GetMove(GameSituation situation)
        {
            return situation.LegalMoves.GetRandom();
        }
    }
}
