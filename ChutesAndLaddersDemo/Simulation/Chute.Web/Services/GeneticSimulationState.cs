using System;
using System.Threading.Tasks;

namespace Chute.Web.Services
{
    internal static class GeneticSimulationState
    {
        public static async Task RunSimulation(int maxGenerations, int simulationsPerGeneration, double misspellingRate)
        {
            Console.WriteLine($"Run({maxGenerations},{simulationsPerGeneration},{misspellingRate})");
            await ChutesAndLadders.Demos.Genetics.EvolutionAsync(maxGenerations, simulationsPerGeneration, misspellingRate, null).ConfigureAwait(false);
        }
    }
}