using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cpuasm
{
    public class Token
    {

    }

    internal class Lexing
    {
        public string[] Keywords = ["movb", "movw", "movd", "movq", "saveb", "savew", "saved", "saveq"];

        public List<Token> Lex(string code)
        {

        }
    }
}
