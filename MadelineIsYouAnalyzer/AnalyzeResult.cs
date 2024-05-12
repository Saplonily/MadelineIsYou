using System.Diagnostics;

namespace Celeste.Mod.MadelineIsYou.Analyze;

[DebuggerDisplay("Rules.Count = {Rules.Count}")]
public sealed class AnalyzeResult
{
    public List<Rule> Rules { get; set; }

	public AnalyzeResult(List<Rule> rules)
	{
		Rules = rules;
	}
}