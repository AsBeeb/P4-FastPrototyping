using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace P4Project
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Michael er awesome... indskriv sti til mappe:");
            string sti = Console.ReadLine();
            Console.WriteLine("Skriv filnavn:");
            string filnavn = "\\" + Console.ReadLine() + ".txt";
            using (StreamReader sr = new StreamReader(sti+filnavn)) {
                string tekst = sr.ReadToEnd();
                Console.WriteLine(tekst);
            }
            Console.Read();
        }
    }
}
