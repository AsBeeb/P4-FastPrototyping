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
            Console.WriteLine("Indskriv sti til mappe:");
            string sti = Console.ReadLine();
            Console.WriteLine("Skriv filnavn:");
            string filnavn = "\\" + Console.ReadLine() + ".txt";
            using (StreamReader sr = new StreamReader(sti+filnavn)) {
                while (sr.Peek() > -1 )
                {
                    string tekst = "";
                    while ( ((char)sr.Peek() != ' ' && ((char)sr.Peek()) != '\n') && sr.Peek() > -1)
                    {
                        tekst += ((Char)sr.Read()).ToString();
                    }
                    if (sr.Peek() > -1) { sr.Read(); }
                    Console.WriteLine(tekst);
                } 
            }
            Console.Read();
        }
    }
}
