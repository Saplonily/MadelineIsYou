using Celeste.Mod.MadelineIsYou.Analyze;

namespace AnalyzerTestConsole;

public class Program
{
    public static void Main()
    {
        Analyzer analyzer = new();
        analyzer.RegisterNoun("madeline");
        analyzer.RegisterNoun("theo");
        analyzer.RegisterVerb("is");
        analyzer.RegisterCondition("on");
        analyzer.RegisterQuality("you");
        analyzer.RegisterQuality("happy");
        analyzer.RegisterQuality("evil");

        string[,] board = new string[,]
        {
            { "theo", "", "" },
            { "on", "", "" },
            { "theo", "", "" },
            { "is", "", "" },
            { "theo", "", "" },
            { "", "", "" },
            { "", "", "" },
            { "", "", "" },
            { "", "", "" },
            { "", "", "" },
        };

        var provider = new ArrayBoardProvider(board);
        var result = analyzer.Analyze(provider);
        Console.WriteLine("Analyze Result:\n");
        foreach (var rule in result.Rules)
        {
            Console.WriteLine(rule);
        }
    }
}