# Pete's Pottery Paradise - Linear Programming Example

This console app demonstrates a small business production optimization model using Google OR-Tools.

Pete's Pottery Paradise makes small and large vases. The model decides how many of each product to make so Pete can maximize revenue while staying within limited clay and glaze inventory.

Run the Linear Programming example:

```powershell
dotnet run --project .\LinearProgramming\LinearProgramming.csproj
```

After running the app, look at `SolveProductionModel()` in `Program.cs` to see how OR-Tools is used to solve the problem. That method creates the GLOP linear solver, defines the production decision variables, adds the clay and glaze inventory constraints, sets the revenue-maximizing objective, and calls `Solve()` to find the optimal production plan.

To experiment with different inventory levels, update the `ClayAvailable` and `GlazeAvailable` constants at the top of `Program.cs`. Increasing those values gives Pete more material to work with, which can change the feasible production plans and may produce a different optimal mix of small and large vases.

Additional examples of optimization, albeit using older tools and code written up to 10 years ago, can be found in the [Optimization repo](https://github.com/bsstahl/Optimization).
