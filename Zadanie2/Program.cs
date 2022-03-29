using Zadanie2.Components;
using Zadanie2.Loaders;
using Zadanie2.CSP;

void TestBinary(int w, int h)
{
    Console.WriteLine($"Binary test for {w}x{h}");
    BinaryLoader binaryLoader = new BinaryLoader(w, h);
    Variable<short?>[,] data = binaryLoader.LoadData($"dane/binary_{w}x{h}");
    ICSP<short?> csp = new CSPBacktracking<short?>(data, new List<IConstraint>() { new BinaryConstraint(data) });
    bool result = csp.FindSolution();
    if (result)
    {
        Console.WriteLine($"Found {csp.Solutions.Count} solutions");
        foreach (var solution in csp.Solutions)
        {
            for (int i = 0; i < solution.GetLength(0); i++)
            {
                for (int j = 0; j < solution.GetLength(1); j++)
                {
                    Console.Write($"{(solution[i, j].Value.HasValue ? solution[i, j].Value.ToString() : "x")}|");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
    else
        Console.WriteLine("No result");
    Console.WriteLine($"Iterations: {csp.Iterations}");
}

void TestFutoshiki(int n)
{
    Console.WriteLine($"Futoshiki test for {n}x{n}");
    FutoshikiLoader futoshikiLoader = new FutoshikiLoader(n);
    (Variable<int?>[,] data, List<IConstraint> constraints) = futoshikiLoader.LoadData($"dane/futoshiki_{n}x{n}");
    List<IConstraint> cspConstraints = new List<IConstraint>() { new FutoshikiConstraint(data) };
    cspConstraints.AddRange(constraints);
    ICSP<int?> csp = new CSPBacktracking<int?>(data, cspConstraints);
    bool result = csp.FindSolution();
    if (result)
    {
        Console.WriteLine($"Found {csp.Solutions.Count} solutions");
        foreach (var solution in csp.Solutions)
        {
            for (int i = 0; i < solution.GetLength(0); i++)
            {
                for (int j = 0; j < solution.GetLength(1); j++)
                {
                    Console.Write($"{(solution[i, j].Value.HasValue ? solution[i, j].Value.ToString() : "x")}|");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
    else
        Console.WriteLine("No result");
    Console.WriteLine($"Iterations: {csp.Iterations}");
}

TestBinary(6, 6);
TestBinary(8, 8);
TestBinary(10, 10);
TestFutoshiki(4);
TestFutoshiki(5);
TestFutoshiki(6);