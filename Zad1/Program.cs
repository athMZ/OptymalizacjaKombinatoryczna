using Google.OrTools.LinearSolver;
// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace Zad1;

internal class Program
{
	static void Main()
	{
		// Set Cover Problem

		// Number of elements to cover
		int n = 10;

		// List of sets, each set contains elements that it covers
		List<HashSet<int>> s =
		[
			new() { 1, 2 },
			new() { 4, 8, 9, 10 },
			new() { 1, 3, 4, 5 },
			new() { 2, 5, 6 },
			new() { 3, 6, 7 },
			new() { 1, 2, 3, 4, 5, 6 },
			new() { 9, 10 },
			new() { 5, 8 },
			new() { 1, 3 },
			new() { 9 }
		];

		// Number of sets
		int m = s.Count;

		// Create the linear solver with the SCIP backend.
		var solver = Solver.CreateSolver("SCIP");
		if (solver is null)
			return;

		// Define the variables, x[j] is 1 if set j is included in the cover, 0 otherwise
		var x = new Variable[m];
		for (int j = 0; j < m; j++) x[j] = solver.MakeBoolVar($"x_{j}");

		Console.WriteLine("Number of variables = " + solver.NumVariables());

		// Define the constraints
		// Each element u must be covered by at least one set
		for (int u = 1; u <= n; ++u)
		{
			using Constraint constraint = solver.MakeConstraint(1, m, "");
			for (int i = 0; i < m; ++i)
			{
				if (s[i].Contains(u))
					constraint.SetCoefficient(x[i], 1);
			}
		}
		Console.WriteLine("Number of constraints = " + solver.NumConstraints());

		// Define the objective
		// Minimize the number of sets used in the cover
		var objective = solver.Objective();
		for (int j = 0; j < m; ++j)
		{
			objective.SetCoefficient(x[j], m);
		}
		objective.SetMinimization();

		// Solve the problem
		var resultStatus = solver.Solve();

		// Check that the problem has an optimal solution.
		if (resultStatus != Solver.ResultStatus.OPTIMAL)
		{
			Console.WriteLine("The problem does not have an optimal solution!");
			return;
		}

		// Output the solution
		Console.WriteLine("Solution:");
		Console.WriteLine("Optimal objective value = " + solver.Objective().Value());

		for (int j = 0; j < m; ++j)
		{
			Console.WriteLine("x[" + j + "] = " + x[j].SolutionValue());
		}

	}
}
