using System;
using System.Collections.Generic;
using System.IO;

namespace ScannerLib
{
    public static class Scanner
    {
        private static Dictionary<string, Token.TokenType> Keywords = new Dictionary<string, Token.TokenType>
        {
            {"play", Token.TokenType.play_token }, 
            {"if", Token.TokenType.if_token }
        };

        public static Token Scan(StreamReader reader)
        {
            Token ans = null;

            // Advance if blank space
            while (Char.IsWhiteSpace((char)reader.Peek()))
            {
                reader.Read();
            }

            // If end of file, return eof token
            if (reader.EndOfStream)
            {
                ans = new Token(Token.TokenType.eof_token);
            }
            else
            {
                // Scan digits
                if (Char.IsDigit((char)reader.Peek()))
                {
                    ans = ScanDigits(reader);
                }
                // Scan identifiers and keywords
                else if(Char.IsLetter((char)reader.Peek())) 
                {
                    ans = ScanWords(reader);
                }
                else
                {
                    char ch = (char)reader.Read();
                    switch (ch)
                    {
                        // Aritmetiske
                        case '+':
                            ans = new Token(Token.TokenType.plus_token);
                            break;
                        case '-':
                            ans = new Token(Token.TokenType.minus_token);
                            break;
                        case '=':
                            ans = TokenComp(reader, '=', Token.TokenType.assign_token, Token.TokenType.equal_token);
                            break;
                        case '*':
                            ans = new Token(Token.TokenType.multiply_token);
                            break;
                        case '/':
                            ans = new Token(Token.TokenType.divide_token);
                            break;
                        case '%':
                            ans = new Token(Token.TokenType.modulo_token);
                            break;
                        case '^':
                            ans = new Token(Token.TokenType.power_token);
                            break;

                        // Logiske
                        case '!':
                            ans = TokenComp(reader, '=', Token.TokenType.not_token, Token.TokenType.notequal_token);
                            break;
                        case '<':
                            ans = TokenComp(reader, '=', Token.TokenType.lessthan_token, Token.TokenType.lessorequal_token);
                            break;
                        case '>':
                            ans = TokenComp(reader, '=', Token.TokenType.greaterthan_token, Token.TokenType.greaterorequal_token);
                            break;
                        case '&':
                            ans = TokenComp(reader, '&', Token.TokenType.and_token, Token.TokenType.and_token);
                            break;
                        case '|':
                            ans = TokenComp(reader, '|', Token.TokenType.or_token, Token.TokenType.or_token);
                            break;

                        // Kontrolstruktur
                        case ';':
                            ans = new Token(Token.TokenType.semicolon_token);
                            break;
                        case ',':
                            ans = new Token(Token.TokenType.comma_token);
                            break;

                        // Datatyper
                        case '(':
                            ans = new Token(Token.TokenType.lparen_token);
                            break;
                        case ')':
                            ans = new Token(Token.TokenType.rparen_token);
                            break;
                        case '[':
                            ans = new Token(Token.TokenType.lsbracket_token);
                            break;
                        case ']':
                            ans = new Token(Token.TokenType.rsbracket_token);
                            break;
                        case '{':
                            ans = new Token(Token.TokenType.lcbracket_token);
                            break;
                        case '}':
                            ans = new Token(Token.TokenType.rcbracket_token);
                            break;

                        // Misc
                        case '.':
                            ans = new Token(Token.TokenType.dot_token);
                            break;
                        case ':':
                            ans = new Token(Token.TokenType.colon_token);
                            break;
                        case '\"':
                            ans = GetString(reader);
                            break;

                        default:
                            Console.WriteLine("Syntaks fejl: Token ikke fundet");
                            break;
                    }
                }
            }

            return ans;
        }

        private static Token GetString(StreamReader reader)
        {
            string value = "";
            while ((char)reader.Peek() != '\n' && (char)reader.Peek() != '\"' && reader.Peek() > -1)
            {
                value += (char)reader.Read();
            }
            if ((char)reader.Peek() == '\"')
            {
                reader.Read();
                return new Token(value, Token.TokenType.stringval_token);
            }
            else
            {
                // Throw exception
                Console.WriteLine("Syntaks fejl: Fejl i string");
                Console.Read();
                return null;
            }
        }

        private static Token TokenComp(StreamReader reader, char expectedSymbol, Token.TokenType option1, Token.TokenType option2)
        {
            if ((char)reader.Peek() == expectedSymbol)
            {
                reader.Read();
                return new Token(option2);
            }
            // Scenariet ved '&&' og '||'
            else if (option1 == option2)
            {
                // Throw exception
                Console.WriteLine("Syntaks fejl: Forventet && eller ||");
                return null;
            }
            else
            {
                return new Token(option1);
            }
        }

        private static Token ScanDigits(StreamReader reader)
        {
            string value = "";
            Token.TokenType type;
            while (Char.IsDigit((char)reader.Peek()))
            {
                value += (char)reader.Read();
            }

            if ((char)reader.Peek() != '.')
            {
                type = Token.TokenType.inum_token;
            }
            else
            {
                type = Token.TokenType.fnum_token;
                value += (char)reader.Read();

                while (Char.IsDigit((char)reader.Peek()))
                {
                    value += (char)reader.Read();
                }
            }

            return new Token(value, type);
        }

        private static Token ScanWords(StreamReader reader)
        {
            string value = "";
            Token.TokenType type;
            while (Char.IsLetterOrDigit((char)reader.Peek()))
            {
                value += (char)reader.Read();
            }

            if (Keywords.TryGetValue(value, out type))
            {
                return new Token(value, type);
            }
            else
            {
                type = Token.TokenType.id_token;
                return new Token(value, type);
            }
        }
    }
}
