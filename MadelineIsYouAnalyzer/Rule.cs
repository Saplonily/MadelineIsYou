namespace Celeste.Mod.MadelineIsYou.Analyze;

public sealed class Rule(Subject subject, Verb verb, Object @object)
{
    public Subject Subject { get; set; } = subject;
    public Verb Verb { get; set; } = verb;
    public Object Object { get; set; } = @object;

    public override string ToString()
        => $"{Subject} {Verb} {Object}";
}