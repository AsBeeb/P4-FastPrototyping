using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ScannerLib;
using ParserLib;
namespace P4Project
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue<Token> tokenQueue = new Queue<Token>();
            //Console.WriteLine("Indskriv sti til mappe:");
            //string sti = Console.ReadLine();
            //Console.WriteLine("Skriv filnavn:");
            //string filnavn = "\\" + Console.ReadLine() + ".txt";
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string gitPath = @"\GitHub\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\";
            string fileToOpen = "Numbers";
            string fileExtension = ".txt";
            //string filePath = String.Format("{0}{1}{2}{3}", docPath, gitPath, fileToOpen, fileExtension);
            string filePath = @"C:\Users\Michael\Source\Repos\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\Michaels Demo.txt";

            using (StreamReader reader = new StreamReader(filePath))
            {
                do
                {
                    Token tempToken = Scanner.Scan(reader);
                    if (tempToken != null)
                    {
                        tokenQueue.Enqueue(tempToken);
                    }
                    Console.WriteLine("Value: " + tokenQueue.Last().Value + " Type: " + tokenQueue.Last().Type.ToString() + "\n");

                } while (tokenQueue.Last().Type != TokenType.eof_token);

                Console.WriteLine(tokenQueue.Count);
            }
            Console.WriteLine("Scan ended");
            Console.Read();

            Parser parser = new Parser(tokenQueue);
            parser.StartParse();
            Console.WriteLine("Sker der noget?");
            Console.Read();
            Console.Read();
        }
    }
}
