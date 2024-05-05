namespace Celeste.Mod.MadelineIsYou.Lexer;

public abstract class RulePart
{
    public abstract IEnumerable<BoardWord> RelatedWord { get; }
}