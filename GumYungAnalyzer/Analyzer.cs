using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GumYungAnalyzer
{
    class Analyzer
    {
        List<Link> _links;
        List<Link> _absoluteCertainLinks;
        List<Link> _relativeCertainLinks;
        List<Link> _certainToPreferredLinks;
        List<Link> _certainToUncertainLinks;
        ResultSet _baseSet;
        bool _hasNew;
        int _lastStepCounter;
        int _counter;
        public Analyzer(List<Link> links, string[] lines)
        {
            _links = links;
            AnalyzerState = AnalyzerStates.AbsoluteCertain;
            _baseSet = new ResultSet();
            _hasNew = false;
            _absoluteCertainLinks = _links.Where(l => l.Certainty == Certainties.Certain && l.IsOneOnZero).ToList();
            _relativeCertainLinks = _links.Where(l => l.Certainty == Certainties.Certain && l.IsOneOnOne).ToList();
            _certainToPreferredLinks = _links.Where(l => (l.Certainty >= Certainties.Preferred) && l.IsOneOnZero).ToList();
            _certainToPreferredLinks.AddRange(_links.Where(l => l.Certainty >= Certainties.Preferred && l.IsOneOnOne).ToArray());
            _certainToUncertainLinks = _links.Where(l => l.Certainty >= Certainties.Uncertain && l.IsOneOnZero).ToList();
            _certainToUncertainLinks.AddRange(_links.Where(l => l.Certainty >= Certainties.Uncertain && l.IsOneOnOne).ToArray());
            Counter = -1;
            Counter = _absoluteCertainLinks.Count - 1;
        }

        public AnalyzerStates AnalyzerState { get; set; }
        public List<Link> LinksToShow
        {
            get
            {
                return AnalyzerState == AnalyzerStates.AbsoluteCertain ?
                _absoluteCertainLinks : AnalyzerState == AnalyzerStates.RelativeCertain ?
                _relativeCertainLinks : AnalyzerState == AnalyzerStates.Preferred ?
                _certainToPreferredLinks : AnalyzerState == AnalyzerStates.Uncertain ?
                _certainToUncertainLinks : AnalyzerState == AnalyzerStates.TempState ?
                _certainToUncertainLinks : null;
            }
        }
        public ResultSet BaseSet
        {
            get
            {
                return _baseSet;
            }
        }
        public int LastStepCounter
        {
            get
            {
                return _lastStepCounter;
            }
        }
        private int Counter
        {
            get
            {
                return _counter;
            }
            set
            {
                _lastStepCounter = _counter;
                _counter = value;
            }
        }
        public TryResults LastTryResult { get; set; }


        public void Step()
        {
            _baseSet.ClearChangeHistory();
            if (AnalyzerState == AnalyzerStates.AbsoluteCertain)
            {
                LastTryResult = TryWhatever(ref _baseSet, _absoluteCertainLinks[Counter]);
                if (LastTryResult == TryResults.Added)
                {
                    _absoluteCertainLinks.RemoveAt(Counter);
                    _hasNew = true;
                }
                else if (LastTryResult == TryResults.Clashed)
                {
                    throw new Exception("Clashed during creation of base set!");
                }


                Counter--;

                if (Counter < 0)
                {
                    if (!_hasNew || _absoluteCertainLinks.Count == 0)
                    {
                        _hasNew = true;
                        Counter = 0;
                        AnalyzerState = AnalyzerStates.RelativeCertain;
                    }
                    else
                    {
                        Counter = _absoluteCertainLinks.Count - 1;
                        _hasNew = false;
                    }
                }
            }
            else if (AnalyzerState == AnalyzerStates.RelativeCertain)
            {
                LastTryResult = TryWhatever(ref _baseSet, _relativeCertainLinks[Counter]);
                if (LastTryResult == TryResults.Added)
                {
                    _hasNew = true;
                }
                else if (LastTryResult == TryResults.Clashed)
                {
                    throw new Exception("Clashed during creation of base set!");
                }

                Counter++;

                if (Counter == _relativeCertainLinks.Count)
                {
                    if (!_hasNew)
                    {
                        _hasNew = true;
                        Counter = 0;
                        AnalyzerState = AnalyzerStates.TempState;
                    }
                    else
                    {
                        Counter = 0;
                        _hasNew = false;
                    }
                }
            }
            else if (AnalyzerState == AnalyzerStates.TempState)
            {
                LastTryResult = TryWhatever(ref _baseSet, _certainToUncertainLinks[Counter]);
                if (LastTryResult == TryResults.Added)
                {
                    _hasNew = true;
                }
                else if (LastTryResult == TryResults.Clashed)
                {
                    //ignore
                }

                Counter++;

                if (Counter == _certainToUncertainLinks.Count)
                {
                    if (!_hasNew)
                    {
                        AnalyzerState = AnalyzerStates.Finish;
                    }
                    else
                    {
                        Counter = 0;
                        _hasNew = false;
                    }
                }
            }
        }


        TryResults TryOneOnZero(ref ResultSet resultSet, Link link)
        {
            if (!link.IsOneOnZero)
            {
                return TryResults.Nothing;
            }

            if (link.Operator is EqualToken)
            {
                return Try(ref resultSet, link.Higher, link.Lower, link.Party1[0], link);
            }
            else if (link.Operator is GreaterThanToken)
            {
                return Try(ref resultSet, null, IntOperation.Max(link.Higher, link.Lower), link.Party1[0], link);
            }
            else if (link.Operator is SmallerThanToken)
            {
                return Try(ref resultSet, IntOperation.Min(link.Higher, link.Lower), 0, link.Party1[0], link);
            }

            return TryResults.Nothing;
        }

        TryResults TryWhatever(ref ResultSet resultSet, Link link)
        {
            if (link.IsOneOnOne)
            {
                return TryOneOnOne(ref resultSet, link);
            }
            else if (link.IsOneOnZero)
            {
                return TryOneOnZero(ref resultSet, link);
            }
            return TryResults.Nothing;
        }

        TryResults TryOneOnOne(ref ResultSet resultSet, Link link)
        {
            if (!link.IsOneOnOne)
            {
                return TryResults.Nothing;
            }
            TryResults tryResult1 = TryOneOnOne1(ref resultSet, link, false);
            TryResults tryResult2 = TryOneOnOne1(ref resultSet, link, true);
            if (tryResult1 == TryResults.Clashed || tryResult2 == TryResults.Clashed)
            {
                return TryResults.Clashed;
            }

            if (tryResult1 == TryResults.Added || tryResult2 == TryResults.Added)
            {
                tryResult1 = TryOneOnOne1(ref resultSet, link, false);
                tryResult2 = TryOneOnOne1(ref resultSet, link, true);
                if (tryResult1 == TryResults.Clashed || tryResult2 == TryResults.Clashed)
                {
                    return TryResults.Clashed;
                }
                return TryResults.Added;
            }

            return TryResults.Nothing;
        }

        TryResults TryOneOnOne1(ref ResultSet resultSet, Link link, bool reversed)
        {

            //if A =>< B and A is of certain range, B's range can be deduced A's range & =><
            string knownFighter = null;
            string unknownFighter = null;
            OperatorToken operatorToken = null;
            if (reversed)
            {
                knownFighter = link.Party2[0];
                unknownFighter = link.Party1[0];
                operatorToken = link.Operator.GetReverse();
            }
            else
            {
                knownFighter = link.Party1[0];
                unknownFighter = link.Party2[0];
                operatorToken = link.Operator;
            }
            if (!resultSet.ContainsFighter(knownFighter))
            {
                return TryResults.Nothing;
            }

            FighterResult knownFighterResult = resultSet.GetFighterResult(knownFighter);
            if (operatorToken is GreaterThanToken)
            {
                //if (knownFighterResult.Higher.HasValue && link.Lower.HasValue)
                //{
                //    int a = 0;
                //}

                //H(?) = H(k) - L(>)
                //L(?) = L(k) - H(>)
                return Try(ref resultSet, IntOperation.Minus(knownFighterResult.Higher, link.Lower),
                     IntOperation.Minus(knownFighterResult.Lower, link.Higher), unknownFighter, link);
            }
            else if (operatorToken is SmallerThanToken || operatorToken is EqualToken)
            {
                if (knownFighterResult.Higher.HasValue && link.Higher.HasValue && operatorToken is EqualToken)
                {
                    int a = 0;
                }

                //H(?) = H(k) + H(< / =)
                //L(?) = L(k) + L(< / =)
                return Try(ref resultSet, IntOperation.Plus(knownFighterResult.Higher, link.Higher),
                    IntOperation.Plus(knownFighterResult.Lower, link.Lower), unknownFighter, link);
            }
            else
            {
                throw new Exception("Operator not included!");
            }
        }



        TryResults Try(ref ResultSet resultSet, int? higher, int? lower, string fighter, Link link)
        {
            int? higherNow = null;
            int? lowerNow = null;

            if (resultSet.ContainsFighter(fighter))
            {
                higherNow = resultSet.GetFighterResult(fighter).Higher;
                lowerNow = resultSet.GetFighterResult(fighter).Lower;
            }

            TryResults result = TryResults.Nothing;
            if ((higherNow.HasValue && higher.HasValue && higher < higherNow)
                || !higherNow.HasValue && higher.HasValue)
            {
                higherNow = higher;
                result = TryResults.Added;
            }
            if ((lowerNow.HasValue && lower.HasValue && lower > 0 && lower > lowerNow)
                || !lowerNow.HasValue && lower.HasValue)
            {
                lowerNow = lower < 0 ? 0 : lower;
                result = TryResults.Added;
            }

            if (!lowerNow.HasValue)
            {
                lowerNow = 0;
                result = TryResults.Added;
            }

            if (higherNow < lowerNow)
            {
                result = TryResults.Clashed;
            }

            if (result == TryResults.Added)
            {
                FighterResult fighterResult;
                if (!resultSet.ContainsFighter(fighter))
                {
                    fighterResult = new FighterResult();
                }
                else
                {
                    fighterResult = resultSet.GetFighterResult(fighter).Clone();
                }
                fighterResult.Higher = higherNow;
                fighterResult.Lower = lowerNow;
                fighterResult.LinkHistory.Add(link);
                resultSet.AddOrUpdateFighterResult(fighter, fighterResult);
            }
            return result;

        }
    }

    enum TryResults
    {
        Nothing, Added, Clashed
    }

    class IntOperation
    {
        public static int? Plus(int? a, int? b)
        {
            return a.HasValue && b.HasValue ? a + b : null;
        }
        public static int? Minus(int? a, int? b)
        {
            return a.HasValue && b.HasValue ? a - b : null;
        }
        public static int? Max(int? a, int? b)
        {
            if (!a.HasValue) return b;
            if (!b.HasValue) return a;
            if (a > b) return a;
            return b;
        }
        public static int? Min(int? a, int? b)
        {
            if (!a.HasValue) return b;
            if (!b.HasValue) return a;
            if (a < b) return a;
            return b;
        }
    }

    class ResultSet
    {
        private Dictionary<string, FighterResult> FighterResults { get; set; }
        public Dictionary<string, FighterResult> ChangeHistory { get; set; }

        public List<string> FighterKeys
        {
            get
            {
                return FighterResults.Keys.ToList();
            }
        }

        public ResultSet()
        {
            FighterResults = new Dictionary<string, FighterResult>();
            ChangeHistory = new Dictionary<string, FighterResult>();
        }

        public bool ContainsFighter(string fighter)
        {
            return FighterResults.ContainsKey(fighter);
        }

        public FighterResult GetFighterResult(string fighter)
        {
            return FighterResults[fighter];
        }

        public void AddOrUpdateFighterResult(string fighter, FighterResult fighterResult)
        {
            if (!ChangeHistory.ContainsKey(fighter))
            {
                ChangeHistory.Add(fighter, null);
            }

            if (FighterResults.ContainsKey(fighter))
            {
                FighterResults[fighter] = fighterResult;
            }
            else
            {
                FighterResults.Add(fighter, fighterResult);
            }
        }

        public void ClearChangeHistory()
        {
            ChangeHistory.Clear();
        }
    }

    class FighterResult
    {
        //public string Fighter { get; set; }
        public int? Higher { get; set; }
        public int? Lower { get; set; }
        public List<Link> LinkHistory { get; set; }

        public FighterResult()//(string fighter)
        {
            //Fighter = fighter;
            LinkHistory = new List<Link>();
        }

        public FighterResult(int? higher, int? lower, List<Link> linkHistory)
        {
            Higher = higher;
            Lower = lower;
            LinkHistory = linkHistory;
            if (LinkHistory == null)
            {
                LinkHistory = new List<Link>();
            }
        }

        public override string ToString()
        {
            if (!Higher.HasValue && !Lower.HasValue)
            {
                return "";
            }
            if (!Higher.HasValue)
            {
                return ">=" + Lower;
            }
            if (!Lower.HasValue)
            {
                return "<=" + Higher;
            }
            if (Lower == Higher)
            {
                return Lower.ToString();
            }
            return Lower + "-" + Higher;
        }

        public FighterResult Clone()
        {
            return new FighterResult(Higher, Lower, LinkHistory.ToList());
        }
    }

    public enum AnalyzerStates
    {
        AbsoluteCertain, RelativeCertain, Preferred, Uncertain,
        Finish, TempState
    }
}