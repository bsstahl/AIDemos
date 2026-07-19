using Google.OrTools.LinearSolver;
using LinearProgramming.Messages;

namespace LinearProgramming.Extensions;

public static class OptimizationResultExtensions
{
    public static void PrintResult(this OptimizationResult result, double clayAvailable, double glazeAvailable)
    {
        Console.WriteLine("Pete's Pottery Paradise - Linear Programming Example");
        Console.WriteLine("LP model: variables are continuous, constraints are linear, and the objective is linear.");

        Console.WriteLine();

        if (result.Status != Solver.ResultStatus.OPTIMAL)
        {
            Console.WriteLine($"Solver status: {result.Status}");
            Console.WriteLine("No optimal production plan was found.");
            return;
        }

        Console.WriteLine("Decision Variables:");
        Console.WriteLine($" small_vases = {FormatNumber(result.SmallVases)}");
        Console.WriteLine($" large_vases = {FormatNumber(result.LargeVases)}");
        Console.WriteLine();

        Console.WriteLine("Objective:");
        Console.WriteLine($" Maximum revenue = ${FormatNumber(result.MaximumRevenue)}");
        Console.WriteLine();

        Console.WriteLine("Resource Usage:");
        Console.WriteLine($" Clay used = {FormatNumber(result.ClayUsed)} / {FormatNumber(clayAvailable)} oz");
        Console.WriteLine($" Glaze used = {FormatNumber(result.GlazeUsed)} / {FormatNumber(glazeAvailable)} oz");
        Console.WriteLine();

        Console.WriteLine("Constraint Status:");
        Console.WriteLine($" Clay constraint is {FormatBinding(result.ClayUsed, clayAvailable)}.");
        Console.WriteLine($" Glaze constraint is {FormatBinding(result.GlazeUsed, glazeAvailable)}.");
        Console.WriteLine();

        Console.WriteLine($"Solver status: {result.Status}");
        Console.WriteLine("Solver used: GLOP");
    }

    private const double BindingTolerance = 1e-7;

    private static string FormatBinding(double used, double available) =>
        Math.Abs(used - available) <= BindingTolerance ? "binding" : "not binding";

    private static string FormatNumber(double value) =>
        Math.Abs(value - Math.Round(value)) <= BindingTolerance
            ? Math.Round(value).ToString("0")
            : value.ToString("0.###");

}
