using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class FormalParamNode : ASTnode
    {
        public IdNode Id;
        public string Type;
    }
}
