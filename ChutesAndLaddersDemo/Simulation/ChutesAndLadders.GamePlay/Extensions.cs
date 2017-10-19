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

        public static int IndexOf(this IEnumerable<Player> players, Player player)
        {
            if (players == null)
                throw new ArgumentNullException(nameof(players));

            if (players.Count() < 1)
                throw new ArgumentException($"{nameof(players)} cannot be empty");

            bool found = false;
            var playerArray = players.ToArray();
            int index = 0;
            while (!found && (index < players.Count()))
            {
                if (playerArray[index].Id == player.Id)
                    found = true;
                else
                    index++;
            }

            if (!found)
                throw new InvalidOperationException("Player not found");

            return index;
        }

        public static IEnumerable<int> Clone(this IEnumerable<int> data)
        {
            return data.ToList().Select(d => d);
        }

        public static IEnumerable<Player> DeepCopy(this IEnumerable<Player> players)
        {
            return players.Select(p => new Player(p.Id, p.Name, p.Strategy));
        }

        public static string Output(this GameAction gameAction)
        {
            // UniqueGameId,PlayerNumber,BoardLocation,Spin,Player1Loc,Player2Loc,Player3Loc,Player4Loc,Player5Loc,Player6Loc,SelectedMove,PlayerWonGame
            var sb = new StringBuilder();

            sb.Append($"{gameAction.UniqueGameId.ToString()},{gameAction.PlayerNumber},{gameAction.BoardLocation},{gameAction.Spin},");
            foreach (var playerLocation in gameAction.PlayerLocations)
                sb.Append($"{playerLocation},");
            sb.AppendLine($"{gameAction.SelectedMove},{Convert.ToByte(gameAction.PlayerWonGame):0}");

            return sb.ToString();
        }

        public static string Output(this IEnumerable<GameAction> gameActions)
        {
            var sb = new StringBuilder();
            foreach (var gameAction in gameActions)
                sb.Append(gameAction.Output());
            return sb.ToString();
        }
    }
}
