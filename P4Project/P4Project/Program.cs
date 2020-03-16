using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ScannerLib;

namespace P4Project
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Token> tokenList = new List<Token>();

            //Console.WriteLine("Indskriv sti til mappe:");
            //string sti = Console.ReadLine();
            //Console.WriteLine("Skriv filnavn:");
            //string filnavn = "\\" + Console.ReadLine() + ".txt";
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string gitPath = @"\GitHub\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\";
            string fileToOpen = "Numbers";
            string fileExtension = ".txt";

            //string filePath = String.Format("{0}{1}{2}{3}", docPath, gitPath, fileToOpen, fileExtension);
            string filePath = @"C:\Users\Michael\Source\Repos\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\Numbers.txt";
            using (StreamReader reader = new StreamReader(filePath)) {
                do
                {
                    Token token = Scanner.Scan(reader);
                    if (token != null)
                    {
                        tokenList.Add(token);
                        Console.WriteLine("Value: " + tokenList.Last().Value + " Type: " + tokenList.Last().Type.ToString() + "\n");
                    }
                } while (tokenList.Last().Type != Token.TokenType.eof_token);

                Console.WriteLine(tokenList.Count);
            }
            Console.Read();

        }
    }
}
