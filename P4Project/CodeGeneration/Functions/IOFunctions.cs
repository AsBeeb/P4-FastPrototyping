using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneration.Functions
{
    class IOFunctions
    {
        public string GetString_()
        {
            return Console.ReadLine();
        }

        public float GetNumber_()
        {
            float val;
            while (!float.TryParse(Console.ReadLine(), out val));
            
            return val;
        }

        public void Print_(object txt)
        {
            Console.Write(txt);
        }

        public int ChooseOption_(bool displayIndex, params string[] options)
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

        public float GetRandomFloat_(float min, float max, bool repeatable)
        {
            if (repeatable)
            {
                return (float)random.NextDouble() * (max - min) + min;
            }
            Random rnd = new Random();
            return (float)rnd.NextDouble() * (max - min) + min;
        }

        private static Random random;

        public int GetRandomInt_(int min, int max, bool repeatable)
        {
            if (repeatable)
            {
                return random.Next(min, max + 1);
            }
            Random rnd = new Random();
            return rnd.Next(min, max + 1);
        }

        public void SetSeed_(int seed)
        {
            random = new Random(seed);
        }
    }
}
