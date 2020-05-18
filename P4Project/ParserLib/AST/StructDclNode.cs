using System.Collections.Generic;

namespace ParserLib.AST
{
    public class StructDclNode : TopDclNode
    {
        public IdNode Id;
        public List<DeclarationNode> Declarations;
        public ConstructorNode Constructor;

        public StructDclNode(IdNode id, List<DeclarationNode> declarations, ConstructorNode constructor)
        {
            this.Id = id;
            this.Declarations = declarations;
            this.Constructor = constructor;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
