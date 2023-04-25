using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GumYungAnalyzer
{
    class Tokenizer
    {

        private StringReader _reader;
        int line = 1;
        int col = 1;
        List<Token> tokens;

        public List<Token> Scan(string expression)
        {
            _reader = new StringReader(expression);
            line = 1;
            col = 1;

            tokens = new List<Token>();
            string fighterName = "";
            while (_reader.Peek() != -1)
            {
                var c = (char)_reader.Peek();
                if (c == ' ' || c == '\r')
                {
                    CheckFighter(ref tokens, ref fighterName);
                    readerRead();
                }
                else if (Char.IsDigit(c))
                {
                    CheckFighter(ref tokens, ref fighterName);
                    var nr = ParseNumber();
                    tokensAdd(new NumberConstantToken(nr));
                }
                //else if (c == '\r' && (char)_reader.Peek() == '\n')
                //{   
                //    tokens.Add(new NextRowToken());
                //    _reader.Read();
                //    _reader.Read();
                //}
                else if (c == ',' || c == '，' || c == '\n')
                {
                    CheckFighter(ref tokens, ref fighterName);
                    tokensAdd(new NextRowToken());
                    readerRead();
                    if (c == '\n')
                    {
                        line++;
                        col = 1;
                    }
                }
                else if (c == '再')
                {
                    CheckFighter(ref tokens, ref fighterName);
                    tokensAdd(new JoiToken());
                    readerRead();
                }
                else if (c == '?' || c == '？')
                {
                    CheckFighter(ref tokens, ref fighterName);
                    tokensAdd(new QuestionMarkToken());
                    readerRead();
                }
                else if (c == '+')
                {
                    readerRead();
                    CheckFighter(ref tokens, ref fighterName);
                    if ((char)_reader.Peek() == '+')
                    {
                        tokensAdd(new PlusPlusToken());
                        readerRead();
                    }
                    else if ((char)_reader.Peek() == '-')
                    {
                        tokensAdd(new PlusMinusToken());
                        readerRead();
                    }
                    else
                    {
                        tokensAdd(new PlusToken());
                    }
                }
                else if (c == '-')
                {
                    readerRead();
                    CheckFighter(ref tokens, ref fighterName);
                    if ((char)_reader.Peek() == '-')
                    {
                        readerRead();
                        while (_reader.Peek() != -1)
                        {
                            char c1 = (char)_reader.Peek();
                            if (c1 == '\n')
                            {
                                break;
                            }
                            readerRead();
                        }
                    }
                    else
                    {
                        tokensAdd(new MinusToken());
                    }
                }
                else if (c == '~')
                {
                    CheckFighter(ref tokens, ref fighterName);
                    tokensAdd(new TildeToken());
                    readerRead();
                }
                else if (c == '=')
                {
                    CheckFighter(ref tokens, ref fighterName);
                    tokensAdd(new EqualToken());
                    readerRead();
                }
                else if (c == '>')
                {
                    CheckFighter(ref tokens, ref fighterName);
                    tokensAdd(new GreaterThanToken());
                    readerRead();
                }
                else if (c == '<')
                {
                    CheckFighter(ref tokens, ref fighterName);
                    tokensAdd(new SmallerThanToken());
                    readerRead();
                }
                else
                {
                    fighterName += c;
                    readerRead();
                }
            }
            CheckFighter(ref tokens, ref fighterName);
            tokensAdd(new NextRowToken());
            return tokens;
        }

        private void tokensAdd(Token token)
        {
            token.Line = line;
            token.Col = col;
            tokens.Add(token);
        }

        private int readerRead()
        {
            col++;
            return _reader.Read();
        }

        private void CheckFighter(ref List<Token> tokens, ref string fighterName)
        {
            if (!String.IsNullOrWhiteSpace(fighterName))
            {
                tokensAdd(new FighterToken(fighterName));
                fighterName = "";
            }
        }

        private int ParseNumber()
        {
            var digits = new List<int>();
            while (Char.IsDigit((char)_reader.Peek()))
            {
                var digit = (char)readerRead();
                int i;
                if (int.TryParse(Char.ToString(digit), out i))
                {
                    digits.Add(i);
                }
                else
                    throw new Exception("Could not parse integer number when parsing digit: " + digit);
            }

            var nr = 0;
            var mul = 1;
            digits.Reverse();
            digits.ForEach(d =>
            {
                nr += d * mul;
                mul *= 10;
            });

            return nr;
        }

    }
}
