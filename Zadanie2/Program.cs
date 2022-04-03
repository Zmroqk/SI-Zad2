using Zadanie2.Constraints;
using Zadanie2.Loaders;
using Zadanie2.CSP;
using Zadanie2.Heuristics;
using Zadanie2;
using Zadanie2.ForwardChecking;

void PrintResults<T>(ICSP<T> csp)
{
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
                    Console.Write($"{(solution[i, j].Value != null ? solution[i, j].Value.ToString() : "x")}|");
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

void PrintResultsTree<T>(int size, ICSPTree<T> csp)
{
    bool result = csp.FindSolution();
    if (result)
    {
        Console.WriteLine($"Found {csp.Solutions.Count} solutions");
        foreach (var solution in csp.Solutions)
        {
            for (int i = 0; i < solution.Count; i++)
            {
                Console.Write($"{(solution[i].Value != null ? solution[i].Value.ToString() : "x")}|");
                if((i + 1) % size == 0)
                    Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
    else
        Console.WriteLine("No result");
    Console.WriteLine($"Iterations: {csp.Iterations}");
}

void TestBinary(int w, int h)
{
    Console.WriteLine($"Binary test for {w}x{h}");
    BinaryLoader binaryLoader = new BinaryLoader(w, h);
    Variable<short?>[,] data = binaryLoader.LoadData($"dane/binary_{w}x{h}");
    ICSP<short?> csp = new CSPBacktracking<short?>(data, 
        new List<IConstraint>() { new BinaryConstraint(data) },
        (domain) => { return new SimpleHeuristic<short?>(domain); },
        new List<Func<Variable<short?>[,], int, int, IConstraint>>()
        );
    PrintResults<short?>(csp);
}

void TestNewBinary(int w, int h)
{
    Console.WriteLine($"Binary new test for {w}x{h}");
    BinaryLoaderTree binaryLoader = new BinaryLoaderTree(w, h);
    List<Variable<short?>> data = binaryLoader.LoadData($"dane/binary_{w}x{h}");
    ICSPTree<short?> csp = new BacktrackingTreeCSP<short?>(
        data,
        new List<IConstraint>() { new BinaryConstraintTree(w, h, data) },
        (domain) => { return new SimpleHeuristic<short?>(domain); },
        (variables) => { return new SimpleChildHeuristic<short?>(w, variables); },
        new List<Func<List<Variable<short?>>, Variable<short?>, IConstraint>>()
        );
    PrintResultsTree<short?>(w, csp);
}

void TestNewRandomChildBinary(int w, int h)
{
    Console.WriteLine($"Binary new random child test for {w}x{h}");
    BinaryLoaderTree binaryLoader = new BinaryLoaderTree(w, h);
    List<Variable<short?>> data = binaryLoader.LoadData($"dane/binary_{w}x{h}");
    ICSPTree<short?> csp = new BacktrackingTreeCSP<short?>(
        data,
        new List<IConstraint>() { new BinaryConstraintTree(w, h, data) },
        (domain) => { return new SimpleHeuristic<short?>(domain); },
        (variables) => { return new RandomChildHeuristic<short?>(variables); },
        new List<Func<List<Variable<short?>>, Variable<short?>, IConstraint>>()
        );
    PrintResultsTree<short?>(w, csp);
}

void TestBinaryRandom(int w, int h)
{
    Console.WriteLine($"Binary random test for {w}x{h}");
    BinaryLoader binaryLoader = new BinaryLoader(w, h);
    Variable<short?>[,] data = binaryLoader.LoadData($"dane/binary_{w}x{h}");
    ICSP<short?> csp = new CSPBacktracking<short?>(data,
        new List<IConstraint>() { new BinaryConstraint(data) },
        (domain) => { return new RandomHeuristic<short?>(domain); },
        new List<Func<Variable<short?>[,], int, int, IConstraint>>()
        );
    PrintResults<short?>(csp);
}

void TestForwardBinary(int w, int h)
{
    Console.WriteLine($"Binary forawrd test for {w}x{h}");
    BinaryLoader binaryLoader = new BinaryLoader(w, h);
    Variable<short?>[,] data = binaryLoader.LoadData($"dane/binary_{w}x{h}");
    ICSP<short?> csp = new ForwardCheckingCSP<short?>(data,
        new List<IConstraint>() { new BinaryConstraint(data) },
        (domain) => { return new SimpleHeuristic<short?>(domain); },
        (i, j, val) => { return new BinaryForwardCheck(data, i, j, val); },
        new List<Func<Variable<short?>[,], int, int, IConstraint>>()
        );
    PrintResults<short?>(csp);
}

void TestFutoshiki(int n)
{
    Console.WriteLine($"Futoshiki test for {n}x{n}");
    FutoshikiLoader futoshikiLoader = new FutoshikiLoader(n);
    (Variable<int?>[,] data, List<InequalityConstraint> constraints) = futoshikiLoader.LoadData($"dane/futoshiki_{n}x{n}");
    List<Func<Variable<int?>[,], int, int, IConstraint>> cspFactoriesConstraints = 
        new List<Func<Variable<int?>[,], int, int, IConstraint>>() 
        { (var, i, j) => new FutoshikiConstraint(var, i, j) };
    List<IConstraint> cspConstraints = new List<IConstraint>();
    cspConstraints.AddRange(constraints);
    ICSP<int?> csp = new CSPBacktracking<int?>(
        data, 
        cspConstraints,
        (domain) => { return new SimpleHeuristic<int?>(domain); },
        cspFactoriesConstraints);
    PrintResults<int?>(csp);
}

void TestNewFutoshiki(int n)
{
    Console.WriteLine($"Futoshiki new test for {n}x{n}");
    FutoshikiLoaderTree futoshikiLoader = new FutoshikiLoaderTree(n);
    (List<Variable<int?>> data, List<InequalityConstraint> constraints) = futoshikiLoader.LoadData($"dane/futoshiki_{n}x{n}");
    List<Func<List<Variable<int?>>, Variable<int?>, IConstraint>> cspFactoriesConstraints =
        new List<Func<List<Variable<int?>>, Variable<int?>, IConstraint>>()
        { (var, current) => new FutoshikiConstraintTree(n, var, current) };
    List<IConstraint> cspConstraints = new List<IConstraint>();
    cspConstraints.AddRange(constraints);
    ICSPTree<int?> csp = new BacktrackingTreeCSP<int?>(
        data,
        cspConstraints,
        (domain) => { return new SimpleHeuristic<int?>(domain); },
        (variables) => { return new SimpleChildHeuristic<int?>(n, variables); },
        cspFactoriesConstraints);
    PrintResultsTree<int?>(n, csp);
}

void TestNewRandomChildFutoshiki(int n)
{
    Console.WriteLine($"Futoshiki new random child test for {n}x{n}");
    FutoshikiLoaderTree futoshikiLoader = new FutoshikiLoaderTree(n);
    (List<Variable<int?>> data, List<InequalityConstraint> constraints) = futoshikiLoader.LoadData($"dane/futoshiki_{n}x{n}");
    List<Func<List<Variable<int?>>, Variable<int?>, IConstraint>> cspFactoriesConstraints =
        new List<Func<List<Variable<int?>>, Variable<int?>, IConstraint>>()
        { (var, current) => new FutoshikiConstraintTree(n, var, current) };
    List<IConstraint> cspConstraints = new List<IConstraint>();
    cspConstraints.AddRange(constraints);
    ICSPTree<int?> csp = new BacktrackingTreeCSP<int?>(
        data,
        cspConstraints,
        (domain) => { return new SimpleHeuristic<int?>(domain); },
        (variables) => { return new RandomChildHeuristic<int?>(variables); },
        cspFactoriesConstraints);
    PrintResultsTree<int?>(n, csp);
}

void TestFutoshikiRandom(int n)
{
    Console.WriteLine($"Futoshiki random test for {n}x{n}");
    FutoshikiLoader futoshikiLoader = new FutoshikiLoader(n);
    (Variable<int?>[,] data, List<InequalityConstraint> constraints) = futoshikiLoader.LoadData($"dane/futoshiki_{n}x{n}");
    List<Func<Variable<int?>[,], int, int, IConstraint>> cspFactoriesConstraints =
        new List<Func<Variable<int?>[,], int, int, IConstraint>>()
        { (var, i, j) => new FutoshikiConstraint(var, i, j) };
    List<IConstraint> cspConstraints = new List<IConstraint>();
    cspConstraints.AddRange(constraints);
    ICSP<int?> csp = new CSPBacktracking<int?>(
        data,
        cspConstraints,
        (domain) => { return new RandomHeuristic<int?>(domain); },
        cspFactoriesConstraints);
    PrintResults<int?>(csp);
}

void TestFutoshikiLast(int n)
{
    Console.WriteLine($"Futoshiki last test for {n}x{n}");
    FutoshikiLoader futoshikiLoader = new FutoshikiLoader(n);
    (Variable<int?>[,] data, List<InequalityConstraint> constraints) = futoshikiLoader.LoadData($"dane/futoshiki_{n}x{n}");
    List<Func<Variable<int?>[,], int, int, IConstraint>> cspFactoriesConstraints =
        new List<Func<Variable<int?>[,], int, int, IConstraint>>()
        { (var, i, j) => new FutoshikiConstraint(var, i, j) };
    List<IConstraint> cspConstraints = new List<IConstraint>();
    cspConstraints.AddRange(constraints);
    ICSP<int?> csp = new CSPBacktracking<int?>(
        data,
        cspConstraints,
        (domain) => { return new LastHeuristic<int?>(domain); },
        cspFactoriesConstraints);
    PrintResults<int?>(csp);
}

void TestForwardFutoshiki(int n)
{
    Console.WriteLine($"Futoshiki test for {n}x{n}");
    FutoshikiLoader futoshikiLoader = new FutoshikiLoader(n);
    (Variable<int?>[,] data, List<InequalityConstraint> constraints) = futoshikiLoader.LoadData($"dane/futoshiki_{n}x{n}");
    List<Func<Variable<int?>[,], int, int, IConstraint>> cspFactoriesConstraints =
        new List<Func<Variable<int?>[,], int, int, IConstraint>>()
        { (var, i, j) => new FutoshikiConstraint(var, i, j) };
    List<IConstraint> cspConstraints = new List<IConstraint>();
    cspConstraints.AddRange(constraints);
    ICSP<int?> csp = new ForwardCheckingCSP<int?>(
        data,
        cspConstraints,
        (domain) => { return new SimpleHeuristic<int?>(domain); },
        (i, j, value) => { return new FutoshikiForwardCheck(data, constraints, i, j, value); },
        cspFactoriesConstraints);
    PrintResults<int?>(csp);
}

void TestNewForwardFutoshiki(int n)
{
    Console.WriteLine($"Futoshiki new forward test for {n}x{n}");
    FutoshikiLoaderTree futoshikiLoader = new FutoshikiLoaderTree(n);
    (List<Variable<int?>> data, List<InequalityConstraint> constraints) = futoshikiLoader.LoadData($"dane/futoshiki_{n}x{n}");
    List<Func<List<Variable<int?>>, Variable<int?>, IConstraint>> cspFactoriesConstraints =
        new List<Func<List<Variable<int?>>, Variable<int?>, IConstraint>>()
        { (var, current) => new FutoshikiConstraintTree(n, var, current) };
    List<IConstraint> cspConstraints = new List<IConstraint>();
    cspConstraints.AddRange(constraints);
    ICSPTree<int?> csp = new ForwardCheckingTreeCSP<int?>(
        data,
        cspConstraints,
        (domain) => { return new SimpleHeuristic<int?>(domain); },
        (variables) => { return new SimpleChildHeuristic<int?>(n, variables); },
        (variables, current) => { return new FutoshikiForwardCheckTree(n, variables, constraints, current); },
        cspFactoriesConstraints);
    PrintResultsTree<int?>(n, csp);
}

//TestBinary(6, 6);
//TestBinary(8, 8);
//TestBinary(10, 10);
//TestNewBinary(6, 6);
//TestNewBinary(8, 8);
//TestNewBinary(10, 10);
//TestNewRandomChildBinary(6, 6);
//TestNewRandomChildBinary(8, 8); // <- Działa długo
//TestNewRandomChildBinary(10, 10); // <- Działa bardzo długo nie używać
//TestBinaryRandom(6, 6);
//TestBinaryRandom(8, 8);
//TestBinaryRandom(10, 10);
//TestForwardBinary(6, 6); // <- Nie działa
//TestForwardBinary(8, 8); // <- Nie działa
//TestForwardBinary(10, 10); // <- Nie działa
//TestFutoshiki(4);
//TestFutoshiki(5);
//TestFutoshiki(6);
//TestFutoshikiRandom(4);
//TestFutoshikiRandom(5);
//TestFutoshikiRandom(6);
//TestFutoshikiLast(4);
//TestFutoshikiLast(5);
//TestFutoshikiLast(6);
//TestForwardFutoshiki(4);
//TestForwardFutoshiki(5);
//TestForwardFutoshiki(6);
TestNewForwardFutoshiki(4);
TestNewForwardFutoshiki(5);
TestNewForwardFutoshiki(6);
//TestNewFutoshiki(4);
//TestNewFutoshiki(5);
//TestNewFutoshiki(6);
//TestNewRandomChildFutoshiki(4);
//TestNewRandomChildFutoshiki(5);
//TestNewRandomChildFutoshiki(6);  // <- Działa bardzo długo nie używać
