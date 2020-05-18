using System.Collections.Generic;

namespace CodeGeneration.Functions
{
    public class ListFunctions
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
