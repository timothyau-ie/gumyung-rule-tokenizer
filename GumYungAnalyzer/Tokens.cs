using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GumYungAnalyzer
{
    public class Token
    {
        public int Line { get; set; }
        public int Col { get; set; }
    }
    public class NextRowToken : Token
    {
        public override string ToString()
        {
            return ",";
        }
    }
    public class JoiToken : Token
    {
        public override string ToString()
        {
            return "再";
        }
    }
    public class QuestionMarkToken : Token
    {
        public override string ToString()
        {
            return "?";
        }
    }
    public class NumberOperandToken : Token
    {
    }
    public class PlusToken : NumberOperandToken
    {
        public override string ToString()
        {
            return "+";
        }
    }
    public class PlusPlusToken : NumberOperandToken
    {
        public override string ToString()
        {
            return "++";
        }
    }
    public class MinusToken : NumberOperandToken
    {
        public override string ToString()
        {
            return "-";
        }
    }
    public class PlusMinusToken : NumberOperandToken
    {
        public override string ToString()
        {
            return "+-";
        }
    }
    public class OperatorToken : Token
    {
        public virtual OperatorToken GetReverse()
        {
            return this;
        }
    }
    public class TildeToken : OperatorToken
    {
        public override string ToString()
        {
            return "~";
        }
    }
    public class EqualToken : OperatorToken
    {
        public override string ToString()
        {
            return "=";
        }
    }
    public class GreaterThanToken : OperatorToken
    {
        public override string ToString()
        {
            return ">";
        }
        public override OperatorToken GetReverse()
        {
            return new SmallerThanToken();
        }
    }
    public class SmallerThanToken : OperatorToken
    {
        public override string ToString()
        {
            return "<";
        }
        public override OperatorToken GetReverse()
        {
            return new GreaterThanToken();
        }
    }
    public class NumberConstantToken : Token
    {
        public NumberConstantToken(int value)
        {
            _value = value;
        }
        private readonly int _value;

        public int Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
    public class FighterToken : Token
    {
        public FighterToken(string value)
        {
            _value = value;
        }
        private readonly string _value;


        public string Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
