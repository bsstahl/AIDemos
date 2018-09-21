using ChutesAndLadders.GamePlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chute.Web.Services
{
    public class GreedySimulationState
    {
        // Lets components receive change notifications
        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public async Task RunSimulation(int maxSimulations)
        {
            Console.WriteLine($"Run({maxSimulations})");
            await Task.Run(() => new SimulationCollectionBuilder()
                .AddPlayer("Player1", new ChutesAndLadders.Strategy.Greedy.Engine())
                .AddPlayer("Player2", new ChutesAndLadders.Strategy.AgressivelyBad.Engine())
                .AddPlayer("Player3", new ChutesAndLadders.Strategy.Linear.Engine())
                .MaxExecutionCount(maxSimulations)
                .OutputResults(true)
                .Run());
        }



    }
}
