namespace Celeste.Mod.MadelineIsYou.Analyze;

public interface IBoardProvider
{
    public IEnumerable<LocatedWord> EnumerateWords();
}