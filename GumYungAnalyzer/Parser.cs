using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GumYungAnalyzer
{
    class Parser
    {
        private readonly List<Token> _tokens;
        private int parsePosition;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public List<Link> Parse()
        {
            parsePosition = 0;
            List<Link> result = ParseRow();
            while (parsePosition < _tokens.Count)
            {
                if (!(_tokens[parsePosition] is NextRowToken))
                {
                    throwMissingException("comma or new line");
                }
                while (_tokens[parsePosition] is NextRowToken)
                {
                    parsePosition++;
                    if (parsePosition >= _tokens.Count)
                    {
                        return result;
                    }
                }
                if (_tokens[parsePosition].Line == 17)//117)
                {
                    //for debugging, set breakpoint
                    int a = 0;
                }
                result.AddRange(ParseRow());
            }

            return result;
        }

        List<Link> ParseRow()
        {
            Party firstParty = ParseParty();
            List<Link> result = null;

            //Row =
            //Party [Operator] RangeWithPrefer |
            //Party Challenger {["再"] Challenger } ["?" | "？"];

            //Operator, Fighter => second option
            //[Operator], [Tilde], Number => first option
            if (parsePosition < _tokens.Count - 1
                && _tokens[parsePosition] is OperatorToken
                && _tokens[parsePosition + 1] is FighterToken)
            {
                result = ParseChallengers(firstParty);
            }
            else
            {
                OperatorToken operatorToken = null;
                if (_tokens[parsePosition] is OperatorToken)
                {
                    operatorToken = (OperatorToken)_tokens[parsePosition];
                    parsePosition++;
                }

                result = ParseRangeWithPrefer(firstParty, operatorToken);
            }

            //if (_tokens[parsePosition] is TildeToken)
            //{
            //    parsePosition++;
            //    if (_tokens[parsePosition] is FighterToken)
            //    {
            //        //tilde as Operator
            //        result = ParseChallengers(true, firstParty);
            //    }
            //    else if (_tokens[parsePosition] is NumberConstantToken)
            //    {
            //        //tilde as Prefer
            //        result = ParseRangeWithPrefer(true, firstParty);
            //    }
            //    else
            //    {
            //        throwException("fighter or number");
            //    }
            //}
            //else if (_tokens[parsePosition] is OperatorToken)
            //{
            //    result = ParseChallengers(false, firstParty);
            //}
            //else
            //{
            //    result = ParseRangeWithPrefer(false, firstParty);
            //}

            if (_tokens[parsePosition] is QuestionMarkToken)
            {
                foreach (Link link in result)
                {
                    link.Certainty = Certainties.Uncertain;
                }
                parsePosition++;
            }

            return result;
        }

        List<Link> ParseChallengers(Party firstParty)
        {
            Party challenged = firstParty;
            Party challenger;
            List<Link> result = ParseChallenger(challenged, out challenger);
            while (true)
            {
                if (_tokens[parsePosition] is JoiToken)
                {
                    parsePosition++;
                    challenged = firstParty;
                }
                else if (!(_tokens[parsePosition] is OperatorToken))
                {
                    return result;
                }
                else
                {
                    challenged = challenger;
                }
                result.AddRange(ParseChallenger(challenged, out challenger));
            }
        }

        List<Link> ParseChallenger(Party challenged, out Party challenger)
        {
            OperatorToken operatorToken = null;

            if (_tokens[parsePosition] is OperatorToken)
            {
                operatorToken = (OperatorToken)_tokens[parsePosition];
                parsePosition++;
            }
            else
            {
                throwMissingException("operator");
            }

            challenger = ParseParty();

            if (ToParseRange())
            {
                return ParseRangeWithPrefer(challenged, operatorToken, challenger);
            }
            else
            {
                return GenerateLinks(operatorToken, null, null, false, challenged, challenger, false);
                //return GenerateLinks1(challenger, operatorToken, challenged, 0, null, Certainties.Certain);
            }
        }

        bool ToParseRange()
        {
            if (_tokens[parsePosition] is NumberConstantToken)
            {
                return true;
            }
            if (parsePosition < _tokens.Count - 1 &&
                (_tokens[parsePosition] is TildeToken || _tokens[parsePosition] is MinusToken)
                && _tokens[parsePosition + 1] is NumberConstantToken)
            {
                return true;
            }
            if (parsePosition < _tokens.Count - 2 &&
                _tokens[parsePosition] is TildeToken
                && _tokens[parsePosition + 1] is MinusToken
                && _tokens[parsePosition + 2] is NumberConstantToken)
            {
                return true;
            }
            return false;
        }

        List<Link> ParseRangeWithPrefer(Party challenged, OperatorToken operatorToken = null, Party challenger = null)
        {
            List<Link> result = new List<Link>();
            result.AddRange(ParseRange(challenged, operatorToken, challenger));
            while (true)
            {
                if (!ToParseRange())
                {
                    return result;
                }

                result.AddRange(ParseRange(challenged, operatorToken, challenger));
            }
        }

        List<Link> ParseRange(Party challenged, OperatorToken operatorToken, Party challenger)
        {
            bool hasTilde = false;
            if (_tokens[parsePosition] is TildeToken)
            {
                hasTilde = true;
                parsePosition++;
            }

            if (!(_tokens[parsePosition] is NumberConstantToken || _tokens[parsePosition] is MinusToken))
            {
                throwMissingException("number or negative sign");
            }

            bool isNegative = false;
            if (_tokens[parsePosition] is MinusToken)
            {
                isNegative = true;
                parsePosition++;
            }

            if (!(_tokens[parsePosition] is NumberConstantToken))
            {
                throwMissingException("number");
            }

            int number1 = ((NumberConstantToken)_tokens[parsePosition]).Value;
            parsePosition++;
            if (isNegative)
            {
                number1 = -number1;
            }

            int? lower = null;
            int? higher = null;
            bool hasPlusPlus = false;

            if (!isNegative &&
                (_tokens[parsePosition] is PlusToken
                || _tokens[parsePosition] is PlusPlusToken
                || _tokens[parsePosition] is PlusMinusToken))
            {
                if (_tokens[parsePosition] is PlusMinusToken)
                {
                    lower = -number1;
                    higher = number1;
                }
                else
                {
                    lower = number1;
                    if (_tokens[parsePosition] is PlusPlusToken)
                    {
                        hasPlusPlus = true;
                    }
                }
                parsePosition++;
            }
            else if (!(_tokens[parsePosition] is MinusToken))
            {
                lower = number1;
                higher = number1;
            }
            else
            {
                //is MinusToken
                parsePosition++;
                if (_tokens[parsePosition] is MinusToken || _tokens[parsePosition] is NumberConstantToken)
                {
                    isNegative = false;
                    if (_tokens[parsePosition] is MinusToken)
                    {
                        isNegative = true;
                        parsePosition++;
                        if (!(_tokens[parsePosition] is NumberConstantToken))
                        {
                            throwMissingException("number");
                        }
                    }

                    int number2 = ((NumberConstantToken)_tokens[parsePosition]).Value;
                    parsePosition++;
                    if (isNegative)
                    {
                        number2 = -number2;
                    }
                    if (number1 == number2)
                    {
                        throwMissingException("different number");
                    }

                    lower = number1 > number2 ? number2 : number1;
                    higher = number1 > number2 ? number1 : number2;
                }
                else
                {
                    higher = number1;
                    lower = 1;
                }
            }

            return GenerateLinks(operatorToken, lower, higher, hasTilde, challenged, challenger, hasPlusPlus);
        }

        List<Link> GenerateLinks(OperatorToken operatorToken, int? lower, int? higher, bool hasTilde,
            Party challenged, Party challenger, bool hasPlusPlus)
        {
            List<Link> results = new List<Link>();

            if (operatorToken is TildeToken)
            {
                operatorToken = new EqualToken();
                if (lower.HasValue || higher.HasValue)
                {
                    hasTilde = true;
                }
                else
                {
                    lower = -1;
                    higher = 1;
                }
            }
            else if (operatorToken == null)
            {
                operatorToken = new EqualToken();
            }
            else if (operatorToken is GreaterThanToken || operatorToken is SmallerThanToken)
            {
                if (!lower.HasValue && !higher.HasValue)
                {
                    lower = 1;
                }
            }

            if (operatorToken is EqualToken && challenger != null)
            {
                if (!lower.HasValue && !higher.HasValue)
                {
                    lower = 0;
                    higher = 0;
                }
                else if (lower != -higher || !lower.HasValue || !higher.HasValue)
                {
                    throwException("Equal sign cannot come with a range where lower limit is the negative of higher limit.");
                }
            }

            results.AddRange(GenerateLinks1(challenger, operatorToken, challenged, lower, higher, 
                hasTilde ? Certainties.Preferred : Certainties.Certain));

            if (hasPlusPlus)
            {
                results.AddRange(GenerateLinks1(challenger, operatorToken, challenged, lower + 2, higher, Certainties.Preferred));
                results.AddRange(GenerateLinks1(challenger, operatorToken, challenged, lower + 4, higher, Certainties.Uncertain));
            }

            return results;
        }


        List<Link> GenerateLinks1(Party challenger, OperatorToken operatorToken, Party challenged, 
            int? lower, int? higher, Certainties certainty)
        {
            int sourceLineNum = _tokens[parsePosition].Line;
            List<Link> result = new List<Link>();
            bool challengedNeedMultipleLinks = challenged.Fighters.Count > 1 && !challenged.IsCooperate;
            bool? challengerNeedMultipleLinks = challenger == null ? null : 
                (bool?)(challenger.Fighters.Count > 1 && !challenger.IsCooperate);

            if (challengedNeedMultipleLinks && challengerNeedMultipleLinks.HasValue && challengerNeedMultipleLinks.Value)
            {
                foreach (string challengedFighter in challenged.Fighters)
                {
                    foreach (string challengerFighter in challenger.Fighters)
                    {
                        result.Add(new Link(new List<string> { challengedFighter }, new List<string> { challengerFighter }, 
                            operatorToken, lower, higher, certainty, sourceLineNum));
                    }
                }
            }
            else if (challengedNeedMultipleLinks)
            {
                foreach (string challengedFighter in challenged.Fighters)
                {
                    result.Add(new Link(new List<string> { challengedFighter }, challenger == null ? null : challenger.Fighters, 
                        operatorToken, lower, higher, certainty, sourceLineNum));
                }
            }
            else if (challengerNeedMultipleLinks.HasValue && challengerNeedMultipleLinks.Value)
            {
                foreach (string challengerFighter in challenger.Fighters)
                {
                    result.Add(new Link(challenged.Fighters, new List<string> { challengerFighter },
                        operatorToken, lower, higher, certainty, sourceLineNum));
                }
            }
            else
            {
                result.Add(new Link(challenged.Fighters, challenger == null ? null : challenger.Fighters,
                    operatorToken, lower, higher, certainty, sourceLineNum));
            }

            return result;
        }

        Party ParseParty()
        {
            if (_tokens[parsePosition] is FighterToken)
            {
                Party party = new Party();

                party.Fighters.Add(((FighterToken)_tokens[parsePosition]).Value);
                parsePosition++;

                if (_tokens[parsePosition] is PlusToken)
                {
                    party.IsCooperate = true;
                    while (_tokens[parsePosition] is PlusToken)
                    {
                        parsePosition++;
                        if (!(_tokens[parsePosition] is FighterToken))
                        {
                            throwMissingException("fighter");
                        }

                        party.Fighters.Add(((FighterToken)_tokens[parsePosition]).Value);
                        parsePosition++;
                    }
                }
                else if (_tokens[parsePosition] is FighterToken)
                {
                    party.IsCooperate = false;
                    while (_tokens[parsePosition] is FighterToken)
                    {

                        party.Fighters.Add(((FighterToken)_tokens[parsePosition]).Value);
                        parsePosition++;
                    }
                }
                return party;
            }
            else
            {
                throwMissingException("fighter");
                return null;
            }
        }

        private void throwMissingException(string missingThing)
        {
            Token token = _tokens[parsePosition];
            throwException(String.Format("Expected {0}, but got \"{1}\"", missingThing, token));
        }
        private void throwException(string message)
        {
            Token token = _tokens[parsePosition];
            throw new Exception(String.Format("Ln {0} Col {1}: {2}", token.Line, token.Col, message));
        }
    }

    class Party
    {
        public List<string> Fighters { get; set; }
        public bool IsCooperate { get; set; }
        public Party()
        {
            Fighters = new List<string>();
        }
    }
}