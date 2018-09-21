using Microsoft.AspNetCore.Blazor;
using System;
using System.Threading.Tasks;

namespace Chute.Web.Services
{
    public class GeneticSimulationState
    {
        public int EvolutionCount { get; set; }
        public int GenerationCount { get; set; }
        public int MostWins { get; set; }


        // Lets components receive change notifications
        public event Action OnChange;

        public GeneticSimulationState()
        {
            this.EvolutionCount = 0;
            this.GenerationCount = 0;
            this.MostWins = 0;
        }

        public async Task RunSimulation(int maxGenerations, int simulationsPerGeneration, double misspellingRate)
        {
            Console.WriteLine($"Run({maxGenerations},{simulationsPerGeneration},{misspellingRate})");
            await ChutesAndLadders.Demos.Genetics.EvolutionAsync(maxGenerations, simulationsPerGeneration, misspellingRate, null);
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}