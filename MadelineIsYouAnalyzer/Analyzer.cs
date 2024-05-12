#pragma warning disable IDE0028
#nullable disable

using System.Diagnostics;
using IntPair = (int x, int y);

namespace Celeste.Mod.MadelineIsYou.Analyze;

public sealed partial class Analyzer
{
    public const string WordAnd = "and";
    public const string WordNot = "not";

    private readonly HashSet<string> nouns;
    private readonly HashSet<string> conds;
    private readonly HashSet<string> qlts;
    private readonly HashSet<string> verbs;

    public Analyzer()
    {
        nouns = new(4);
        qlts = new(4);
        verbs = new(4);
        conds = new(4);
    }

    public void RegisterCondition(string cond) => conds.Add(cond);
    public void RegisterNoun(string noun) => nouns.Add(noun);
    public void RegisterQuality(string qlt) => qlts.Add(qlt);
    public void RegisterVerb(string verb) => verbs.Add(verb);
    private bool IsNoun(string word) => nouns.Contains(word);
    private bool IsQlt(string word) => qlts.Contains(word);
    private bool IsVerb(string word) => verbs.Contains(word);
    private bool IsCond(string word) => conds.Contains(word);
    private static bool IsAnd(string word) => word is WordAnd;
    private static bool IsNot(string word) => word is WordNot;

    public AnalyzeResult Analyze(IBoardProvider board)
    {
        if (board is null) throw new ArgumentNullException(nameof(board));
        List<Rule> rules = new();

        Dictionary<IntPair, LocatedWord> locatedWords = new();
        foreach (var item in board.EnumerateWords())
            locatedWords.Add((item.X, item.Y), item);

        foreach (var (pos, word) in locatedWords)
        {
            int x = word.X;
            int y = word.Y;
            if (!IsNoun(word)) continue;

            for (int dir = 0; dir < 2; dir++)
            {
                int index = 0;
                while (TryGetWordAt(index - 1, out var wordPrevious) && IsNot(wordPrevious))
                {
                    index--;
                }

                if (TryGetWordAt(index - 1, out var wordPrevious2) &&
                    TryGetWordAt(index - 2, out var wordPrevious3))
                {
                    if (IsAnd(wordPrevious2) && IsNoun(wordPrevious3))
                        continue;

                    bool isCond = IsCond(wordPrevious2);
                    if (isCond)
                    {
                        if (IsNot(wordPrevious3))
                        {
                            int ind = index - 3;
                            LocatedWord wordPrevious4;
                            while (TryGetWordAt(ind, out wordPrevious4) && IsNot(wordPrevious4))
                                ind--;
                            if (IsNoun(wordPrevious4))
                                continue;
                        }
                        else if (IsNoun(wordPrevious3))
                            continue;
                    }
                }

                if (!TryParseSubject(index, out int length, out Subject subject))
                    continue;
                index += length;

                if (!TryParseVerb(index, out length, out Verb verb))
                    continue;
                index += length;

                if (!TryParseObject(index, out length, out Object @object))
                    continue;
                index += length;

                Rule rule = new(subject, verb, @object);
                rules.Add(rule);
                continue;

                bool TryGetWordAt(int index, out LocatedWord locatedWord)
                {
                    if (dir == 0)
                        return locatedWords.TryGetValue((x + index, y), out locatedWord);
                    else if (dir == 1)
                        return locatedWords.TryGetValue((x, y + index), out locatedWord);
                    locatedWord = default;
                    return false;
                }

                bool TryParseSubject(int index, out int length, out Subject subject)
                {
                    if (!TryParseConditionalSubject(index, out length, out subject))
                        goto failed;
                    return true;
                failed:
                    length = 0;
                    subject = null;
                    return false;
                }

                bool TryParseObject(int index, out int length, out Object @object)
                {
                    if (!TryParseAndChainObject(index, out length, out @object))
                        goto failed;
                    return true;
                failed:
                    length = 0;
                    @object = null;
                    return false;
                }

                // oh god, this is much easier
                bool TryParseVerb(int index, out int length, out Verb verb)
                {
                    if (TryGetWordAt(index, out var word) && IsVerb(word))
                    {
                        length = 1;
                        verb = new Verb(word);
                        return true;
                    }
                    else
                    {
                        length = 0;
                        verb = null;
                        return false;
                    }
                }

                // can also return subject which isn't a conditional subject, but I can't think up a more suitable name
                bool TryParseConditionalSubject(int index, out int length, out Subject subject)
                {
                    int curIndex = index;
                    int curLength = 0;
                    if (!TryParseAndChainSubject(curIndex, out int firstLength, out var firstSubject))
                        goto failed;
                    curIndex += firstLength;
                    curLength += firstLength;
                    if (TryGetWordAt(curIndex, out var nextWord))
                    {
                        bool notted = false;
                        if (IsNot(nextWord))
                        {
                            int notsCount = CountNots(curIndex);
                            curIndex += notsCount;
                            curLength += notsCount;
                            notted = notsCount % 2 == 1;
                            if (!TryGetWordAt(curIndex, out nextWord))
                                goto firstOnly;
                        }
                        if (IsCond(nextWord))
                        {
                            if (!TryParseAndChainSubject(curIndex + 1, out int secondLength, out var secondSubject))
                                goto firstOnly;
                            curIndex += 1 + secondLength;
                            curLength += 1 + secondLength;

                            length = curLength;
                            if (!notted)
                                subject = new ConditionalSubject(firstSubject, nextWord, secondSubject);
                            else
                                subject = new NotConditionalSubject(firstSubject, nextWord, secondSubject);
                            return true;
                        }
                    }
                firstOnly:
                    length = firstLength;
                    subject = firstSubject;
                    return true;
                failed:
                    length = 0;
                    subject = null;
                    return false;
                }

                // can also return subject which isn't an "and chain", but I can't think up a more suitable name
                bool TryParseAndChainSubject(int index, out int length, out Subject subject)
                {
                    int curIndex = index;
                    int curLength = 0;
                    if (!TryParseNottedSubject(curIndex, out int firstLength, out Subject firstSubject))
                        goto failed;
                    curLength += firstLength;
                    curIndex += firstLength;

                    if (TryGetWordAt(curIndex, out var andWord) &&
                        IsAnd(andWord) &&
                        TryParseNottedSubject(curIndex + 1, out int secondLength, out Subject secondSubject))
                    {
                        List<Subject> subjectsInAndChain = [firstSubject, secondSubject];
                        curIndex += 1 + secondLength;
                        curLength += 1 + secondLength;
                        while (true)
                        {
                            if (TryGetWordAt(curIndex, out var nextAndWord) &&
                                IsAnd(nextAndWord) &&
                                TryParseNottedSubject(curIndex + 1, out int thirdLength, out Subject thirdSubject))
                            {
                                curIndex += 1 + thirdLength;
                                curLength += 1 + thirdLength;
                                subjectsInAndChain.Add(thirdSubject);
                            }
                            else
                            {
                                break;
                            }
                        }
                        length = curLength;
                        subject = new AndSubject(subjectsInAndChain);
                        return true;
                    }
                    else
                    {
                        length = curLength;
                        subject = firstSubject;
                        return true;
                    }


                failed:
                    length = 0;
                    subject = null;
                    return false;
                }

                // can also return not "notted" subject, but I can't think up a more suitable name
                bool TryParseNottedSubject(int index, out int length, out Subject subject)
                {
                    if (!TryGetWordAt(index, out var word))
                        goto failed;

                    bool isNoun = IsNoun(word);
                    bool isNot = IsNot(word);

                    if (isNoun)
                    {
                        length = 1;
                        subject = new NounSubject(word);
                        return true;
                    }
                    else if (isNot)
                    {
                        var notCounts = CountNots(index);
                        if (!TryGetWordAt(index + notCounts, out var nextWord) || !IsNoun(nextWord))
                            goto failed;
                        if (notCounts % 2 == 0)
                            subject = new NounSubject(nextWord);
                        else
                            subject = new NotSubject(new NounSubject(nextWord));
                        length = notCounts + 1;
                        return true;
                    }
                    else
                    {
                        goto failed;
                    }
                failed:
                    length = 0;
                    subject = null;
                    return false;
                }

                // can also return not "notted" object, but I can't think up a more suitable name
                bool TryParseNottedObject(int index, out int length, out Object @object)
                {
                    if (!TryGetWordAt(index, out var word))
                        goto failed;

                    bool isQlt = IsQlt(word);
                    bool isNoun = IsNoun(word);
                    bool isNot = IsNot(word);

                    if (isNoun)
                    {
                        length = 1;
                        @object = new NounObject(word);
                        return true;
                    }
                    else if (isQlt)
                    {
                        length = 1;
                        @object = new QualityObject(word);
                        return true;
                    }
                    else if (isNot)
                    {
                        var notCounts = CountNots(index);
                        if (!TryGetWordAt(index + notCounts, out var nextWord))
                            goto failed;
                        bool nextIsNoun = IsNoun(nextWord);
                        bool nextIsQlt = IsQlt(nextWord);
                        if (nextIsNoun)
                        {
                            if (notCounts % 2 == 0)
                                @object = new NounObject(nextWord);
                            else
                                @object = new NotObject(new NounObject(nextWord));
                        }
                        else if (nextIsQlt)
                        {
                            if (notCounts % 2 == 0)
                                @object = new QualityObject(nextWord);
                            else
                                @object = new NotObject(new QualityObject(nextWord));
                        }
                        else
                        {
                            goto failed;
                        }
                        length = notCounts + 1;
                        return true;
                    }
                    else
                    {
                        goto failed;
                    }
                failed:
                    length = 0;
                    @object = null;
                    return false;
                }

                // can also return subject which isn't an "and chain", but I can't think up a more suitable name
                bool TryParseAndChainObject(int index, out int length, out Object @object)
                {
                    int curIndex = index;
                    int curLength = 0;
                    if (!TryParseNottedObject(curIndex, out int firstLength, out Object firstObject))
                        goto failed;
                    curLength += firstLength;
                    curIndex += firstLength;

                    if (TryGetWordAt(curIndex, out var andWord) &&
                        IsAnd(andWord) &&
                        TryParseNottedObject(curIndex + 1, out int secondLength, out Object secondObject))
                    {
                        List<Object> objectsInAndChain = [firstObject, secondObject];
                        curIndex += 1 + secondLength;
                        curLength += 1 + secondLength;
                        while (true)
                        {
                            if (TryGetWordAt(curIndex, out var nextAndWord) &&
                                IsAnd(nextAndWord) &&
                                TryParseNottedObject(curIndex + 1, out int thirdLength, out Object thirdObject))
                            {
                                curIndex += 1 + thirdLength;
                                curLength += 1 + thirdLength;
                                objectsInAndChain.Add(thirdObject);
                            }
                            else
                            {
                                break;
                            }
                        }
                        length = curLength;
                        @object = new AndObject(objectsInAndChain);
                        return true;
                    }
                    else
                    {
                        length = curLength;
                        @object = firstObject;
                        return true;
                    }


                failed:
                    length = 0;
                    @object = null;
                    return false;
                }

                int CountNots(int index)
                {
                    int count = 0;
                    while (TryGetWordAt(index + count, out var nextWord) && IsNot(nextWord))
                        count++;
                    return count;
                }
            }
        }
        return new AnalyzeResult(rules);
    }
}