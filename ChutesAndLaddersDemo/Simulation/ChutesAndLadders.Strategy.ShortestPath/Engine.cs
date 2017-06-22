using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.Entities;

namespace ChutesAndLadders.Strategy.ShortestPath
{
    public class Engine : Interfaces.IGameStrategy
    {
        PathCollection _paths = new PathCollection();

        public string Name => "Shortest Path";

        public int WinCount { get; set; }


        public int GetMove(GameSituation situation)
        {
            var bestOption = _paths.FindClosestToEnd(situation.LegalMoves);
            //var bestOptionDistance = _paths[bestOption];
            //var maxOptionDistance = _paths[situation.LegalMoves.Max()];
            //if (bestOptionDistance < maxOptionDistance)
            //    Console.WriteLine($"*** Greedy option not the best for {situation.BoardLocation}-{situation.Spin}");
            return bestOption;
        }

        public override string ToString()
        {
            return _paths.ToString();
        }
    }
}
