using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zadanie2.Constraints;

namespace Zadanie2.Loaders
{
    internal class BinaryLoaderTree : ILoader
    {
        private int Width { get; }
        private int Height { get; }

        public BinaryLoaderTree(int w, int h)
        {
            Width = w;
            Height = h;
        }

        public List<Variable<short?>> LoadData(string path)
        {
            if (!File.Exists(path))
                throw new FileLoadException("Cannot load file. File does not exists", path);
            string[] lines = File.ReadAllLines(path);
            if (lines.GetLength(0) != Height)
                throw new InvalidDataException($"Read {lines.Length} lines from file expected {Height}");            
            List<Variable<short?>> data = new List<Variable<short?>>(Height * Width);
            for (int i = 0; i < Height; i++)
            {
                if (lines.GetLength(0) > 0 && lines[i].Length != Width)
                    throw new InvalidDataException($"Line is {lines[i].Length} characters long expected {Width}");
                for (int j = 0; j < Width; j++)
                {
                    switch(lines[i][j])
                    {
                        case '0':
                            data.Add(new Variable<short?>(0));
                            break;
                        case '1':
                            data.Add(new Variable<short?>(1));
                            break;
                        default:
                            data.Add(new Variable<short?>(new List<short?>() { 0, 1 }));
                            break;
                    }
                }
            }
            return data;
        }
    }
}
