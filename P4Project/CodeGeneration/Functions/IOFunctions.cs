using System;

namespace CodeGeneration.Functions
{
    public class IOFunctions
    {
        public static string GetString_()
        {
            return Console.ReadLine();
        }

        public static float GetNumber_()
        {
            float val;
            while (!float.TryParse(Console.ReadLine(), out val));
            
            return val;
        }

        public static void Print_(object txt)
        {
            Console.Write(txt);
        }

        public static int ChooseOption_(bool displayIndex, params string[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                string msg = ((displayIndex == true) ? $"{i + 1}: " : "");
                Console.WriteLine(msg + $"{options[i]}");
            }

            int choice = 0;
            do
            {
                int.TryParse(Console.ReadLine(), out choice);
            }
            while (choice <= 0 || choice > options.Length);

            return choice;
        }

        public static float GetRandomFloat_(float min, float max)
        {
            return (float)random.NextDouble() * (max - min) + min;
        }

        private static Random random = new Random();

        public static int GetRandomInt_(int min, int max)
        {
            return random.Next(min, max + 1);
        }

        public static void SetSeed_(int seed)
        {
            random = new Random(seed);
        }
    }
}
