using Google.OrTools.LinearSolver;

namespace LinearProgramming.Messages;

public sealed record OptimizationResult(
    Solver.ResultStatus Status,
    double SmallVases,
    double LargeVases,
    double MaximumRevenue,
    double ClayUsed,
    double GlazeUsed);
