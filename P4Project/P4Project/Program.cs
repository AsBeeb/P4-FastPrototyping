using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ScannerLib;
using ParserLib;
using ParserLib.AST;
using SemanticLib;
using CodeGeneration;

namespace P4Project
{
    public class Program
    {
        private static void Main(string[] args)
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
            string filePath = @"C:\Users\Michael\Source\Repos\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\Demo2.txt";
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
            //Console.ReadKey();

            // Parser
            Parser parser = new Parser(tokenQueue);
            ProgNode AST = parser.StartParse();
            Console.WriteLine("Parser done");
            //Console.ReadKey();

            // Pretty printer
            //PrettyPrintVisitor vis = new PrettyPrintVisitor();
            //vis.Visit(AST);

            // Semantics
            SymbolTable symbolTable = new SymbolTable();
            DeclarationVisitor dclVisitor = new DeclarationVisitor(symbolTable);
            dclVisitor.Visit(AST);
            symbolTable.PrintTable(symbolTable.GlobalScope, 1);

            var typeVisitor = new TypeVisitor(symbolTable);
            typeVisitor.Visit(AST);

            CodeGeneratorVisitor codeGeneratorVisitor = new CodeGeneratorVisitor();
            codeGeneratorVisitor.Visit(AST);

            Console.WriteLine(codeGeneratorVisitor.CSharpString);
            Console.WriteLine("Færdig");
            Console.ReadKey();
        }

    }
}
