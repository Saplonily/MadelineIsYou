using System.Collections;

namespace Celeste.Mod.MadelineIsYou.Lexer;

internal sealed class SingleEnumerable<T> : IEnumerable<T>
{
	private T value;

	public SingleEnumerable(T value)
	{
		this.value = value;
	}

    public IEnumerator<T> GetEnumerator() { yield return value; }
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}