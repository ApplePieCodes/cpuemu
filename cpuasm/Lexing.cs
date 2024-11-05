using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cpuasm
{
    public enum TokenType
    {
        Identifier,
        IntegerLiteral,
        HexLiteral
    }

    public class Token
    {
        public TokenType Type;
        public string Value;
    }

    internal class Lexing
    {
        public string[] Keywords = ["movb", "movw", "movd", "movq", "saveb", "savew", "saved", "saveq", "loadb", "loadw", "loadd", "loadq", "inc", "dec"];

        public List<Token> Lex(string code)
        {
            List<Token> tokens = [];
            StringBuilder buffer = new();

            for (int i = 0; i < code.Length; i++)
            {
                if (char.IsLetter(code[i]))
                {
                    buffer.Append(code[i]);
                    i++;
                    while (char.IsLetterOrDigit(code[i]))
                    {
                        buffer.Append(code[i]);
                        i++;
                    }
                    
                }

                buffer.Clear();
            }

            return 
        }
    }
}
