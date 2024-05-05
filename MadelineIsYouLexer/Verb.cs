namespace Celeste.Mod.MadelineIsYou.Lexer;

public sealed class Verb : RulePart
{
    public override IEnumerable<BoardWord> RelatedWord => new SingleEnumerable<BoardWord>(BoardWord);

    public BoardWord BoardWord { get; set; }

    public string Word => BoardWord.Word;

    public Verb(BoardWord word)
    {
        BoardWord = word;
    }

    public override string ToString()
        => BoardWord.Word;
}