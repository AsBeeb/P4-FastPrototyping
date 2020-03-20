﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class FloatValueNode : ExpressionNode
    {
        public float FloatValue;

        public FloatValueNode(float floatValue) : base("Float")
        {
            FloatValue = floatValue;
        }
    }
}