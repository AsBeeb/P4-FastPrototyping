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
            //Console.WriteLine("Indskriv sti til mappe:");
            //string sti = Console.ReadLine();
            //Console.WriteLine("Skriv filnavn:");
            //string filnavn = "\\" + Console.ReadLine() + ".txt";
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string gitPath = @"\GitHub\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\";
            string fileToOpen = "Demo2";
            string fileExtension = ".txt";
            string filePath = String.Format("{0}{1}{2}{3}", docPath, gitPath, fileToOpen, fileExtension);
            //string filePath = @"C:\Users\Michael\Source\Repos\P4-FastPrototyping\P4Project\P4Project\KodeEksempler\Demo2.txt";
            
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
            //symbolTable.PrintTable(symbolTable.GlobalScope, 1);

            var typeVisitor = new TypeVisitor(symbolTable);
            typeVisitor.Visit(AST);

            CodeGeneratorVisitor codeGeneratorVisitor = new CodeGeneratorVisitor(symbolTable);
            codeGeneratorVisitor.Visit(AST);

            Console.WriteLine(codeGeneratorVisitor.CSharpString);

            CSharpCompiler.CompileAndStartConsole(codeGeneratorVisitor.CSharpString);

            Console.WriteLine("Done compiling");

            Console.WriteLine("Do you want to save the C# file? \n1: Yes \n2: No");
            if (Console.ReadLine().Replace(" ", "") == "1")
            {
                SaveProgram(codeGeneratorVisitor.CSharpString.ToString());
            }
        }


        // Skabelse af uniform fordeling af tal for floats.................. som Michael overså tidligere da han præsenterede sin random funktion
        // TalMellem0Og1 = 1 / (50000 - randomint(0, 49999));
        // return TalMellem0Og1 * (max - min) + min;


        public static void SaveProgram(string Program)
        {
            //Create string variables needed
            string fileName = "";
            string folderPath = "";
            string destinationOption = "";

            //Find where to save the file
            do
            {
                Console.WriteLine("\n \nChoose destination: \n1: Desktop \n2: Documents");
                destinationOption = Console.ReadLine();
                if (destinationOption == "1")
                {
                    folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
                else if (destinationOption == "2")
                {
                    folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                else
                {
                    Console.WriteLine("Unavailable option chosen");
                }
            } while (!(destinationOption == "1" || destinationOption == "2"));

            //Naming of the file
            Console.Write("\nSpecify name of the file: ");
            folderPath += "\\";
            fileName += Console.ReadLine();
            //If the file already exists we add a '1' at the end until the name doesn't exists already
            while (File.Exists(folderPath + fileName + ".cs"))
            {
                fileName += "1";
            }
            fileName += ".cs";

            Console.WriteLine(folderPath + "\n\n");
            //Creates and writes to the file
            using (StreamWriter SW = File.CreateText(folderPath + fileName))
            {
                SW.WriteLine(Program);
            }

            Console.WriteLine($"{fileName} saved at {folderPath}");
            Console.ReadLine();
        }
    }
}
