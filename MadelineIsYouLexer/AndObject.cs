using System.Text;

namespace Celeste.Mod.MadelineIsYou.Lexer;

public sealed class AndObject : Object
{
    public override IEnumerable<BoardWord> RelatedWord
    {
        get
        {
            foreach (var obj in Objects)
                foreach (var w in obj.RelatedWord)
                    yield return w;
            foreach (var a in Ands)
                yield return a;
        }
    }

    public override IEnumerable<Object> AtomObjects => Objects;

    public List<BoardWord> Ands { get; set; }
    public List<Object> Objects { get; set; }

    public AndObject(List<Object> objects, List<BoardWord> ands)
    {
        if (objects.Count < 2)
            throw new ArgumentOutOfRangeException(nameof(objects));
        if (ands.Count != objects.Count - 1)
            throw new ArgumentOutOfRangeException(nameof(ands));
        Objects = objects;
        Ands = ands;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append('(');
        sb.Append(Objects[0]);
        for (int i = 1; i < Objects.Count; i++)
        {
            sb.Append(" and ");
            sb.Append(Objects[i]);
        }
        sb.Append(')');
        return sb.ToString();
    }
}