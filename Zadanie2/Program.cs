using Zadanie2.Constraints;
using Zadanie2.Loaders;
using Zadanie2.CSP;
using Zadanie2.Heuristics;
using Zadanie2;
using Zadanie2.ForwardChecking;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Legends;
using OxyPlot.Axes;
using OxyPlot.Core.Drawing;

void PrintMetrics(Metrics metrics)
{
    Console.WriteLine($"Time: {metrics.Time} Nodes Visited: {metrics.NodesVisited}");
}

void PrintResults<T>(ICSP<T> csp)
{
    if (csp.Solutions.Count > 0)
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
    if (csp.Solutions.Count > 0)
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

Metrics TestNewBinary(int w, int h, bool printResults)
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
    Metrics metrics = new Metrics();
    DateTime startTime = DateTime.Now;
    bool result = csp.FindSolution();
    metrics.Time = (DateTime.Now - startTime).TotalMilliseconds;
    metrics.NodesVisited = csp.Iterations;
    if (printResults)
        PrintResultsTree<short?>(w, csp);
    return metrics;
}

Metrics TestNewRandomChildBinary(int w, int h, bool printResults)
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
    Metrics metrics = new Metrics();
    DateTime startTime = DateTime.Now;
    bool result = csp.FindSolution();
    metrics.Time = (DateTime.Now - startTime).TotalMilliseconds;
    metrics.NodesVisited = csp.Iterations;
    if (printResults)
        PrintResultsTree<short?>(w, csp);
    return metrics;
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

Metrics TestNewForwardBinary(int w, int h, bool printResults)
{
    Console.WriteLine($"Binary new forawrd test for {w}x{h}");
    BinaryLoaderTree binaryLoader = new BinaryLoaderTree(w, h);
    List<Variable<short?>> data = binaryLoader.LoadData($"dane/binary_{w}x{h}");
    ICSPTree<short?> csp = new ForwardCheckingTreeCSP<short?>(data,
        new List<IConstraint>() { new BinaryConstraintTree(w, h, data) },
        (domain) => { return new SimpleHeuristic<short?>(domain); },
        (List<Variable<short?>> variables) => { return new SimpleChildHeuristic<short?>(w, variables); }, 
        (variables, current) => { return new BinaryForwardCheckTree(w, variables, current); },
        new List<Func<List<Variable<short?>>, Variable<short?>, IConstraint>>()
        );
    Metrics metrics = new Metrics();
    DateTime startTime = DateTime.Now;
    bool result = csp.FindSolution();
    metrics.Time = (DateTime.Now - startTime).TotalMilliseconds;
    metrics.NodesVisited = csp.Iterations;
    if (printResults)
        PrintResultsTree<short?>(w, csp);
    return metrics;
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

Metrics TestNewFutoshiki(int n, bool printResults)
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
    Metrics metrics = new Metrics();
    DateTime startTime = DateTime.Now;
    bool result = csp.FindSolution();
    metrics.Time = (DateTime.Now - startTime).TotalMilliseconds;
    metrics.NodesVisited = csp.Iterations;
    if (printResults)
        PrintResultsTree<int?>(n, csp);
    return metrics;
}

Metrics TestNewRandomChildFutoshiki(int n, bool printResults)
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
    Metrics metrics = new Metrics();
    DateTime startTime = DateTime.Now;
    bool result = csp.FindSolution();
    metrics.Time = (DateTime.Now - startTime).TotalMilliseconds;
    metrics.NodesVisited = csp.Iterations;
    if (printResults)
        PrintResultsTree<int?>(n, csp);
    return metrics;
}

Metrics TestNewForwardRandomChildFutoshiki(int n, bool printResults)
{
    Console.WriteLine($"Futoshiki new forward random child test for {n}x{n}");
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
        (variables) => { return new RandomChildHeuristic<int?>(variables); },
        (variables, current) => { return new FutoshikiForwardCheckTree(n, variables, constraints, current); },
        cspFactoriesConstraints);
    Metrics metrics = new Metrics();
    DateTime startTime = DateTime.Now;
    bool result = csp.FindSolution();
    metrics.Time = (DateTime.Now - startTime).TotalMilliseconds;
    metrics.NodesVisited = csp.Iterations;
    if (printResults)
        PrintResultsTree<int?>(n, csp);
    return metrics;
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

Metrics TestNewForwardFutoshiki(int n, bool printResults)
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
    Metrics metrics = new Metrics();
    DateTime startTime = DateTime.Now;
    bool result = csp.FindSolution();
    metrics.Time = (DateTime.Now - startTime).TotalMilliseconds;
    metrics.NodesVisited = csp.Iterations;
    if (printResults)
        PrintResultsTree<int?>(n, csp);
    return metrics;
}

Metrics TestNewRandomForwardFutoshiki(int n, bool printResults)
{
    Console.WriteLine($"Futoshiki new forward random heuristic test for {n}x{n}");
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
        (domain) => { return new RandomHeuristic<int?>(domain); },
        (variables) => { return new SimpleChildHeuristic<int?>(n, variables); },
        (variables, current) => { return new FutoshikiForwardCheckTree(n, variables, constraints, current); },
        cspFactoriesConstraints);
    Metrics metrics = new Metrics();
    DateTime startTime = DateTime.Now;
    bool result = csp.FindSolution();
    metrics.Time = (DateTime.Now - startTime).TotalMilliseconds;
    metrics.NodesVisited = csp.Iterations;
    if (printResults)
        PrintResultsTree<int?>(n, csp);
    return metrics;
}

//TestBinary(6, 6);
//TestBinary(8, 8);
//TestBinary(10, 10);
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
//TestForwardFutoshiki(4); <- Nie działa
//TestForwardFutoshiki(5); <- Nie działa
//TestForwardFutoshiki(6); <- Nie działa

Metrics binary_6_6 = TestNewBinary(6, 6, false);
PrintMetrics(binary_6_6);
Metrics binary_8_8 = TestNewBinary(8, 8, false);
PrintMetrics(binary_8_8);
Metrics binary_10_10 = TestNewBinary(10, 10, false);
PrintMetrics(binary_10_10);
//Metrics binary_random_6_6 = TestNewRandomChildBinary(6, 6, false);
//Metrics binary_random_8_8 = TestNewRandomChildBinary(8, 8, false); // <- Działa długo
//TestNewRandomChildBinary(10, 10); // <- Działa bardzo długo nie używać
Metrics binary_forward_6_6 = TestNewForwardBinary(6, 6, false);
PrintMetrics(binary_forward_6_6);
Metrics binary_forward_8_8 = TestNewForwardBinary(8, 8, false);
PrintMetrics(binary_forward_8_8);
Metrics binary_forward_10_10 = TestNewForwardBinary(10, 10, false);
PrintMetrics(binary_forward_10_10);

Metrics futoshiki_forward_4 = TestNewForwardFutoshiki(4, false);
PrintMetrics(futoshiki_forward_4);
Metrics futoshiki_forward_5 = TestNewForwardFutoshiki(5, false);
PrintMetrics(futoshiki_forward_5);
Metrics futoshiki_forward_6 = TestNewForwardFutoshiki(6, false);
PrintMetrics(futoshiki_forward_6);
Metrics futoshiki_4 = TestNewFutoshiki(4, false);
PrintMetrics(futoshiki_4);
Metrics futoshiki_5 = TestNewFutoshiki(5, false);
PrintMetrics(futoshiki_5);
Metrics futoshiki_6 = TestNewFutoshiki(6, false);
PrintMetrics(futoshiki_6);
//Metrics futoshiki_random_4 = TestNewRandomChildFutoshiki(4, false);
//Metrics futoshiki_random_5 = TestNewRandomChildFutoshiki(5, false);
//Metrics futoshiki_random_6 = TestNewRandomChildFutoshiki(6, false);  // <- Działa bardzo długo nie używać
Metrics futoshiki_forward_random_4 = TestNewForwardRandomChildFutoshiki(4, false);
PrintMetrics(futoshiki_forward_random_4);
Metrics futoshiki_forward_random_5 = TestNewForwardRandomChildFutoshiki(5, false);
PrintMetrics(futoshiki_forward_random_5);
Metrics futoshiki_forward_random_6 = TestNewForwardRandomChildFutoshiki(6, false);
PrintMetrics(futoshiki_forward_random_6);
Metrics futoshiki_forward_random_heuristic_4 = TestNewRandomForwardFutoshiki(4, false);
PrintMetrics(futoshiki_forward_random_4);
Metrics futoshiki_forward_random_heuristic_5 = TestNewRandomForwardFutoshiki(5, false);
PrintMetrics(futoshiki_forward_random_5);
Metrics futoshiki_forward_random_heuristic_6 = TestNewRandomForwardFutoshiki(6, false);
PrintMetrics(futoshiki_forward_random_heuristic_6);

PlotModel binary_comparision_plot = new PlotModel();
binary_comparision_plot.Background = OxyColors.White;
Legend legend = new Legend();
legend.LegendPosition = LegendPosition.TopRight;
binary_comparision_plot.Legends.Add(legend);
LinearAxis iterationsAxis = new LinearAxis() { Key = "IterationsAxis", Title = "Iterations" };
LinearAxis timeAxis = new LinearAxis() { Key = "TimeAxis", Position = AxisPosition.Right, Title = "Time" };
binary_comparision_plot.Axes.Add(iterationsAxis);
binary_comparision_plot.Axes.Add(timeAxis);
LineSeries binary_comparision_series_iterations = new LineSeries();
binary_comparision_series_iterations.Title = "Binary backtracking iterations";
binary_comparision_series_iterations.Color = OxyColors.DarkRed;
binary_comparision_series_iterations.Points.Add(new DataPoint(6, binary_6_6.NodesVisited));
binary_comparision_series_iterations.Points.Add(new DataPoint(8, binary_8_8.NodesVisited));
binary_comparision_series_iterations.Points.Add(new DataPoint(10, binary_10_10.NodesVisited));
binary_comparision_series_iterations.YAxisKey = "IterationsAxis";
LineSeries binary_comparision_series_Time = new LineSeries();
binary_comparision_series_Time.Title = "Binary backtracking Time";
binary_comparision_series_Time.Color = OxyColors.DarkGreen;
binary_comparision_series_Time.Points.Add(new DataPoint(6, binary_6_6.Time));
binary_comparision_series_Time.Points.Add(new DataPoint(8, binary_8_8.Time));
binary_comparision_series_Time.Points.Add(new DataPoint(10, binary_10_10.Time));
binary_comparision_series_Time.YAxisKey = "TimeAxis";
LineSeries binary_comparision_forward_series_iterations = new LineSeries();
binary_comparision_forward_series_iterations.Title = "Binary forward checking iterations";
binary_comparision_forward_series_iterations.Color = OxyColors.OrangeRed;
binary_comparision_forward_series_iterations.Points.Add(new DataPoint(6, binary_forward_6_6.NodesVisited));
binary_comparision_forward_series_iterations.Points.Add(new DataPoint(8, binary_forward_8_8.NodesVisited));
binary_comparision_forward_series_iterations.Points.Add(new DataPoint(10, binary_forward_10_10.NodesVisited));
binary_comparision_forward_series_iterations.YAxisKey = "IterationsAxis";
LineSeries binary_comparision_forward_series_Time = new LineSeries();
binary_comparision_forward_series_Time.Title = "Binary forward checking Time";
binary_comparision_forward_series_Time.Color = OxyColors.LightGreen;
binary_comparision_forward_series_Time.Points.Add(new DataPoint(6, binary_forward_6_6.Time));
binary_comparision_forward_series_Time.Points.Add(new DataPoint(8, binary_forward_8_8.Time));
binary_comparision_forward_series_Time.Points.Add(new DataPoint(10, binary_forward_10_10.Time));
binary_comparision_forward_series_Time.YAxisKey = "TimeAxis";
binary_comparision_plot.Series.Add(binary_comparision_series_iterations);
binary_comparision_plot.Series.Add(binary_comparision_series_Time);
binary_comparision_plot.Series.Add(binary_comparision_forward_series_iterations);
binary_comparision_plot.Series.Add(binary_comparision_forward_series_Time);
DirectoryInfo dinfo = new DirectoryInfo("Images");
if (!dinfo.Exists)
    dinfo.Create();
PngExporter.Export(binary_comparision_plot, "Images/Binary Comparision Plot.png", 1920, 1080);


PlotModel futoshiki_comparision_plot = new PlotModel();
futoshiki_comparision_plot.Background = OxyColors.White;
Legend futoshiki_legend = new Legend();
futoshiki_legend.LegendPosition = LegendPosition.TopRight;
futoshiki_comparision_plot.Legends.Add(futoshiki_legend);
LinearAxis futoshiki_iterationsAxis = new LinearAxis() { Key = "IterationsAxis", Title = "Iterations" };
LinearAxis futoshiki_timeAxis = new LinearAxis() { Key = "TimeAxis", Position = AxisPosition.Right, Title = "Time" };
futoshiki_comparision_plot.Axes.Add(futoshiki_iterationsAxis);
futoshiki_comparision_plot.Axes.Add(futoshiki_timeAxis);
LineSeries futoshiki_comparision_series_iterations = new LineSeries();
futoshiki_comparision_series_iterations.Title = "futoshiki backtracking iterations";
futoshiki_comparision_series_iterations.Color = OxyColors.DarkRed;
futoshiki_comparision_series_iterations.Points.Add(new DataPoint(4, futoshiki_4.NodesVisited));
futoshiki_comparision_series_iterations.Points.Add(new DataPoint(5, futoshiki_5.NodesVisited));
futoshiki_comparision_series_iterations.Points.Add(new DataPoint(6, futoshiki_6.NodesVisited));
futoshiki_comparision_series_iterations.YAxisKey = "IterationsAxis";
LineSeries futoshiki_comparision_series_Time = new LineSeries();
futoshiki_comparision_series_Time.Title = "futoshiki backtracking Time";
futoshiki_comparision_series_Time.Color = OxyColors.DarkGreen;
futoshiki_comparision_series_Time.Points.Add(new DataPoint(4, futoshiki_4.Time));
futoshiki_comparision_series_Time.Points.Add(new DataPoint(5, futoshiki_5.Time));
futoshiki_comparision_series_Time.Points.Add(new DataPoint(6, futoshiki_6.Time));
futoshiki_comparision_series_Time.YAxisKey = "TimeAxis";
LineSeries futoshiki_comparision_forward_series_iterations = new LineSeries();
futoshiki_comparision_forward_series_iterations.Title = "futoshiki forward checking iterations";
futoshiki_comparision_forward_series_iterations.Color = OxyColors.OrangeRed;
futoshiki_comparision_forward_series_iterations.Points.Add(new DataPoint(4, futoshiki_forward_4.NodesVisited));
futoshiki_comparision_forward_series_iterations.Points.Add(new DataPoint(5, futoshiki_forward_5.NodesVisited));
futoshiki_comparision_forward_series_iterations.Points.Add(new DataPoint(6, futoshiki_forward_6.NodesVisited));
futoshiki_comparision_forward_series_iterations.YAxisKey = "IterationsAxis";
LineSeries futoshiki_comparision_forward_series_Time = new LineSeries();
futoshiki_comparision_forward_series_Time.Title = "futoshiki forward checking Time";
futoshiki_comparision_forward_series_Time.Color = OxyColors.LightGreen;
futoshiki_comparision_forward_series_Time.Points.Add(new DataPoint(4, futoshiki_forward_4.Time));
futoshiki_comparision_forward_series_Time.Points.Add(new DataPoint(5, futoshiki_forward_5.Time));
futoshiki_comparision_forward_series_Time.Points.Add(new DataPoint(6, futoshiki_forward_6.Time));
futoshiki_comparision_forward_series_Time.YAxisKey = "TimeAxis";
futoshiki_comparision_plot.Series.Add(futoshiki_comparision_series_iterations);
futoshiki_comparision_plot.Series.Add(futoshiki_comparision_series_Time);
futoshiki_comparision_plot.Series.Add(futoshiki_comparision_forward_series_iterations);
futoshiki_comparision_plot.Series.Add(futoshiki_comparision_forward_series_Time);
PngExporter.Export(futoshiki_comparision_plot, "Images/futoshiki Comparision Plot.png", 1920, 1080);



PlotModel child_heuristic_comparision_plot = new PlotModel();
child_heuristic_comparision_plot.Background = OxyColors.White;
Legend child_heuristic_legend = new Legend();
child_heuristic_legend.LegendPosition = LegendPosition.TopRight;
child_heuristic_comparision_plot.Legends.Add(child_heuristic_legend);
LinearAxis child_heuristic_iterationsAxis = new LinearAxis() { Key = "IterationsAxis", Title = "Iterations" };
LinearAxis child_heuristic_timeAxis = new LinearAxis() { Key = "TimeAxis", Position = AxisPosition.Right, Title = "Time" };
child_heuristic_comparision_plot.Axes.Add(child_heuristic_iterationsAxis);
child_heuristic_comparision_plot.Axes.Add(child_heuristic_timeAxis);
LineSeries child_heuristic_comparision_series_iterations = new LineSeries();
child_heuristic_comparision_series_iterations.Title = "futoshiki standard heuristic iterations";
child_heuristic_comparision_series_iterations.Color = OxyColors.DarkRed;
child_heuristic_comparision_series_iterations.Points.Add(new DataPoint(4, futoshiki_forward_4.NodesVisited));
child_heuristic_comparision_series_iterations.Points.Add(new DataPoint(5, futoshiki_forward_5.NodesVisited));
child_heuristic_comparision_series_iterations.Points.Add(new DataPoint(6, futoshiki_forward_6.NodesVisited));
child_heuristic_comparision_series_iterations.YAxisKey = "IterationsAxis";
LineSeries child_heuristic_comparision_series_Time = new LineSeries();
child_heuristic_comparision_series_Time.Title = "futoshiki standard heuristic Time";
child_heuristic_comparision_series_Time.Color = OxyColors.DarkGreen;
child_heuristic_comparision_series_Time.Points.Add(new DataPoint(4, futoshiki_forward_4.Time));
child_heuristic_comparision_series_Time.Points.Add(new DataPoint(5, futoshiki_forward_5.Time));
child_heuristic_comparision_series_Time.Points.Add(new DataPoint(6, futoshiki_forward_6.Time));
child_heuristic_comparision_series_Time.YAxisKey = "TimeAxis";
LineSeries child_heuristic_random_comparision_series_iterations = new LineSeries();
child_heuristic_random_comparision_series_iterations.Title = "futoshiki random heuristic iterations";
child_heuristic_random_comparision_series_iterations.Color = OxyColors.DarkRed;
child_heuristic_random_comparision_series_iterations.Points.Add(new DataPoint(4, futoshiki_forward_random_4.NodesVisited));
child_heuristic_random_comparision_series_iterations.Points.Add(new DataPoint(5, futoshiki_forward_random_5.NodesVisited));
child_heuristic_random_comparision_series_iterations.Points.Add(new DataPoint(6, futoshiki_forward_random_6.NodesVisited));
child_heuristic_random_comparision_series_iterations.YAxisKey = "IterationsAxis";
LineSeries child_heuristic_random_comparision_series_Time = new LineSeries();
child_heuristic_random_comparision_series_Time.Title = "futoshiki random heuristic Time";
child_heuristic_random_comparision_series_Time.Color = OxyColors.DarkGreen;
child_heuristic_random_comparision_series_Time.Points.Add(new DataPoint(4, futoshiki_forward_random_4.Time));
child_heuristic_random_comparision_series_Time.Points.Add(new DataPoint(5, futoshiki_forward_random_5.Time));
child_heuristic_random_comparision_series_Time.Points.Add(new DataPoint(6, futoshiki_forward_random_6.Time));
child_heuristic_random_comparision_series_Time.YAxisKey = "TimeAxis";
child_heuristic_comparision_plot.Series.Add(child_heuristic_comparision_series_iterations);
child_heuristic_comparision_plot.Series.Add(child_heuristic_comparision_series_Time);
child_heuristic_comparision_plot.Series.Add(child_heuristic_random_comparision_series_iterations);
child_heuristic_comparision_plot.Series.Add(child_heuristic_random_comparision_series_Time);
PngExporter.Export(child_heuristic_comparision_plot, "Images/child heuristic Comparision Plot.png", 1920, 1080);

{
    PlotModel heuristic_comparision_plot = new PlotModel();
    heuristic_comparision_plot.Background = OxyColors.White;
    Legend heuristic_legend = new Legend();
    heuristic_legend.LegendPosition = LegendPosition.TopRight;
    heuristic_comparision_plot.Legends.Add(heuristic_legend);
    LinearAxis heuristic_iterationsAxis = new LinearAxis() { Key = "IterationsAxis", Title = "Iterations" };
    LinearAxis heuristic_timeAxis = new LinearAxis() { Key = "TimeAxis", Position = AxisPosition.Right, Title = "Time" };
    heuristic_comparision_plot.Axes.Add(heuristic_iterationsAxis);
    heuristic_comparision_plot.Axes.Add(heuristic_timeAxis);
    LineSeries heuristic_comparision_series_iterations = new LineSeries();
    heuristic_comparision_series_iterations.Title = "futoshiki standard heuristic iterations";
    heuristic_comparision_series_iterations.Color = OxyColors.DarkRed;
    heuristic_comparision_series_iterations.Points.Add(new DataPoint(4, futoshiki_forward_4.NodesVisited));
    heuristic_comparision_series_iterations.Points.Add(new DataPoint(5, futoshiki_forward_5.NodesVisited));
    heuristic_comparision_series_iterations.Points.Add(new DataPoint(6, futoshiki_forward_6.NodesVisited));
    heuristic_comparision_series_iterations.YAxisKey = "IterationsAxis";
    LineSeries heuristic_comparision_series_Time = new LineSeries();
    heuristic_comparision_series_Time.Title = "futoshiki standard heuristic Time";
    heuristic_comparision_series_Time.Color = OxyColors.DarkGreen;
    heuristic_comparision_series_Time.Points.Add(new DataPoint(4, futoshiki_forward_4.Time));
    heuristic_comparision_series_Time.Points.Add(new DataPoint(5, futoshiki_forward_5.Time));
    heuristic_comparision_series_Time.Points.Add(new DataPoint(6, futoshiki_forward_6.Time));
    heuristic_comparision_series_Time.YAxisKey = "TimeAxis";
    LineSeries heuristic_random_comparision_series_iterations = new LineSeries();
    heuristic_random_comparision_series_iterations.Title = "futoshiki random heuristic iterations";
    heuristic_random_comparision_series_iterations.Color = OxyColors.DarkRed;
    heuristic_random_comparision_series_iterations.Points.Add(new DataPoint(4, futoshiki_forward_random_heuristic_4.NodesVisited));
    heuristic_random_comparision_series_iterations.Points.Add(new DataPoint(5, futoshiki_forward_random_heuristic_5.NodesVisited));
    heuristic_random_comparision_series_iterations.Points.Add(new DataPoint(6, futoshiki_forward_random_heuristic_6.NodesVisited));
    heuristic_random_comparision_series_iterations.YAxisKey = "IterationsAxis";
    LineSeries heuristic_random_comparision_series_Time = new LineSeries();
    heuristic_random_comparision_series_Time.Title = "futoshiki random heuristic Time";
    heuristic_random_comparision_series_Time.Color = OxyColors.DarkGreen;
    heuristic_random_comparision_series_Time.Points.Add(new DataPoint(4, futoshiki_forward_random_heuristic_4.Time));
    heuristic_random_comparision_series_Time.Points.Add(new DataPoint(5, futoshiki_forward_random_heuristic_5.Time));
    heuristic_random_comparision_series_Time.Points.Add(new DataPoint(6, futoshiki_forward_random_heuristic_6.Time));
    heuristic_random_comparision_series_Time.YAxisKey = "TimeAxis";
    heuristic_comparision_plot.Series.Add(heuristic_comparision_series_iterations);
    heuristic_comparision_plot.Series.Add(heuristic_comparision_series_Time);
    heuristic_comparision_plot.Series.Add(heuristic_random_comparision_series_iterations);
    heuristic_comparision_plot.Series.Add(heuristic_random_comparision_series_Time);
    PngExporter.Export(heuristic_comparision_plot, "Images/heuristic Comparision Plot.png", 1920, 1080);
}
