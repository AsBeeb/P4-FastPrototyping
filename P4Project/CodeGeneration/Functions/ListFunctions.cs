using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneration.Functions
{
    class ListFunctions
    {
        private static void ListAdd<T>(List<T> lst, T element)
        {
            lst.Add(element);
        }

        private static void ListRemove<T>(List<T> lst, T element)
        {
            lst.Remove(element);
        }

        private static void ListEmpty<T>(List<T> lst)
        {
            lst.Clear();
        }

        private static int ListLength<T>(List<T> lst)
        {
            return lst.Count;
        }
    }
}
