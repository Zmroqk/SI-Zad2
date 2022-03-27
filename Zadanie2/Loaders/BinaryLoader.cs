using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zadanie2.Components;

namespace Zadanie2.Loaders
{
    internal class BinaryLoader : ILoader
    {
        private int Width { get; }
        private int Height { get; }

        public BinaryLoader(int w, int h)
        {
            Width = w;
            Height = h;
        }

        public Variable<short?>[,] LoadData(string path)
        {
            if (!File.Exists(path))
                throw new FileLoadException("Cannot load file. File does not exists", path);
            string[] lines = File.ReadAllLines(path);
            if (lines.GetLength(0) != Height)
                throw new InvalidDataException($"Read {lines.Length} lines from file expected {Height}");            
            Variable<short?>[,] data = new Variable<short?>[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                if (lines.GetLength(0) > 0 && lines[i].Length != Width)
                    throw new InvalidDataException($"Line is {lines[i].Length} characters long expected {Width}");
                for (int j = 0; j < Width; j++)
                {
                    switch(lines[i][j])
                    {
                        case '0':
                            data[i, j] = new Variable<short?>(0);
                            break;
                        case '1':
                            data[i, j] = new Variable<short?>(1);
                            break;
                        default:
                            data[i, j] = new Variable<short?>(new List<short?>() { 0, 1 });
                            break;
                    }
                }
            }
            return data;
        }
    }
}
