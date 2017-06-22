using ChutesAndLadders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.Linear
{
    public class Engine : Interfaces.IGameStrategy
    {
        public string Name { get => "Linear"; }

        public int WinCount { get; set; }


        public int GetMove(GameSituation situation)
        {
            if (!situation.LegalMoves.Contains(situation.BoardLocation + situation.Spin))
                throw new InvalidOperationException();
            return situation.BoardLocation + situation.Spin;
        }
    }
}
