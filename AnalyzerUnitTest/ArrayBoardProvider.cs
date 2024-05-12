using Celeste.Mod.MadelineIsYou.Analyze;

namespace AnalyzerUnitTest;

public sealed class ArrayBoardProvider : IBoardProvider
{
    private readonly string[,] array;

    public ArrayBoardProvider(string[,] array)
    {
        this.array = array;
    }

    public IEnumerable<LocatedWord> EnumerateWords()
    {
        for (int i = 0; i < array.GetLength(0); i++)
            for (int j = 0; j < array.GetLength(1); j++)
            {
                var v = array[i, j];
                if (!string.IsNullOrEmpty(v))
                    yield return new LocatedWord(j, i, array[i, j]);
            }
    }
}
