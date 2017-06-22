using ChutesAndLadders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Interfaces
{
    public interface IGameStrategy
    {
        string Name { get; }
        int WinCount { get; set; }

        int GetMove(GameSituation situation);
    }
}
