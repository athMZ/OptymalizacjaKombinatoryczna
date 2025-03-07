//Zadanie 1, wariant 8 - Sudoku
//Note from the author: I code in english.

using Google.OrTools.LinearSolver;
using Solver = Google.OrTools.LinearSolver.Solver;

namespace Zad1_8;

public class Program
{
	/*
	 * This program solves a sudoku puzzle using the Google OR-Tools library.
	 * To load in a puzzle, provide a text file with the puzzle as an argument.
	 * We assume that the puzzle is a 9x9 grid, with empty cells represented by 0.
	 *
	 * I have provided 3 input files for testing:
	 * - input.txt - data from class pdf
	 * - input1.txt - data from online sudoku solver, level hard, has solution
	 * - input2.txt - a puzzle with no solution
	 */
	private static void Main(string[] args)
	{
		if (args.Length == 0)
		{
			Console.WriteLine("This program solves a sudoku puzzle using the Google OR-Tools library.\nTo load in a puzzle, provide a text file with the puzzle as an argument.\nWe assume that the puzzle is a 9x9 grid, with empty cells represented by 0.");
			return;
		}

		var filePath = args[0];
		var inputMatrix = ReadFileIntoMatrix(filePath);

		Console.WriteLine("Input Data:");
		DisplayAsSudoku(inputMatrix);

		var solver = Solver.CreateSolver("SCIP");

		/*
		* Create variables

		* I have decided to use boolean variables to treat the sudoku as something like this:
		* Is the cell [2,3] a 5? Yes/No

		* I use 3 indexes to represent position and value.
		*/
		var sudokuVars = new Variable[9, 9, 9];
		for (var i = 0; i < 9; i++) //Position in grid
		{
			for (var j = 0; j < 9; j++) //Position in grid
			{
				for (var k = 0; k < 9; k++) //Values
				{
					sudokuVars[i, j, k] = solver.MakeBoolVar($"x_{i}_{j}_{k}");
				}
			}
		}

		/*
		* Each cell must have exactly one number
		* This part essentially creates the constraints as equations similar to that:
		* x[2,3,0] + x[2,3,1] + x[2,3,2] + ... + x[2,3,8] = 1

		* Since these are binary variables (0 or 1), this constraint ensures that exactly one of these variables equals 1, meaning the cell contains exactly one value.
		*/
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < 9; j++)
			{
				var constraint = solver.MakeConstraint(1, 1); //The equation must equal 1
				for (var k = 0; k < 9; k++)
				{
					constraint.SetCoefficient(sudokuVars[i, j, k], 1); //We multiply each variable by, i.e.: 1*x[2,3,0] - That means that we include that variable in the equation of the constraint
				}
			}
		}

		/*
		 * Each row contains each value exactly once
		 * The idea is the same as for the previous constraint, but this time we change the order of the indexes to fit rows.
		 */
		for (var i = 0; i < 9; i++)
		{
			for (var k = 0; k < 9; k++)
			{
				var constraint = solver.MakeConstraint(1, 1);
				for (var j = 0; j < 9; j++)
				{
					constraint.SetCoefficient(sudokuVars[i, j, k], 1);
				}
			}
		}

		/*
		 * Each column contains each value exactly once
		 * The idea is the same as for the previous constraint, but this time we change the order of the indexes to fit rows.
		 */
		for (var j = 0; j < 9; j++)
		{
			for (var k = 0; k < 9; k++)
			{
				var constraint = solver.MakeConstraint(1, 1);
				for (var i = 0; i < 9; i++)
				{
					constraint.SetCoefficient(sudokuVars[i, j, k], 1);
				}
			}
		}

		/*
		 * Each 3x3 box contains each value exactly once
		 * We also want to create an equation similar to the previous ones, but this time we need to iterate over the 3x3 boxes.
		 */
		for (var boxRow = 0; boxRow < 3; boxRow++)
		{
			for (var boxCol = 0; boxCol < 3; boxCol++)
			{
				for (var k = 0; k < 9; k++)
				{
					var constraint = solver.MakeConstraint(1, 1);
					for (var i = 3 * boxRow; i < 3 * (boxRow + 1); i++)
					{
						for (var j = 3 * boxCol; j < 3 * (boxCol + 1); j++)
						{
							constraint.SetCoefficient(sudokuVars[i, j, k], 1);
						}
					}
				}
			}
		}

		/*
		 * Add constraints for the initial values
		 * We need to add constraints for the initial values in the puzzle.
		 * We need to ensure that the variables corresponding to the initial values are set to 1.
		 */
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < 9; j++)
			{
				if (inputMatrix[i, j] <= 0) continue;

				// Values in the puzzle are 1-9, but our k index is 0-8
				var constraint = solver.MakeConstraint(1, 1);
				constraint.SetCoefficient(sudokuVars[i, j, inputMatrix[i, j] - 1], 1);
			}
		}

		// Solve the system
		var resultStatus = solver.Solve();

		// Check if the problem has a feasible solution
		if (resultStatus != Solver.ResultStatus.OPTIMAL)
		{
			Console.WriteLine("The problem does not have an optimal solution!");
			return;
		}

		var solution = GetSolutionsAsSimpleArrayFromVariables(sudokuVars);

		Console.WriteLine("\nSolution:");
		DisplayAsSudoku(solution);
	}

	private static int[,] ReadFileIntoMatrix(string filePath)
	{
		if (!File.Exists(filePath))
		{
			Console.WriteLine("The file does not exist.");
			Environment.Exit(1);
		}

		Console.WriteLine($"Input file: {filePath}");

		using var file = new StreamReader(filePath);
		var matrix = new int[9, 9];

		for (var i = 0; i < 9; i++)
		{
			var line = file.ReadLine();

			for (var j = 0; j < 9; j++) matrix[i, j] = line[j] - '0';
		}

		return matrix;
	}

	private static void DisplayAsSudoku(int[,] matrix)
	{
		for (var i = 0; i < 9; i++)
		{
			if (i % 3 == 0 && i > 0)
			{
				Console.WriteLine("------+-------+------");
			}
			for (var j = 0; j < 9; j++)
			{
				if (j % 3 == 0 && j > 0)
				{
					Console.Write("| ");
				}
				Console.Write(matrix[i, j] != 0 ? matrix[i, j].ToString() : " ");
				Console.Write(" ");
			}
			Console.WriteLine();
		}
	}

	private static int[,] GetSolutionsAsSimpleArrayFromVariables(Variable[,,] variables)
	{
		var solution = new int[9, 9];
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < 9; j++)
			{
				for (var k = 0; k < 9; k++)
				{
					if (!(variables[i, j, k].SolutionValue() > 0.5)) continue;

					// k is 0-indexed, but values are 1-9
					solution[i, j] = k + 1;
					break;
				}
			}
		}

		return solution;
	}
}
