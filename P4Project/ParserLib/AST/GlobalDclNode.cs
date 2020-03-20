using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class GlobalDclNode : TopDclNode
    {
        public IdNode Id;
        public ExpressionNode InitialValue;
        public string Type;

        public GlobalDclNode(IdNode id, ExpressionNode initVal, string type)
        {
            Id = id;
            InitialValue = initVal;
            Type = type;
        }
    }
}
