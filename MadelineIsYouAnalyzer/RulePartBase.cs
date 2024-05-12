namespace Celeste.Mod.MadelineIsYou.Analyze;

public abstract class Subject
{
    public abstract override string ToString();
}

public sealed class Verb(string word)
{
    public string Word { get; set; } = word;
    public override string ToString() => Word;
}

public abstract class Object
{
    public abstract override string ToString();
}