using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie2.Components
{
    internal class BinaryConstraint : IConstraint
    {
        Variable<short?>[,] Variables { get; } 
        public BinaryConstraint(Variable<short?>[,] variables)
        {
            Variables = variables;
        }
        public bool CheckConstraint()
        {
            Dictionary<int, List<int?>> columns = new Dictionary<int, List<int?>>();
            Dictionary<int, List<int?>> rows = new Dictionary<int, List<int?>>();
            //Column and rows 3 same values
            for(int i = 0; i < Variables.GetLength(0); i++)
            {
                rows.Add(i, new List<int?>());
                for(int j = 0; j < Variables.GetLength(1); j++)
                {
                    if(i == 0)
                        columns.Add(j, new List<int?>());
                    if(i - 2 >= 0)
                    {
                        if (Variables[i, j].Value.HasValue && Variables[i - 1, j].Value.HasValue && Variables[i - 2, j].Value.HasValue)
                            if (Variables[i, j].Value == Variables[i - 1, j].Value && Variables[i, j].Value == Variables[i - 2, j].Value)
                                return false;
                    }
                    if(j - 2 >= 0)
                    {
                        if (Variables[i, j].Value.HasValue && Variables[i, j-1].Value.HasValue && Variables[i, j - 2].Value.HasValue)
                            if (Variables[i, j].Value == Variables[i, j - 1].Value && Variables[i, j].Value == Variables[i, j - 2].Value)
                                return false;
                    }
                    rows[i].Add(Variables[i, j].Value);
                    columns[j].Add(Variables[i, j].Value);
                }
            }       
            List<List<int?>> fullColumns = columns.Values.Where(column => !column.Contains(null)).ToList();
            List<List<int?>> fullRows = rows.Values.Where(row => !row.Contains(null)).ToList();
            int columnsZeros = 0;
            for(int i = 0; i < fullColumns.Count; i++) {
                int zeros = fullColumns[i].Where(elem => elem.Value == 0).Count();
                if (i == 0)
                    columnsZeros = zeros;
                if (columnsZeros != zeros)
                    return false;
                for (int j = 0; j < fullColumns.Count; j++) {
                    if(i != j)
                    {   
                        if(fullColumns[i].SequenceEqual(fullColumns[j]))
                            return false;
                    }
                }
            }
            int rowZeros = 0;
            for (int i = 0; i < fullRows.Count; i++)
            {
                int zeros = fullRows[i].Where(elem => elem.Value == 0).Count();
                if (i == 0)
                    rowZeros = zeros;
                if (rowZeros != zeros)
                    return false;
                for (int j = 0; j < fullRows.Count; j++)
                {
                    if (i != j)
                    {
                        if (fullRows[i].SequenceEqual(fullRows[j]))
                            return false;
                    }
                }
            }
            return true;
        }
    }
}
