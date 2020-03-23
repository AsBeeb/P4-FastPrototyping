using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace ScannerLib
{
    public enum TokenType
    {
        default_token, plus_token, minus_token, multiply_token, divide_token, modulo_token, power_token, assign_token,
        lessthan_token, greaterthan_token, lessorequal_token, greaterorequal_token, and_token, or_token,
        notequal_token, equal_token, not_token, if_token, elif_token, else_token, while_token, play_token,
        until_token, vs_token, in_token, semicolon_token, colon_token, return_token, intdcl_token, floatdcl_token,
        struct_token, inum_token, fnum_token, lsbracket_token, rsbracket_token, stringdcl_token, stringval_token,
        id_token, booldcl_token, boolval_token, void_token, lparen_token, rparen_token, lcbracket_token,
        rcbracket_token, func_token, local_token, global_token, comment_token, dot_token, comma_token, eof_token
    }

    public class Token
    {
        public string Value;
        public TokenType Type;
        public int Line;
        public Token(string value, TokenType type, int line)
        {
            Value = value;
            Type = type;
            Line = line;
        }

        public Token(TokenType type, int line)
        {
            Type = type;
            Line = line;
        }

        public bool IsInPredictSet(params TokenType[] types)
        {
            return types.Contains(this.Type);
        }
    }
}
