using ChutesAndLadders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chute
{
    public static class Demo3aExtensions
    {
        public static void Demo3a_GeneticAnalysis(this GameBoard board)
        {
            int maxOptions = 0;
            int totalConditions = 0;
            int totalConditionalOptions = 0;

            for (int startingPoint = 0; startingPoint < 100; startingPoint++)
            {
                for (byte spin = 1; spin < 7; spin++)
                {
                    var endPoints = board.GetLegalEndpoints(startingPoint, spin);
                    int currentOptions = endPoints.Count();
                    if ((currentOptions > 1) && (!endPoints.Contains(100)))
                    {
                        totalConditions++;
                        totalConditionalOptions += currentOptions;
                        if (currentOptions > maxOptions)
                            maxOptions = currentOptions;
                        Console.WriteLine($"{startingPoint},{spin},{currentOptions}");
                    }
                }
            }

            Console.WriteLine($"Max Options: {maxOptions}");
            Console.WriteLine($"Total Conditions: {totalConditions}");
            Console.WriteLine($"Total Conditional Options: {totalConditionalOptions}");
        }

    }
}
