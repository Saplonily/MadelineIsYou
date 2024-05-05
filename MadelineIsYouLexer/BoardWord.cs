using System.Diagnostics;

namespace Celeste.Mod.MadelineIsYou.Lexer;

[DebuggerDisplay("{Word,nq} ({X}, {Y})")]
public struct BoardWord
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Word { get; set; }
    public object Parameter { get; set; }

    public BoardWord(int x, int y, string word, object parameter = null)
    {
        X = x;
        Y = y;
        Word = word;
        Parameter = parameter;
    }
}