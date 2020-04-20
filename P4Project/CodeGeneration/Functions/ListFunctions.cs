using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneration.Functions
{
    class ListFunctions
    {
        private void ListAdd<T>(List<T> lst, T element)
        {
            lst.Add(element);
        }

        private void ListRemove<T>(List<T> lst, T element)
        {
            lst.Remove(element);
        }

        private void ListEmpty<T>(List<T> lst)
        {
            lst.Clear();
        }

        private int ListLength<T>(List<T> lst)
        {
            return lst.Count;
        }
    }
}
