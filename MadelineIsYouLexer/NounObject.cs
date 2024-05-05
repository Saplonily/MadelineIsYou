
namespace Celeste.Mod.MadelineIsYou.Lexer;

public sealed class NounObject : Object
{
    public override IEnumerable<Object> AtomObjects => new SingleEnumerable<Object>(this);
    public override IEnumerable<BoardWord> RelatedWord => new SingleEnumerable<BoardWord>(BoardWord);

    public BoardWord BoardWord { get; set; }

    public NounObject(BoardWord boardWord)
    {
        BoardWord = boardWord;
    }

    public override string ToString() 
        => BoardWord.Word;
}