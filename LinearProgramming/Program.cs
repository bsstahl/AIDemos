using Google.OrTools.LinearSolver;
using LinearProgramming.Extensions;
using LinearProgramming.Messages;

namespace LinearProgramming;

internal static class Program
{
    private const double ClayAvailable = 24.0;
    private const double GlazeAvailable = 16.0;

    private static int Main(string[] args)
    {
        OptimizationResult result = SolveProductionModel();
        result.PrintResult(ClayAvailable, GlazeAvailable);
        return result.Status == Solver.ResultStatus.OPTIMAL ? 0 : 2;
    }

    private static OptimizationResult SolveProductionModel()
    {
        // Initialize OR-Tools by creating the GLOP Linear Programming solver.
        Solver solver = Solver.CreateSolver("GLOP")
            ?? throw new InvalidOperationException("The GLOP solver is not available.");

        // Decision variables:
        // small_vases represents the number of small vases to produce.
        // large_vases represents the number of large vases to produce.
        // Linear Programming uses continuous variables, so fractional quantities are allowed.
        Variable smallVases = solver.MakeNumVar(0.0, double.PositiveInfinity, "small_vases");
        Variable largeVases = solver.MakeNumVar(0.0, double.PositiveInfinity, "large_vases");

        // Resource constraints:
        // Clay:  1S + 4L <= 24
        // Glaze: 1S + 2L <= 16
        solver.Add(smallVases + 4 * largeVases <= ClayAvailable);
        solver.Add(smallVases + 2 * largeVases <= GlazeAvailable);

        // Objective function:
        // Maximize revenue = 3S + 9L.
        solver.Maximize(3 * smallVases + 9 * largeVases);

        Solver.ResultStatus status = solver.Solve();

        double smallVaseCount = smallVases.SolutionValue();
        double largeVaseCount = largeVases.SolutionValue();
        double clayUsed = smallVaseCount + 4 * largeVaseCount;
        double glazeUsed = smallVaseCount + 2 * largeVaseCount;

        return new OptimizationResult(
            Status: status,
            SmallVases: smallVaseCount,
            LargeVases: largeVaseCount,
            MaximumRevenue: solver.Objective().Value(),
            ClayUsed: clayUsed,
            GlazeUsed: glazeUsed);
    }
}