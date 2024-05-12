using System.Diagnostics;

namespace Celeste.Mod.MadelineIsYou.Analyze;

[DebuggerDisplay("{Word,nq} ({X}, {Y})")]
public struct LocatedWord
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Word { get; set; }

    public LocatedWord(int x, int y, string word)
    {
        X = x;
        Y = y;
        Word = word;
    }

    public static implicit operator string(in LocatedWord word)
        => word.Word;
}