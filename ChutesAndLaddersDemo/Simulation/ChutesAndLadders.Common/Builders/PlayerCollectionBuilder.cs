using System;
using System.Collections.Generic;
using System.Text;
using ChutesAndLadders.Entities;
using ChutesAndLadders.Interfaces;

namespace ChutesAndLadders.Builders
{
    public class PlayerCollectionBuilder
    {
        private List<Player> _players = new List<Player>();

        public Player[] Build()
        {
            return _players.ToArray();
        }

        public PlayerCollectionBuilder Add(string playerName, IGameStrategy strategy)
        {
            _players.Add(new Player(playerName, strategy));
            return this;
        }
    }
}
