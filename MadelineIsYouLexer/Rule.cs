namespace Celeste.Mod.MadelineIsYou.Lexer;

public sealed class Rule
{
    public Subject Subject { get; set; }
    public Verb Verb { get; set; }
    public Object Object { get; set; }

    public IEnumerable<BoardWord> RelatedWord => Subject.RelatedWord.Concat(Verb.RelatedWord).Concat(Object.RelatedWord);

    public Rule(Subject subject, Verb verb, Object @object)
    {
        Subject = subject;
        Verb = verb;
        Object = @object;
    }

    public override string ToString()
        => $"{Subject} {Verb} {Object}";
}