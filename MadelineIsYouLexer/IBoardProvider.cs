namespace Celeste.Mod.MadelineIsYou.Lexer;

public interface IBoardProvider
{
    public IEnumerable<BoardWord> EnumerateWords();
}