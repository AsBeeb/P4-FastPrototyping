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
using CSharpCompilerLib;
using CodeGeneration;

namespace P4Project
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Queue<Token> tokenQueue = new Queue<Token>();

            string fileName = args[0];
            string filePath = Directory.GetCurrentDirectory();
            //Console.WriteLine("Indskriv sti til mappe:");
            //string sti = Console.ReadLine();
            //Console.WriteLine("Skriv filnavn:");
            //string filnavn = "\\" + Console.ReadLine() + ".txt";
            //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string gitPath = @"\GitHub\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\";
            //string fileToOpen = "Demo2";
            //string fileExtension = ".txt";
            //string filePath = String.Format("{0}{1}{2}{3}", docPath, gitPath, fileToOpen, fileExtension);
            //string filePath = @"C:\Users\Michael\Source\Repos\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\TurBaseretKampspil.txt";
            //string filePath = @"C:\Users\Michael\Source\Repos\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\Minesweeper.txt";

            // Print location where project is installed TEMPORARY
            Console.WriteLine("Current Dir: " + filePath);

            // Check for args
            if (args.Length > 0)
            {
                filePath += "\\" + fileName;
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not found.");
                    return;
                }
            }
            else
            {
                // If no arguments are given (filename, save option) end.
                return;
            }

            // Scanner
            using (StreamReaderExpanded reader = new StreamReaderExpanded(filePath))
            {
                do
                {
                    Token tempToken = Scanner.Scan(reader);
                    if (tempToken != null)
                    {
                        tokenQueue.Enqueue(tempToken);
                    }

                } while (tokenQueue.Count == 0 || tokenQueue.Last().Type != TokenType.eof_token);

            }

            // Parser
            Parser parser = new Parser(tokenQueue);
            ProgNode AST = parser.StartParse();

            // Pretty printer
            //PrettyPrintVisitor vis = new PrettyPrintVisitor();
            //vis.Visit(AST);

            // Semantics
            SymbolTable symbolTable = new SymbolTable();
            DeclarationVisitor dclVisitor = new DeclarationVisitor(symbolTable);
            dclVisitor.Visit(AST);

            var typeVisitor = new TypeVisitor(symbolTable);
            typeVisitor.Visit(AST);

            CodeGeneratorVisitor codeGeneratorVisitor = new CodeGeneratorVisitor(symbolTable);
            codeGeneratorVisitor.Visit(AST);

            if (args.Length > 1 && args[1] == "-s" )
            {
                SaveProgram(codeGeneratorVisitor.CSharpString.ToString(), filePath);
            }

            CSharpCompiler.CompileAndStartConsole(codeGeneratorVisitor.CSharpString);
        }

        public static void SaveProgram(string Program, string filePath)
        {
            //Naming of the file
            //filePath = filePath.TrimEnd('.', 't', 'x', 't');
            filePath = filePath.Replace(".txt", "");
            filePath += DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            filePath += ".cs";
            Console.WriteLine(filePath);
            Console.ReadKey();
            //Creates and writes to the file
            using (StreamWriter SW = File.CreateText(filePath))
            {
                SW.WriteLine(Program);
            }
        }
    }
}
