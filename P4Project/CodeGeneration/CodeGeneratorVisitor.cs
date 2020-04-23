using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserLib;
using ParserLib.AST;
using SemanticLib;

namespace CodeGeneration
{
    public class CodeGeneratorVisitor : Visitor
    {
        public StringBuilder CSharpString = new StringBuilder();
        private int IndentationLevel = 0;
        public SymbolTable SymTbl;

        public CodeGeneratorVisitor(SymbolTable ST) 
        {
            SymTbl = ST;
        }

        private void PrettyPrintNewLine()
        {
            CSharpString.Append($"\n" + new string(' ', 4 * IndentationLevel));
        }

        public override void Visit(ArrayAccessNode node)
        {
            CSharpString.Append("[");
            node.IndexValue.Accept(this);
            CSharpString.Append("]");
        }

        public override void Visit(AssignmentNode node)
        {
            node.LeftValue.Accept(this);
            CSharpString.Append(" = ");
            if (node.LeftValue.Type == "string")
            {
                CSharpString.Append("(");
                node.RightValue.Accept(this);
                CSharpString.Append(").ToString()");
            }
            else if (node.RightValue.Type == "float" && node.LeftValue.Type == "int")
            {
                CSharpString.Append("(int)(");
                node.RightValue.Accept(this);
                CSharpString.Append(")");
            }
            else
            {
                node.RightValue.Accept(this);
            }
            CSharpString.Append(";");
            PrettyPrintNewLine();
        }

        public override void Visit(BinaryExpressionNode node)
        {
            if (node.Operator != BinaryOperator.POWER && node.Operator != BinaryOperator.STRING_CONCAT)
            {
                string binaryOperator = "";
                CSharpString.Append("(" + node.Type + ")(");
                node.LeftExpr.Accept(this);
                switch (node.Operator)
                {
                    case BinaryOperator.GREATER_OR_EQUALS:
                        binaryOperator = ">=";
                        break;
                    case BinaryOperator.LESS_OR_EQUALS:
                        binaryOperator = "<=";
                        break;
                    case BinaryOperator.GREATER_THAN:
                        binaryOperator = ">";
                        break;
                    case BinaryOperator.LESS_THAN:
                        binaryOperator = "<";
                        break;
                    case BinaryOperator.EQUALS:
                        binaryOperator = "==";
                        break;
                    case BinaryOperator.NOT_EQUALS:
                        binaryOperator = "!=";
                        break;
                    case BinaryOperator.PLUS:
                        binaryOperator = "+";
                        break;
                    case BinaryOperator.MINUS:
                        binaryOperator = "-";
                        break;
                    case BinaryOperator.MULTIPLY:
                        binaryOperator = "*";
                        break;
                    case BinaryOperator.DIVIDE:
                        binaryOperator = "/";
                        break;
                    case BinaryOperator.MODULO:
                        binaryOperator = "%";
                        break;
                    case BinaryOperator.OR:
                        binaryOperator = "||";
                        break;
                    case BinaryOperator.AND:
                        binaryOperator = "&&";
                        break;
                }
                CSharpString.Append(" " + binaryOperator + " ");
                node.RightExpr.Accept(this);
                CSharpString.Append(") ");
            }
            else
            {
                switch (node.Operator)
                {
                    case BinaryOperator.STRING_CONCAT:
                        CSharpString.Append("(");
                        node.LeftExpr.Accept(this);
                        CSharpString.Append(").ToString() + (");
                        node.RightExpr.Accept(this);
                        CSharpString.Append(").ToString()");
                        break;
                    case BinaryOperator.POWER:
                        CSharpString.Append("Math.Pow(");
                        node.LeftExpr.Accept(this);
                        CSharpString.Append(",");
                        node.RightExpr.Accept(this);
                        CSharpString.Append(")");
                        break;
                }
            }
        }

        public override void Visit(BlockNode node)
        {
            PrettyPrintNewLine();
            CSharpString.Append("{");
            IndentationLevel++;
            PrettyPrintNewLine();
            foreach (StmtNode stmt in node.StmtNodes)
            {
                stmt.Accept(this);
            }
            IndentationLevel--;
            PrettyPrintNewLine();
            CSharpString.Append("}");
            PrettyPrintNewLine();
        }


        public override void Visit(BoolValueNode node)
        {
            CSharpString.Append(node.BoolValue.ToString().ToLower());
        }

        public override void Visit(ConstructorNode node)
        {
            CSharpString.Append("public ");
            node.Id.Accept(this);
            CSharpString.Append(" (");
            node.FormalParamNodes?.ForEach(x => {
                if (node.FormalParamNodes.IndexOf(x) != 0)
                {
                    CSharpString.Append(", ");
                }
                x.Accept(this);
            });
            CSharpString.Append(")");
            node.Block.Accept(this);
        }

        public override void Visit(DeclarationNode node)
        {
            if (node.IsArray)
            {
                if (IsPrimitiveType(node.Type)){
                    CSharpString.Append("List<" + node.Type + "> ");
                }
                else
                {
                    CSharpString.Append("List<" + node.Type + "_> ");
                }
            }
            else
            {
                CSharpString.Append(node.Type);
                if (!IsPrimitiveType(node.Type))
                {
                    CSharpString.Append("_");
                }
                CSharpString.Append(" ");
            }
            node.Id.Accept(this);
            
            CSharpString.Append(" = ");

            if (node.InitialValue != null)
            {
                if (!IsPrimitiveType(node.Type))
                {
                    if (node.InitialValue is FuncCallExpressionNode StructConstructerCall)
                    {
                        if (!SymTbl.IsDeclaredLocally(StructConstructerCall.Id.Id))
                            CSharpString.Append("new ");
                    }
                    node.InitialValue.Accept(this);
                }
                else
                {
                    if (node.Type == "string")
                    {
                        CSharpString.Append("(");
                        node.InitialValue.Accept(this);
                        CSharpString.Append(").ToString()");
                    }
                    else if (node.Type == "int" && node.InitialValue.Type == "float")
                    {
                        CSharpString.Append("(int)(");
                        node.InitialValue.Accept(this);
                        CSharpString.Append(")");
                    }
                    else
                    {
                        node.InitialValue.Accept(this);
                    }
                }
            }
            else
            {
                //Default values if a variable isn't initialized
                if (node.IsArray || node.Type.Contains("[]"))
                {
                    string type = node.Type.Replace("[]", "");
                    if (IsPrimitiveType(type))
                    {
                        CSharpString.Append("new List<" + type + ">()");
                    }
                    else
                    {
                        CSharpString.Append("new List<" + type + "_>()");
                    }
                }
                else
                {
                    switch (node.Type)
                    {
                        case "int":
                            CSharpString.Append("0");
                            break;
                        case "float":
                            CSharpString.Append("0.0f");
                            break;
                        case "string":
                            CSharpString.Append("\"\"");
                            break;
                        case "bool":
                            CSharpString.Append("false");
                            break;
                        default:
                            CSharpString.Append( "new " + node.Type + "_()");
                            break;
                    }
                } 
            }
            CSharpString.Append(";");
            PrettyPrintNewLine();
        }

        public override void Visit(ElifNode node)
        {
            CSharpString.Append("else if (");
            node.ControlExpr.Accept(this);
            CSharpString.Append(")");
            node.ElifBody.Accept(this);
        }

        public override void Visit(ElseNode node)
        {
            CSharpString.Append("else");
            node.ElseBody.Accept(this);
        }

        public override void Visit(FieldAccessNode node)
        {
            CSharpString.Append(".");
            node.Id.Accept(this);
        }

        public override void Visit(FloatValueNode node)
        {
            CSharpString.Append(node.FloatValue.ToString().Replace(",", ".") +"f");
        }

        public override void Visit(FormalParamNode node)
        {
            //if (node.Type.Contains("[]"))
            if (node.IsArray)
            {
                CSharpString.Append("List<");
            }

            CSharpString.Append(node.Type.Replace("[]", ""));
            // tilføjet et tjek på om det er et struct, hvis det er så tilføjer vi _ efter typen.
            if (!IsPrimitiveType(node.Type.Replace("[]", "")))
            {
                CSharpString.Append("_");
            }
            //if (node.Type.Contains("[]"))
            if (node.IsArray)
            {
                CSharpString.Append("> ");
            }
            CSharpString.Append(" ");
            node.Id.Accept(this);
        }

        public override void Visit(FuncCallExpressionNode node)
        {
            node.Id.Accept(this);
            CSharpString.Append("(");
            node.ActualParameters?.ForEach(x => {
                if (node.ActualParameters.IndexOf(x) != 0)
                {
                    CSharpString.Append(", ");
                }
                x.Accept(this);
            });
            CSharpString.Append(")");
        }

        public override void Visit(FuncCallStmtNode node)
        {
            node.Id.Accept(this);
            CSharpString.Append("(");
            node.ActualParameters?.ForEach(x => {
                if (node.ActualParameters.IndexOf(x) != 0)
                {
                    CSharpString.Append(", ");
                }
                x.Accept(this);
            });
            CSharpString.Append(");");
            PrettyPrintNewLine();
        }

        public override void Visit(FunctionDclNode node)
        {
            CSharpString.Append("static ");
            if (node.ReturnType.Contains("[]"))
            {
                CSharpString.Append("List<");
            }
            
            CSharpString.Append(node.ReturnType.Replace("[]", ""));
            //midlertidig løsning til at sætte _ på typen hvis den ikke er primitiv eller void.
            if (!IsPrimitiveType(node.ReturnType.Replace("[]", "")) && node.ReturnType != "void")
            {
                CSharpString.Append("_");
            }
            if (node.ReturnType.Contains("[]"))
            {
                CSharpString.Append("> ");
            }
            CSharpString.Append(" ");

            // Checks if the function declared is main
            if (node.Id.Id == "main")
            {
                CSharpString.Append("Main");
            }
            else
            {
                node.Id.Accept(this);
            }
            CSharpString.Append("(");
            node.FormalParamNodes?.ForEach(x => {
                if (node.FormalParamNodes.IndexOf(x) != 0)
                {
                    CSharpString.Append(", ");
                }
                x.Accept(this);
            });
            CSharpString.Append(")");
            node.FuncBody.Accept(this);
        }

        public override void Visit(GlobalDclNode node)
        {
            CSharpString.Append("static ");
            //if (node.Type.Contains("[]"))
            if (node.IsArray)
            {
                CSharpString.Append("List<");
                CSharpString.Append(node.Type.Replace("[]", "")); // global int example 
                CSharpString.Append("> ");
            }
            else
            {
                CSharpString.Append(node.Type);
                if (!IsPrimitiveType(node.Type))
                {
                    CSharpString.Append("_");
                }
                CSharpString.Append(" ");
            }


            
            node.Id.Accept(this);
            CSharpString.Append(" = ");
            if (node.InitialValue != null)
            {
                if (!IsPrimitiveType(node.Type))
                {
                    if (node.InitialValue is FuncCallExpressionNode StructConstructerCall)
                    {
                        if (!SymTbl.IsDeclaredLocally(StructConstructerCall.Id.Id))
                            CSharpString.Append("new ");
                    }
                    node.InitialValue.Accept(this);
                }
                else
                {
                    if (node.Type == "string")
                    {
                        CSharpString.Append("(");
                        node.InitialValue.Accept(this);
                        CSharpString.Append(").ToString()");
                    }
                    else if (node.Type == "int" && node.InitialValue.Type == "float")
                    {
                        CSharpString.Append("(int)(");
                        node.InitialValue.Accept(this);
                        CSharpString.Append(")");
                    }
                    else
                    {
                        node.InitialValue.Accept(this);
                    }
                }
            }
            else
            {
                //Default values if a variable isn't initialized
                //if (node.Type.Contains("[]"))
                if (node.IsArray)
                {
                    string type = node.Type.Replace("[]", "");
                    if (IsPrimitiveType(type))
                    {
                        CSharpString.Append("new List<" + type + ">()");
                    }
                    else
                    {
                        CSharpString.Append("new List<" + type + "_>()");
                    }
                }
                else
                {
                    switch (node.Type)
                    {
                        case "int":
                            CSharpString.Append("0");
                            break;
                        case "float":
                            CSharpString.Append("0.0f");
                            break;
                        case "string":
                            CSharpString.Append("\"\"");
                            break;
                        case "bool":
                            CSharpString.Append("false");
                            break;
                        default:
                            CSharpString.Append("new " + node.Type + "_()");
                            break;
                    }
                }
            }
            CSharpString.Append(";");
            PrettyPrintNewLine();
        }

        public override void Visit(IdExpressionNode node)
        {
            CSharpString.Append(node.Id + "_");
            node.IdOperations?.ForEach(x => {
                x.Accept(this);
            });
            
        }

        public override void Visit(IdNode node)
        {
            CSharpString.Append(node.Id + "_");
            node.IdOperations?.ForEach(x => {
                x.Accept(this);
            });
        }

        public override void Visit(IfNode node)
        {
            CSharpString.Append("if (");
            node.ControlExpression.Accept(this);
            CSharpString.Append(") ");
            node.IfBody.Accept(this);
            node.ElifNodes?.ForEach(x => x.Accept(this));
            node.ElseNode?.Accept(this);
        }

        public override void Visit(IntValueNode node)
        {
            CSharpString.Append(node.IntValue.ToString());
        }

        public override void Visit(PlayLoopNode node)
        {
            int local = IndentationLevel;

            CSharpString.Append("{");
            IndentationLevel++;
            CSharpString.Append("List<" + node.Player.Type + "> AllElements" + local +  " = new List<" + node.Player.Type + ">();");
            PrettyPrintNewLine();
            CSharpString.Append(node.Player.Type + " ");
            node.Player.Accept(this);
            CSharpString.Append(";");
            PrettyPrintNewLine();
            CSharpString.Append("List<" + node.Player.Type + ">");
            node.Opponents.Accept(this);
            CSharpString.Append(" = new List<" + node.Player.Type + ">();");
            PrettyPrintNewLine();
            CSharpString.Append("do");
            PrettyPrintNewLine();
            CSharpString.Append("{");
            IndentationLevel++;
            PrettyPrintNewLine();
            CSharpString.Append("AllElements" + local + " = ");
            node.AllPlayers.Accept(this);
            CSharpString.Append(";");
            PrettyPrintNewLine();
            CSharpString.Append("for (int i = 0; i < AllElements" + local + ".Count; i++)");
            PrettyPrintNewLine();
            IndentationLevel++;
            CSharpString.Append("{");
            IndentationLevel++;
            PrettyPrintNewLine();
            node.Player.Accept(this);
            CSharpString.Append(" = AllElements" + local + "[i];");
            PrettyPrintNewLine();
            node.Opponents.Accept(this);
            CSharpString.Append(" = AllElements" + local + ".Where((x, j) => j != i).ToList();");
            node.PlayLoopBody.Accept(this);
            IndentationLevel--;
            CSharpString.Append("}");
            PrettyPrintNewLine();
            IndentationLevel--;
            CSharpString.Append("} while (!(");
            node.UntilCondition.Accept(this);
            CSharpString.Append("));");

            IndentationLevel--;
            CSharpString.Append("}");
            IndentationLevel--;


        }

        public override void Visit(ProgNode node)
        {
            CSharpString.Append("using System;");
            PrettyPrintNewLine();
            CSharpString.Append("using System.Collections.Generic;");
            PrettyPrintNewLine();
            CSharpString.Append("using System.Linq;");
            PrettyPrintNewLine();
            CSharpString.Append("using System.Text;");
            PrettyPrintNewLine();

            CSharpString.Append("public class Program__");
            PrettyPrintNewLine();
            CSharpString.Append("{");
            IndentationLevel++;
            PrettyPrintNewLine();
            GenerateStandardFunctions();
            foreach (TopDclNode DclNode in node.TopDclNodes)
            {
                CSharpString.Append("public ");
                DclNode.Accept(this);
                PrettyPrintNewLine();
            }
            IndentationLevel--;
            PrettyPrintNewLine();
            CSharpString.Append("}");
        }

        public override void Visit(ReturnNode node)
        {
            CSharpString.Append("return ");
            node.ReturnValue.Accept(this);
            CSharpString.Append(";");
        }

        public override void Visit(StringValueNode node)
        {
            CSharpString.Append($"\"{node.StringValue}\"");
        }

        public override void Visit(StructDclNode node)
        {
            CSharpString.Append("class ");
            node.Id.Accept(this);
            CSharpString.Append("{");
            IndentationLevel++;
            PrettyPrintNewLine();
            foreach (DeclarationNode DclNode in node.Declarations)
            {
                CSharpString.Append("public ");
                DclNode.Accept(this);
            }

            if (node.Constructor == null || node.Constructor.FormalParamNodes.Count > 0)
            {
                CSharpString.Append("public " + node.Id.Id + "_()");
                CSharpString.Append("{");
                PrettyPrintNewLine();
                CSharpString.Append("}");
                PrettyPrintNewLine();
            }

            node.Constructor?.Accept(this);
            IndentationLevel--;
            PrettyPrintNewLine();
            CSharpString.Append("}");
        }

        public override void Visit(UnaryExpressionNode node)
        {
            UnaryOperator Operator = node.Operator;
            if (node.Operator != UnaryOperator.DEFAULT)
            {
                switch (Operator)
                {
                    case UnaryOperator.NOT:
                        CSharpString.Append(" !(");
                        break;
                    case UnaryOperator.UNARY_MINUS:
                        CSharpString.Append(" -(");
                        break;
                }
                node.ExprNode.Accept(this);
                CSharpString.Append(")");
            }
            else
            {
                node.ExprNode.Accept(this);
            }
        }

        public override void Visit(WhileNode node)
        {
            CSharpString.Append("while (");
            node.ControlExpr.Accept(this);
            CSharpString.Append(")");
            node.WhileLoopBody.Accept(this);
        }

        public static bool IsPrimitiveType (string type)
        {
            bool isPrimitive = false;
            if (type == "bool" || type == "int" || type == "float" || type == "string")
            {
                isPrimitive = true;
            }
            return isPrimitive;
        }

        private void GenerateStandardFunctions()
        {
            CSharpString.Append(@"public static string GetString_()
    {
        return Console.ReadLine();
    }

    public static float GetNumber_()
    {
        float val;
        while (!float.TryParse(Console.ReadLine(), out val));
    
        return val;
    }

    public static void Print_(object txt)
    {
        Console.Write(txt);
    }

    public static int ChooseOption_(bool displayIndex, params string[] options)
    {
        for (int i = 0; i < options.Length; i++)
        {
            string msg = ((displayIndex == true) ? $""{ i + 1}: "" : """");
            Console.WriteLine(msg + $""{options[i]}"");
        }

        int choice = 0;
        do
        {
            int.TryParse(Console.ReadLine(), out choice);
            if(choice <= 0 || choice > options.Length)
            {
                Console.WriteLine(""Choice out of bounds. Please try again."");
            }
        }
        while (choice <= 0 || choice > options.Length);

        return choice;
    }

    public static float GetRandomFloat_(float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    private static Random random = new Random();

    public static int GetRandomInt_(int min, int max)
    {
        return random.Next(min, max + 1);
    }

    public static void SetSeed_(int seed)
    {
        random = new Random(seed);
    }

    private static void ListAdd_<T>(List<T> lst, T element)
    {
        lst.Add(element);
    }

    private static void ListRemove_<T>(List<T> lst, T element)
    {
        lst.Remove(element);
    }

    private static void ListEmpty_<T>(List<T> lst)
    {
        lst.Clear();
    }

    private static int ListLength_<T>(List<T> lst)
    {
        return lst.Count;
    }
    ");
        }
    }
}
