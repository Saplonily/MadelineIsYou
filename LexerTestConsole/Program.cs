using Celeste.Mod.MadelineIsYou.Lexer;

namespace LexerTestConsole;

public class Program
{
    public static void Main()
    {
        Lexer lexer = new();
        lexer.RegisterNoun("madeline");
        lexer.RegisterNoun("theo");
        lexer.RegisterVerb("is");
        lexer.RegisterAdjective("you");
        lexer.RegisterAdjective("happy");
        lexer.RegisterAdjective("evil");

        /* This board expected
         * 
         *  (madeline and theo) is (happy and theo and happy)
         *  theo is evil
         *  (theo and madeline and theo) is happy
         *  madeline is happy
         *  madeline is you
         *  
         */
        string[,] array = new string[,]
        {
            { "madeline", "and", "theo", "is", "happy", "and", "theo", "and", "happy" },
            { "theo", "and", "theo", "and", "theo", "and", "is", "is", "happy" },
            { "and", "madeline", "is", "happy", "", "", "evil", "", "" },
            { "madeline", "", "", "", "", "", "", "", "" },
            { "and", "", "", "madeline", "you", "", "", "", "" },
            { "theo", "", "", "", "", "", "is", "", "" },
            { "is", "", "", "", "", "", "madeline", "is", "you" },
            { "happy", "", "", "", "", "", "", "", "" }
        };

        /* This board expected
         * 
         *  madeline is you
         *  (madeline and theo) is (happy and happy)
         *  madeline is theo
         *  theo is madeline
        */
        string[,] array2 = new string[,]
        {
            { "madeline", "is", "you" },
            { "", "", "" },
            { "", "", "" },
            { "", "", "" },
            { "", "", "" },
            { "", "", "" },
            { "", "", "" },
        };
        var provider = new ArrayBoardProvider(array2);
        var result = lexer.Analyze(provider);
        Console.WriteLine("Analyze Result:\n");
        foreach (var rule in result.Rules)
        {
            Console.WriteLine(rule);
        }
    }
}