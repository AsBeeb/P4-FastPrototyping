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

        public StructDclNode(IdNode Id, List<DeclarationNode> Declarations,ConstructorNode Constructor)
        {
            this.Id = Id;
            this.Declarations = Declarations;
            this.Constructor = Constructor;
        }
    }
}
