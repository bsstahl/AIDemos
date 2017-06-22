using ChutesAndLadders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Entities
{
    public class Player
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public int BoardLocation { get; set; }
        public IGameStrategy Strategy { get; set; }

        public int WinCount { get; set; }

        public Player(string name, IGameStrategy strategy)
            :this(Guid.NewGuid(), name, strategy) { }

        public Player(Guid id, string name, IGameStrategy strategy)
        {
            this.Id = id;
            this.Name = name;
            this.Strategy = strategy;
            this.WinCount = 0;
            this.BoardLocation = 0;
        }
    }
}
