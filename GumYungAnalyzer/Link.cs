using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GumYungAnalyzer
{
    class Link
    {
        public List<string> Party1 { get; set; }
        public List<string> Party2 { get; set; }
        public OperatorToken Operator { get; set; }
        public int? Lower { get; set; }
        public int? Higher { get; set; }
        public Certainties Certainty { get; set; }

        public int SourceLineNum { get; set; }

        //public Link()
        //{
        //    Party1 = new List<string>();
        //    Party2 = new List<string>();
        //}

        public Link(List<string> party1, List<string> party2, OperatorToken operatorToken, int? lower, int? higher, Certainties certainty, int sourceLineNum)
        {
            Party1 = party1;
            Party2 = party2;
            Operator = operatorToken;
            Lower = lower;
            Higher = higher;
            Certainty = certainty;
            SourceLineNum = sourceLineNum;
        }

        public override string ToString()
        {
            string s = "";
            foreach (string ss in Party1)
            {
                s += ss + "+";
            }
            s = s.Substring(0, s.Length - 1);

            s += Operator;

            if (Party2 != null)
            {
                foreach (string ss in Party2)
                {
                    s += ss + "+";
                }
                s = s.Substring(0, s.Length - 1);
            }

            if (Lower != null)
            {
                s += " from " + Lower;
            }

            if (Higher != null)
            {
                s += " to " + Higher;
            }

            if (Certainty != Certainties.Certain)
            {
                s += " (" + Certainty + ")";
            }

            s += " - Line " + SourceLineNum;

            return s;
        }

        public bool IsOneOnZero
        {
            get
            {
                return Party2 == null && Party1.Count == 1;
            }
        }
        public bool IsOneOnOne
        {
            get
            {
                return Party1.Count == 1 && Party2 != null && Party2.Count == 1;
            }
        }
    }

    enum Certainties
    {
        Certain = 999, Preferred = 2, Uncertain = 1
    }
}