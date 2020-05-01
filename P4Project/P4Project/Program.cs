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

            string filePath = Directory.GetCurrentDirectory();
      
            // Check for args
            if (args.Length > 0)
            {
                string fileName = args[0];
                filePath += "\\" + fileName;
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not found.");
                    return;
                }
            }
            else
            {
                // If no arguments are given (filename, save option) end
                Console.WriteLine("Error: missing arguments(filename)");
                Console.ReadKey();
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
            
            //DeclarationVisitor dclVisitor = new DeclarationVisitor(symbolTable);
            //dclVisitor.Visit(AST);

            TypeVisitor typeVisitor = new TypeVisitor(symbolTable);
            typeVisitor.Visit(AST);

            // Codegeneration
            CodeGeneratorVisitor codeGeneratorVisitor = new CodeGeneratorVisitor(symbolTable);
            codeGeneratorVisitor.Visit(AST);

            // If -s parameter is set, save the .cs code
            if (args.Length > 1 && args[1] == "-s" )
            {
                SaveProgram(codeGeneratorVisitor.CSharpString.ToString(), filePath);
            }

            // After .cs code is generated, run Roslyn compiler
            CSharpCompiler.CompileAndStartConsole(codeGeneratorVisitor.CSharpString);
        }

        public static void SaveProgram(string Program, string filePath)
        {
            filePath = filePath.Replace(".txt", "");
            filePath += DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            filePath += ".cs";
            
            //Creates and writes to the file
            using (StreamWriter SW = File.CreateText(filePath))
            {
                SW.WriteLine(Program);
            }
        }
    }
}
