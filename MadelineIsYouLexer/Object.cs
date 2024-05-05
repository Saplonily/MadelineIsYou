namespace Celeste.Mod.MadelineIsYou.Lexer;

public abstract class Object : RulePart
{
    public abstract IEnumerable<Object> AtomObjects { get; }
    public abstract override string ToString();
}