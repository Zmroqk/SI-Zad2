using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zadanie2.Components;

namespace Zadanie2.Loaders
{
    internal class FutoshikiLoader : ILoader
    {
        private int Size { get; }
        public FutoshikiLoader(int n)
        {
            Size = n;
        }

        public (Variable<int?>[,] data, List<IConstraint> constraints) LoadData(string path)
        {
            if (!File.Exists(path))
                throw new FileLoadException("Cannot load file. File does not exists", path);
            string[] lines = File.ReadAllLines(path);
            if (lines.GetLength(0) != Size * 2 - 1)
                throw new InvalidDataException($"Read {lines.Length} lines from file expected {Size * 2 - 1}");
            Variable<int?>[,] data = new Variable<int?>[Size, Size];
            for (int i = 0; i < Size * 2 - 1; i += 2)
            {
                for (int j = 0; j < lines[i].Length; j += 2)
                {
                    switch (lines[i][j])
                    {
                        case '-':
                        case '>':
                        case '<':                          
                            break;
                        case 'x':
                            data[i/2, j/2] = new Variable<int?>(Enumerable.Range(1, Size).Select(value => (int?)value).ToList());
                            break;
                        default:
                            data[i/2, j/2] = new Variable<int?>(
                                int.Parse(lines[i][j].ToString()), 
                                Enumerable.Range(1, Size).Select(value => (int?)value).ToList()
                                );
                            break;
                    }
                }
            }
            List<IConstraint> constraints = new List<IConstraint>();
            for (int i = 0; i < Size * 2 - 1; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    switch (lines[i][j])
                    {
                        case '-':
                            break;
                        case '>':
                            if (i % 2 == 0)
                                constraints.Add(new InequalityConstraint(data[i / 2, j / 2], data[i / 2, j / 2 + 1]));
                            else
                                constraints.Add(new InequalityConstraint(data[(i - 1) / 2, j], data[(i + 1) / 2, j]));
                            break;
                        case '<':
                            if (i % 2 == 0)
                                constraints.Add(new InequalityConstraint(data[i / 2, j / 2 + 1], data[i / 2, j / 2]));
                            else
                                constraints.Add(new InequalityConstraint(data[(i + 1) / 2, j], data[(i - 1) / 2, j]));
                            break;
                        default:
                            break;
                    }
                }
            }
            return (data, constraints);
        }
    }
}
