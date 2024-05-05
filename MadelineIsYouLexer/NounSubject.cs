namespace Celeste.Mod.MadelineIsYou.Lexer;

public sealed class NounSubject : Subject
{
    public override IEnumerable<Subject> AtomSubjects => new SingleEnumerable<Subject>(this);
    public override IEnumerable<BoardWord> RelatedWord => new SingleEnumerable<BoardWord>(BoardWord);

    public BoardWord BoardWord { get; set; }
    public string Word => BoardWord.Word;

    public NounSubject(BoardWord boardWord)
    {
        BoardWord = boardWord;
    }

    public override string ToString() 
        => BoardWord.Word;
}