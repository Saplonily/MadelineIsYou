namespace Celeste.Mod.MadelineIsYou.Lexer;

public abstract class Subject : RulePart
{
    public abstract IEnumerable<Subject> AtomSubjects { get; }
    public abstract override string ToString();
}