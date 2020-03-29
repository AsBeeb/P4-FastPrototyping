using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ScannerLib;
using ParserLib;
using ParserLib.AST;
using System.Security.Cryptography;
namespace P4Project
{
    class Program
    {
        public static int PrevRandom = GetSeed();
        static void Main(string[] args)
        {
            Queue<Token> tokenQueue = new Queue<Token>();
            //Console.WriteLine("Indskriv sti til mappe:");
            //string sti = Console.ReadLine();
            //Console.WriteLine("Skriv filnavn:");
            //string filnavn = "\\" + Console.ReadLine() + ".txt";
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string gitPath = @"\GitHub\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\";
            string fileToOpen = "Demo2";
            string fileExtension = ".txt";
            //string filePath = String.Format("{0}{1}{2}{3}", docPath, gitPath, fileToOpen, fileExtension);
            string filePath = @"C:\Users\Michael\Source\Repos\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\Michaels Demo.txt";

            using (StreamReaderExpanded reader = new StreamReaderExpanded(filePath))
            {
                do
                {
                    Token tempToken = Scanner.Scan(reader);
                    if (tempToken != null)
                    {
                        tokenQueue.Enqueue(tempToken);
                    }
                    //Console.WriteLine("Value: " + tokenQueue.Last().Value + " Type: " + tokenQueue.Last().Type.ToString() + "\n");

                } while (tokenQueue.Count == 0 || tokenQueue.Last().Type != TokenType.eof_token);

                Console.WriteLine(tokenQueue.Count);
            }
            Console.WriteLine("Scan ended");
            Console.ReadKey();

            Parser parser = new Parser(tokenQueue);
            ASTnode AST = parser.StartParse();
            PrettyPrintVisitor vis = new PrettyPrintVisitor();
            vis.Visit(AST);
            Console.ReadKey();

            Console.WriteLine("Sker der noget?");
            Console.ReadKey();
        }
        //Random number generator using true random numbers. (not fun to use)
        //public static int GenerateRandomNumber()
        //{

        //    RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        //    byte[] bytearray = new byte[1];
        //    randomNumberGenerator.GetBytes(bytearray);

        //    foreach (byte bit in bytearray)
        //    {
        //        Console.WriteLine(bit.ToString());
        //    }
        //    return bytearray[0];
        //}
        public static int GenerateRandomNumber2(int maxValue, int minValue)
        {
            PrevRandom = minValue + (PrevRandom*17 + 11) % maxValue;
            return PrevRandom;
        }
        static int GetSeed()
        {
            return DateTime.Now.Millisecond;
        }
        //removes the randomness from the random number generator by setting the seed
        public static void SetRandomNumberSeed(int value)
        {
            PrevRandom = value;
        }
    }
}
