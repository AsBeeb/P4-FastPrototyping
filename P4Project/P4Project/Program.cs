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

namespace Versus
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // Check for environment variable
            checkOrSetEnvVariable();
            
            // The queue for tokens
            Queue<Token> tokenQueue = new Queue<Token>();

            // The filepath for the directory in which the compiler have been launched.
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
            
            DeclarationVisitor dclVisitor = new DeclarationVisitor(symbolTable);
            dclVisitor.Visit(AST);

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

        private static void checkOrSetEnvVariable()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string pathVariable = "PATH";
            EnvironmentVariableTarget target = EnvironmentVariableTarget.User;
            string oldVariableValue = Environment.GetEnvironmentVariable(pathVariable, target);

            if (!oldVariableValue.Contains(baseDir))
            {
                //Console.WriteLine("ENV VAR DOESN'T EXIST, SETTING THE ENV VARIABLE");
                string newVariableValue = oldVariableValue + ';' + baseDir;

                // Print out the old and new variables:
                //Console.WriteLine("The old var: " + oldVariableValue);
                //Console.WriteLine("The new var: " + newVariableValue);

                // Set the variable:
                setEnvironmentVariable(pathVariable, baseDir, target);
                Console.WriteLine("ENVIRONMENT VARIABLE SET(first time use)");
            }
            else
            {
                // The var exists do nothing
                //Console.WriteLine("Env Variable exist, do noting.");
            }
        }

        private static void setEnvironmentVariable(string variableName, string folderPath, EnvironmentVariableTarget target)
        {
            // Retrieve the old string var, then add the new path to it.
            string oldVariableValue = Environment.GetEnvironmentVariable(variableName, target);
            string newVariableValue = oldVariableValue + ';' + folderPath;
            Environment.SetEnvironmentVariable(variableName, newVariableValue, target);
        }
    }
}
