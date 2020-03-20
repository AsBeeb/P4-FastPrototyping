using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class StructDclNode : TopDclNode
    {
        public IdNode Id;
        public List<DeclarationNode> Declarations;
        public ConstructorNode Constructor;
    }
}
