using System;
using System.Collections.Generic;
using System.IO;

namespace ScannerLib
{
    public static class Scanner
    {
        public static int line = 1;
        //Specifies all the keyword tokens
        private static Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"if", TokenType.if_token},
            {"else", TokenType.else_token},
            {"elif", TokenType.elif_token},
            {"while", TokenType.while_token},
            {"play", TokenType.play_token},
            {"until", TokenType.until_token},
            {"vs", TokenType.vs_token},
            {"in", TokenType.in_token},
            {"return", TokenType.return_token},
            {"int", TokenType.intdcl_token},
            {"float", TokenType.floatdcl_token},
            {"struct", TokenType.struct_token},
            {"string", TokenType.stringdcl_token},
            {"bool", TokenType.booldcl_token},
            {"true", TokenType.boolval_token}, // OBS: true and false map to the same token type.
            {"false", TokenType.boolval_token},
            {"void", TokenType.void_token},
            {"func", TokenType.func_token},
            {"local", TokenType.local_token},
            {"global", TokenType.global_token}
        };

        public static Token Scan(StreamReader reader)
        {
            Token ans = null;

            // Advance if blank space
            while (Char.IsWhiteSpace((char)reader.Peek()))
            {
                //Counts the line number for the exception message
                if ((char)reader.Peek() == '\n')
                    line += 1;
                reader.Read();

            }

            // If end of file, return eof token
            if (reader.EndOfStream)
            {
                ans = new Token(TokenType.eof_token);
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
                            ans = new Token(TokenType.plus_token);
                            break;
                        case '-':
                            ans = new Token(TokenType.minus_token);
                            break;
                        case '=':
                            ans = TokenComp(reader, '=', TokenType.assign_token, TokenType.equal_token);
                            break;
                        case '*':
                            ans = new Token(TokenType.multiply_token);
                            break;
                        case '/':
                            ans = new Token(TokenType.divide_token);
                            break;
                        case '%':
                            ans = new Token(TokenType.modulo_token);
                            break;
                        case '^':
                            ans = new Token(TokenType.power_token);
                            break;

                        // Logiske
                        case '!':
                            ans = TokenComp(reader, '=', TokenType.not_token, TokenType.notequal_token);
                            break;
                        case '<':
                            ans = TokenComp(reader, '=', TokenType.lessthan_token, TokenType.lessorequal_token);
                            break;
                        case '>':
                            ans = TokenComp(reader, '=', TokenType.greaterthan_token, TokenType.greaterorequal_token);
                            break;
                        case '&':
                            ans = TokenComp(reader, '&', TokenType.and_token, TokenType.and_token);
                            break;
                        case '|':
                            ans = TokenComp(reader, '|', TokenType.or_token, TokenType.or_token);
                            break;

                        // Kontrolstruktur
                        case ';':
                            ans = new Token(TokenType.semicolon_token);
                            break;
                        case ',':
                            ans = new Token(TokenType.comma_token);
                            break;

                        // Datatyper
                        case '(':
                            ans = new Token(TokenType.lparen_token);
                            break;
                        case ')':
                            ans = new Token(TokenType.rparen_token);
                            break;
                        case '[':
                            ans = new Token(TokenType.lsbracket_token);
                            break;
                        case ']':
                            ans = new Token(TokenType.rsbracket_token);
                            break;
                        case '{':
                            ans = new Token(TokenType.lcbracket_token);
                            break;
                        case '}':
                            ans = new Token(TokenType.rcbracket_token);
                            break;

                        // Diverse
                        case '.':
                            ans = new Token(TokenType.dot_token);
                            break;
                        case ':':
                            ans = new Token(TokenType.colon_token);
                            break;
                        case '\"':
                            ans = GetString(reader);
                            break;
                        case '#':
                            SkipComment(reader);
                            break;

                        default:
                            throw new LexicalException(line);
                    }
                }
            }

            return ans;
        }

        private static void SkipComment(StreamReader reader) 
        {
            //Reads until the symbol right before the end of the line or file
            while ((char)reader.Peek() != '\n' && reader.Peek() > -1)
            {
                reader.Read();
            }

        }

        private static Token GetString(StreamReader reader)
        {
            string value = "";
            // Reads every character until it meets a new line symbol, or " symbol or EOF.
            while ((char)reader.Peek() != '\n' && (char)reader.Peek() != '\"' && reader.Peek() > -1)
            {
                value += (char)reader.Read();
            }
            // Checks if the cause of the stopped while loop is a " symbol.
            if ((char)reader.Peek() == '\"')
            {
                reader.Read();
                return new Token(value, TokenType.stringval_token);
            }
            // Throw exception because of EOF or runaway string.
            else
            {
                throw new LexicalException(line);
            }
        }

        private static Token TokenComp(StreamReader reader, char expectedSymbol, TokenType option1, TokenType option2)
        {
            // Checks whether the symbol is the one expected.
            if ((char)reader.Peek() == expectedSymbol)
            {
                reader.Read();
                return new Token(option2);
            }
            // If it isn't the expected symbol and the options are the same it throws an exception. 
            // This could be if we expect && but only read &, then the second one would throw the exception.
            else if (option1 == option2)
            {
                throw new LexicalException(line);
            }
            else
            {
                return new Token(option1);
            }
        }

        private static Token ScanDigits(StreamReader reader)
        {
            string value = "";
            TokenType type;
            // Reads until it meets a symbol that isn't a digit.
            while (Char.IsDigit((char)reader.Peek()))
            {
                value += (char)reader.Read();
            }
            // Checks whether the next symbol is a dot, if it isn't, the number is an integer.
            if ((char)reader.Peek() != '.')
            {
                type = TokenType.inum_token;
            }
            // If the next symbol was a dot the number is a float and we read the dot + a string of digits after.
            else
            {
                type = TokenType.fnum_token;
                value += (char)reader.Read();
                // If there is no digits after the dot we throw an exception.
                if (!Char.IsDigit((char)reader.Peek()))
                {
                    throw new LexicalException(line);
                }
                    
                // Reads the digits after the dot.
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
            TokenType type;
            // Reads all letters or digits until a nonletter and nondigit is read.
            while (Char.IsLetterOrDigit((char)reader.Peek()))
            {
                value += (char)reader.Read();
            }

            /* Check for reserved keywords. In this case, only "true" and "false" are saved as values.
            Other values, such as "if" and "play", are always the same, and are therefore discarded to save space. */
            if (Keywords.TryGetValue(value, out type))
            {
                return (type == TokenType.boolval_token) ? new Token(value, type) : new Token(type);
            }
            // Alternatively, the word is saved as an identifier token.
            else
            {
                type = TokenType.id_token;
                return new Token(value, type);
            }
        }
    }
}
