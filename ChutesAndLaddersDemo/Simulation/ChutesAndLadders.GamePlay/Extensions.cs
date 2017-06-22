using ChutesAndLadders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.GamePlay
{
    public static class Extensions
    {
        public static bool DecisionNeeded(this GameBoard board, GameSituation situation)
        {
            var legalEndpoints = board.GetLegalEndpoints(situation.BoardLocation, situation.Spin);
            bool win = legalEndpoints.ResultsInWin();
            return (!win && legalEndpoints.Count() > 1);
        }

        public static bool HasWon(this Player player)
        {
            return (new int[] { player.BoardLocation }).ResultsInWin();
        }

        public static bool ResultsInWin(this IEnumerable<int> legalEndpoints)
        {
            return legalEndpoints.Contains(100);
        }

        public static Player[] Rotate(this Player[] players)
        {
            var copy = new List<Player>();
            for (int i = 1; i < players.Length; i++)
                copy.Add(players[i]);
            copy.Add(players[0]);
            return copy.ToArray();
        }

        public static IEnumerable<Player> DeepCopy(this IEnumerable<Player> players)
        {
            return players.Select(p => new Player(p.Id, p.Name, p.Strategy));
        }
    }
}
