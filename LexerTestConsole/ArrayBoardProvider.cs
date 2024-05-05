using Celeste.Mod.MadelineIsYou.Lexer;

namespace LexerTestConsole;

public sealed class ArrayBoardProvider : IBoardProvider
{
    private readonly string[,] array;

    public ArrayBoardProvider(string[,] array)
    {
        this.array = array;
    }

    public IEnumerable<BoardWord> EnumerateWords()
    {
        for (int i = 0; i < array.GetLength(0); i++)
            for (int j = 0; j < array.GetLength(1); j++)
            {
                var v = array[i, j];
                if (!string.IsNullOrEmpty(v))
                    yield return new BoardWord(j, i, array[i, j]);
            }
    }
}
