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
            string filePath = "";
            string currentPath = "";
            StringBuilder CSharpCompiledString = new StringBuilder();
            if (args.Length > 1)
            {
                currentPath = Directory.GetCurrentDirectory();
                filePath = currentPath + args[0];
                if (!File.Exists(filePath))
                {
                    throw new Exception("File not found.");
                }
                else
                {
                    CSharpCompiledString = CompileProgramToCSharp(filePath);
                }
                if (args[1] == "-S")
                {
                    SaveProgram(CSharpCompiledString.ToString(), currentPath);
                }
            }
            else if (args.Length == 1)
            {
                if (args[0] == "-S")
                {
                    Console.WriteLine("Insert full path to file or name of file in current folder");
                    filePath = Console.ReadLine();
                    currentPath = Directory.GetCurrentDirectory();
                    while (!File.Exists(filePath) && !File.Exists(currentPath + filePath))
                    {
                        Console.WriteLine("File not found, try again.");
                        filePath = Console.ReadLine();
                    }
                    if (File.Exists(filePath))
                    {
                        CSharpCompiledString = CompileProgramToCSharp(filePath);
                        SaveProgram(CSharpCompiledString.ToString(), currentPath);
                    }
                    else
                    {
                        CSharpCompiledString = CompileProgramToCSharp(currentPath + filePath);
                        SaveProgram(CSharpCompiledString.ToString(), currentPath);
                    }
                }
                else
                {
                    filePath = Directory.GetCurrentDirectory() + args[0];
                    if (!File.Exists(filePath))
                    {
                        throw new Exception("File not found.");
                    }
                    CSharpCompiledString = CompileProgramToCSharp(filePath);
                }
            }
            else
            {
                Console.WriteLine("Insert full path to file or name of file in current folder");
                filePath = Console.ReadLine();
                currentPath = Directory.GetCurrentDirectory();
                while (!File.Exists(filePath) && !File.Exists(currentPath + filePath))
                {
                    Console.WriteLine("File not found, try again.");
                    filePath = Console.ReadLine();
                }
                if (File.Exists(filePath))
                {
                    CSharpCompiledString = CompileProgramToCSharp(filePath);
                }
                else
                {
                    CSharpCompiledString = CompileProgramToCSharp(currentPath + filePath);
                }
                Console.WriteLine("Do you want to save the C# file? \n1: Yes \n2: No");
                if (Console.ReadLine().Replace(" ", "") == "1")
                {
                    if (File.Exists(filePath))
                    {
                        SaveProgram(CSharpCompiledString.ToString(), currentPath);
                    }
                    else
                    {
                        SaveProgram(CSharpCompiledString.ToString(), currentPath);
                    }
                }
            }
            CSharpCompiler.CompileAndStartConsole(CSharpCompiledString);
        }

        public static StringBuilder CompileProgramToCSharp(string filePath)
        {
            Queue<Token> tokenQueue = new Queue<Token>();

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

            // Semantics
            SymbolTable symbolTable = new SymbolTable();
            DeclarationVisitor dclVisitor = new DeclarationVisitor(symbolTable);
            dclVisitor.Visit(AST);

            var typeVisitor = new TypeVisitor(symbolTable);
            typeVisitor.Visit(AST);

            CodeGeneratorVisitor codeGeneratorVisitor = new CodeGeneratorVisitor(symbolTable);
            codeGeneratorVisitor.Visit(AST);
            return codeGeneratorVisitor.CSharpString;
        }



        // Skabelse af uniform fordeling af tal for floats.................. som Michael overså tidligere da han præsenterede sin random funktion
        // TalMellem0Og1 = 1 / (50000 - randomint(0, 49999));
        // return TalMellem0Og1 * (max - min) + min;


        public static void SaveProgram(string Program, string directoryPath = "")
        {
            //Create string variables needed
            string fileName = "";
            string folderPath = "";
            string destinationOption;

            //Find where to save the file
            do
            {
                Console.WriteLine("\n \nChoose destination: \n1: Desktop \n2: Documents \n3: Current directory");
                destinationOption = Console.ReadLine();
                if (destinationOption == "1")
                {
                    folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
                else if (destinationOption == "2")
                {
                    folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                else if (destinationOption == "3")
                {
                    if (directoryPath != "")
                    {
                        folderPath = directoryPath;
                    }
                    else
                    {
                        Console.WriteLine("Unavailable option chosen");
                    }
                }
                else
                {
                    Console.WriteLine("Unavailable option chosen");
                }
            } while (!(destinationOption == "1" || destinationOption == "2" ||( destinationOption == "2" && directoryPath != "")));

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
