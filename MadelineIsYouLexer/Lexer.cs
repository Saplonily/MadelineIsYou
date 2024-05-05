#pragma warning disable IDE0028
#pragma warning disable CS8509

using System;
using IntPair = (int x, int y);

namespace Celeste.Mod.MadelineIsYou.Lexer;

public sealed class Lexer
{
    public const string WordAnd = "and";

    private readonly HashSet<string> nouns;
    private readonly HashSet<string> adjs;
    private readonly HashSet<string> verbs;

    public Lexer()
    {
        nouns = new();
        adjs = new();
        verbs = new();
    }

    public void RegisterNoun(string noun) => nouns.Add(noun);
    public void RegisterAdjective(string adj) => adjs.Add(adj);
    public void RegisterVerb(string verb) => verbs.Add(verb);
    private bool IsNoun(string word) => nouns.Contains(word);
    private bool IsAdj(string word) => adjs.Contains(word);
    private bool IsVerb(string word) => verbs.Contains(word);
    private bool IsAnd(string word) => word is WordAnd;

    public AnalyzeResult Analyze(IBoardProvider board)
    {
        List<Rule> rules = new();
        Dictionary<IntPair, int> boardCheckState = new();
        Dictionary<IntPair, BoardWord> boardWords = new();
        foreach (var item in board.EnumerateWords())
        {
            boardWords.Add((item.X, item.Y), item);
        }
        foreach (var (pos, word) in boardWords)
        {
            int x = word.X;
            int y = word.Y;
            for (int dir = 0; dir < 2; dir++)
            {
                int index = 0;
                if (!TryGetSubject(index, out Subject sbj, out int length))
                    continue;
                index += length;

                if (!TryGetVerb(index, out Verb verb, out length))
                    continue;
                index += length;

                if (!TryGetObject(index, out Object obj, out length))
                    continue;

                rules.Add(new Rule(sbj, verb, obj));

                bool TryGetSubject(int index, out Subject subject, out int length)
                {
                    if (!TryGetWordAt(index, out var thisBoardWord))
                        goto failed;

                    if (!IsNoun(thisBoardWord.Word) || IsWordChecked(index))
                        goto failed;

                    // TODO this recursion can be optimized
                    if (!TryGetWordAt(index + 1, out var nextBoardWord) || !IsAnd(nextBoardWord.Word))
                    {
                        subject = new NounSubject(thisBoardWord);
                        length = 1;
                        return true;
                    }

                    if (!TryGetSubject(index + 2, out Subject innerSbj, out int innerLength))
                        goto failed;

                    if (innerSbj is NounSubject nsbj)
                    {
                        MarkWordChecked(index + 2);
                        subject = new AndSubject([new NounSubject(thisBoardWord), new NounSubject(nsbj.BoardWord)], [nextBoardWord]);
                        length = 2 + innerLength;
                        return true;
                    }
                    else if (innerSbj is AndSubject asbj)
                    {
                        length = 2 + innerLength;
                        List<Subject> sbjs = new(4);
                        sbjs.Add(new NounSubject(thisBoardWord));
                        sbjs.AddRange(asbj.Subjects);
                        subject = new AndSubject(sbjs, [nextBoardWord]);
                        return true;
                    }

                failed:
                    subject = null;
                    length = 0;
                    return false;
                }

                bool TryGetVerb(int index, out Verb verb, out int length)
                {
                    if (!TryGetWordAt(index, out BoardWord boardWord))
                        goto failed;

                    if (!IsVerb(boardWord.Word))
                        goto failed;

                    verb = new Verb(boardWord);
                    length = 1;
                    return true;

                failed:
                    verb = null;
                    length = 0;
                    return false;
                }

                bool TryGetObject(int index, out Object @object, out int length)
                {
                    if (!TryGetWordAt(index, out BoardWord thisBoardWord))
                        goto failed;

                    bool isNoun = IsNoun(thisBoardWord.Word);
                    bool isAdj = IsAdj(thisBoardWord.Word);
                    if ((!isNoun && !isAdj) || IsWordChecked(index))
                        goto failed;

                    Object obj;
                    if (isNoun)
                        obj = new NounObject(thisBoardWord);
                    else if (isAdj)
                        obj = new AdjectiveObject(thisBoardWord);
                    else
                        throw new InvalidOperationException();

                    // TODO this recursion can be optimized
                    if (!TryGetWordAt(index + 1, out BoardWord nextBoardWord) || !IsAnd(nextBoardWord.Word))
                    {
                        @object = obj;
                        length = 1;
                        return true;
                    }

                    if (!TryGetObject(index + 2, out Object innerObj, out int innerLength))
                        goto failed;

                    if (innerObj is NounObject nobj)
                    {
                        MarkWordChecked(index + 2);
                        @object = new AndObject([obj, nobj], [nextBoardWord]);
                        length = 2 + innerLength;
                        return true;
                    }
                    else if (innerObj is AdjectiveObject adjobj)
                    {
                        MarkWordChecked(index + 2);
                        @object = new AndObject([obj, adjobj], [nextBoardWord]);
                        length = 2 + innerLength;
                        return true;
                    }
                    else if (innerObj is AndObject aobj)
                    {
                        length = 2 + innerLength;
                        List<Object> objs = new(4);
                        objs.Add(obj);
                        objs.AddRange(aobj.Objects);
                        @object = new AndObject(objs, [nextBoardWord]);
                        return true;
                    }

                failed:
                    @object = null;
                    length = 0;
                    return false;
                }

                bool TryGetWordAt(int index, out BoardWord sceneWord)
                {
                    if (dir == 0)
                        return boardWords.TryGetValue((x + index, y), out sceneWord);
                    else if (dir == 1)
                        return boardWords.TryGetValue((x, y + index), out sceneWord);
                    sceneWord = default;
                    return false;
                }

                void MarkWordChecked(int index)
                {
                    if (dir == 0)
                    {
                        int v = 1 << dir;
                        if (boardCheckState.TryGetValue((x + index, y), out int pv))
                            v |= pv;
                        boardCheckState[(x + index, y)] = v;
                        return;
                    }
                    else if (dir == 1)
                    {
                        int v = 1 << dir;
                        if (boardCheckState.TryGetValue((x, y + index), out int pv))
                            v |= pv;
                        boardCheckState[(x, y + index)] = v;
                        return;
                    }
                    throw new ArgumentException(nameof(dir));
                }

                bool IsWordChecked(int index)
                {
                    if (dir == 0)
                    {
                        if (boardCheckState.TryGetValue((x + index, y), out int v))
                            return (v & (1 << dir)) != 0;
                        else
                            return false;
                    }
                    else if (dir == 1)
                    {
                        if (boardCheckState.TryGetValue((x, y + index), out int v))
                            return (v & (1 << dir)) != 0;
                        else
                            return false;
                    }
                    throw new ArgumentException(nameof(dir));
                }
            }
        }
        return new AnalyzeResult(rules);
    }
}