using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class FunctionDclNode : TopDclNode
    {
        public IdNode Id;
        public string ReturnType;
        public List<FormalParamNode> FormalParamNodes;
        public BlockNode FuncBody;
    }
}
