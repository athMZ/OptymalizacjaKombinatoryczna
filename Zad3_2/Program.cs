// Zadanie 3, wariant 2
// Mikołaj Zuziak

using System.Diagnostics;

namespace Zad3_2;

internal class Program
{
	// The basic recursive solution to the problem
	private static long MaxScoreRecursive(int[] arr, int start, int end)
	{
		// Failsafe condition
		if (start > end)
			return 0;

		// Sum of all elements in the current range [start, end]
		long totalSum = 0;
		for (var i = start; i <= end; i++)
			totalSum += arr[i];

		// We either pick the first or the last element of the current range [start, end]
		// The opponent will then pick the best option for them, so we subtract their score from the total sum
		long chooseStart = totalSum - MaxScoreRecursive(arr, start + 1, end);
		long chooseEnd = totalSum - MaxScoreRecursive(arr, start, end - 1);

		// We return the maximum score we can achieve by choosing either the first or the last element
		return Math.Max(chooseStart, chooseEnd);
	}

	// The memoized version of the recursive solution - improves performance by storing previously calculated results
	private static long MaxScoreWithMemo(int[] arr, int start, int end, long[,] memo)
	{
		// Failsafe condition
		if (start > end)
			return 0;

		// Check if we have already calculated the score for this range [start, end]
		// If we have, return the stored value - this avoids recalculating the same score multiple times
		if (memo[start, end] != -1)
			return memo[start, end];

		// Sum of all elements in the current range [start, end]
		long totalSum = 0;
		for (int i = start; i <= end; i++)
			totalSum += arr[i];

		// We either pick the first or the last element of the current range [start, end]
		// The opponent will then pick the best option for them, so we subtract their score from the total sum
		long chooseStart = totalSum - MaxScoreWithMemo(arr, start + 1, end, memo);
		long chooseEnd = totalSum - MaxScoreWithMemo(arr, start, end - 1, memo);

		// Store the result in the memoization table to avoid recalculating it in the future
		memo[start, end] = Math.Max(chooseStart, chooseEnd);

		// We return the maximum score we can achieve by choosing either the first or the last element
		return memo[start, end];
	}

	private static void Main(string[] args)
	{
		var stopwatch = new Stopwatch();
		stopwatch.Start();

		//The first argument is the text file with the sequence of numbers
		//The second argument is -m flag to use memo or not

		if (string.IsNullOrEmpty(args[0]))
		{
			Console.WriteLine("Please provide the input file path.");
			return;
		}

		var inputFilePath = args[0];

		var useMemo = args.Length > 1 && args[1] == "-m";

		// Read the sequence of numbers from the input file
		// Numbers are separated by spaces
		// For example: 4 5 1 3

		if (!Path.Exists(inputFilePath))
		{
			Console.WriteLine($"The file {inputFilePath} does not exist.");
			return;
		}

		var file = new StreamReader(inputFilePath);
		var line = file.ReadLine();
		file.Close();

		var sequence = line?
			.Split(' ')
			.Select(x => Convert.ToInt32(x))
			.ToArray();

		var n = sequence!.Length;

		if (n <= 0)
		{
			Console.WriteLine("The input file is empty or contains invalid data.");
			return;
		}

		if (useMemo)
		{
			// Initialize memoization table with -1 (indicating not calculated yet)
			long[,] memo = new long[n, n];
			for (int i = 0; i < n; i++)
				for (int j = 0; j < n; j++)
					memo[i, j] = -1;

			long resultMemo = MaxScoreWithMemo(sequence, 0, n - 1, memo);
			Console.WriteLine($"Maximum points for first player (with memo): {resultMemo}");

			stopwatch.Stop();
			Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");

			return;
		}

		long resultRecursive = MaxScoreRecursive(sequence, 0, n - 1);
		Console.WriteLine($"Maximum points for first player (recursive): {resultRecursive}");

		stopwatch.Stop();
		Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
	}
}