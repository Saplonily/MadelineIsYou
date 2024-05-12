using System.Text;

namespace Celeste.Mod.MadelineIsYou.Analyze;

public sealed class NounSubject(string word) : Subject
{
    public string Word { get; set; } = word;
    public override string ToString() => Word;
}

public sealed class NounObject(string word) : Object
{
    public string Word { get; set; } = word;
    public override string ToString() => Word;
}

public sealed class QualityObject(string word) : Object
{
    public string Word { get; set; } = word;
    public override string ToString() => Word;
}

public sealed class NotSubject(NounSubject subject) : Subject
{
    public NounSubject NounSubject { get; set; } = subject;
    public override string ToString() => $"(NOT {NounSubject})";
}

public sealed class NotObject : Object
{
    public Object Object { get; set; }

    public NotObject(NounObject nounObject) => Object = nounObject;
    public NotObject(QualityObject qualityObject) => Object = qualityObject;

    public override string ToString() => $"(NOT {Object})";
}

public sealed class AndSubject : Subject
{
    public List<Subject> Subjects { get; set; }

    public AndSubject(List<Subject> subjects)
    {
        if (subjects.Count < 2)
            throw new ArgumentOutOfRangeException(nameof(subjects));
        if (subjects.Any(obj => obj is not NounSubject and not NotSubject))
            throw new ArgumentException("All Subjects must be NounSubject or NotSubject.", nameof(subjects));
        Subjects = subjects;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append('(');
        sb.Append(Subjects[0]);
        for (int i = 1; i < Subjects.Count; i++)
        {
            sb.Append(" AND ");
            sb.Append(Subjects[i]);
        }
        sb.Append(')');
        return sb.ToString();
    }
}

public sealed class AndObject : Object
{
    public List<Object> Objects { get; set; }

    public AndObject(List<Object> objects)
    {
        if (objects.Count < 2)
            throw new ArgumentOutOfRangeException(nameof(objects));
        if (objects.Any(obj => obj is not NounObject and not QualityObject and not NotObject))
            throw new ArgumentException("All Objects must be NounObject or QualityObject or NotObject.", nameof(objects));
        Objects = objects;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append('(');
        sb.Append(Objects[0]);
        for (int i = 1; i < Objects.Count; i++)
        {
            sb.Append(" AND ");
            sb.Append(Objects[i]);
        }
        sb.Append(')');
        return sb.ToString();
    }
}

// "AND" between conditional words is currently not supported :(
public sealed class ConditionalSubject : Subject
{
    public Subject Subject { get; set; }
    public string Word { get; set; }
    public Subject ConditionSubject { get; set; }

    public ConditionalSubject(Subject subject, string word, Subject conditionSubject)
    {
        if (subject is ConditionalSubject || conditionSubject is ConditionalSubject)
            throw new ArgumentException("Must not be ConditionalSubject.", nameof(subject));

        Subject = subject;
        Word = word;
        ConditionSubject = conditionSubject;
    }

    public override string ToString() => $"({Subject} {Word} {ConditionSubject})";
}

public sealed class NotConditionalSubject : Subject
{
    public Subject Subject { get; set; }
    public string Word { get; set; }
    public Subject ConditionSubject { get; set; }

    public NotConditionalSubject(Subject subject, string word, Subject conditionSubject)
    {
        if (subject is ConditionalSubject || conditionSubject is ConditionalSubject)
            throw new ArgumentException("Must not be ConditionalSubject.", nameof(subject));

        Subject = subject;
        Word = word;
        ConditionSubject = conditionSubject;
    }

    public override string ToString() => $"({Subject} NOT {Word} {ConditionSubject})";
}