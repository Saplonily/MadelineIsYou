namespace Celeste.Mod.MadelineIsYou.Lexer;

public sealed class AnalyzeResult
{
    public List<Rule> Rules { get; set; }

	public AnalyzeResult(List<Rule> rules)
	{
		Rules = rules;
	}
}