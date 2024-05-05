using System.Text;

namespace Celeste.Mod.MadelineIsYou.Lexer;

public sealed class AndSubject : Subject
{
    public override IEnumerable<BoardWord> RelatedWord
    {
        get
        {
            foreach (var sbj in Subjects)
                foreach (var w in sbj.RelatedWord)
                    yield return w;
            foreach (var a in Ands)
                yield return a;
        }
    }

    public override IEnumerable<Subject> AtomSubjects => Subjects;

    public List<BoardWord> Ands { get; set; }
    public List<Subject> Subjects { get; set; }

    public AndSubject(List<Subject> subjects, List<BoardWord> ands)
    {
        if (subjects.Count < 2)
            throw new ArgumentOutOfRangeException(nameof(subjects));
        if (ands.Count != subjects.Count - 1)
            throw new ArgumentOutOfRangeException(nameof(ands));
        Subjects = subjects;
        Ands = ands;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append('(');
        sb.Append(Subjects[0]);
        for (int i = 1; i < Subjects.Count; i++)
        {
            sb.Append(" and ");
            sb.Append(Subjects[i]);
        }
        sb.Append(')');
        return sb.ToString();
    }
}