using Celeste.Mod.MadelineIsYou.Analyze;

namespace AnalyzerUnitTest;

// This test tests all samples in the top of the file "MadelineIsYouAnalyzer/Analyzer.cs"

/*
    N is Q
    N is (NOT Q)
    (NOT N) is Q
    (N AND N) is Q
    N is (N AND Q)
    N on N is Q
    (N and N) on N is Q
    N on (N and N) is Q
    N (NOT on) N is Q  
    (N AND N) on (N AND N) is Q

    Specials:

    N on N AND (N on N is N) -> "N on N is N"
*/

[TestClass]
public class BasicTests
{
    private readonly Analyzer analyzer;

    public BasicTests()
    {
        analyzer = new();
        analyzer.RegisterNoun("N");
        analyzer.RegisterCondition("on");
        analyzer.RegisterVerb("is");
        analyzer.RegisterQuality("Q");
    }

    public void BasicTestHelper(string[,] board, params string[] expecteds)
    {
        var result = analyzer.Analyze(new ArrayBoardProvider(board));
        Assert.AreEqual(expecteds.Length, result.Rules.Count);
        var rules = result.Rules;
        for (int i = 0; i < expecteds.Length; i++)
            Assert.AreEqual(expecteds[i], rules[i].ToString());
    }

    [TestMethod]
    public void Test1()
    {
        BasicTestHelper(new string[,]
        {
            { "N", "is", "Q" }
        },
        "N is Q"
        );
    }

    [TestMethod]
    public void Test2()
    {
        BasicTestHelper(new string[,]
        {
            { "N", "is", "not", "Q" }
        },
        "N is (NOT Q)"
        );
    }

    [TestMethod]
    public void Test3()
    {
        BasicTestHelper(new string[,]
        {
            { "not", "N", "is", "Q" }
        },
        "(NOT N) is Q"
        );
    }

    [TestMethod]
    public void Test4()
    {
        BasicTestHelper(new string[,]
        {
            { "N", "and", "N", "is", "Q" }
        },
        "(N AND N) is Q"
        );
    }

    [TestMethod]
    public void Test5()
    {
        BasicTestHelper(new string[,]
        {
            { "N", "is", "N", "and", "Q" }
        },
        "N is (N AND Q)"
        );
    }

    [TestMethod]
    public void Test6()
    {
        BasicTestHelper(new string[,]
        {
            { "N", "on", "N", "is", "Q" }
        },
        "(N on N) is Q"
        );
    }

    [TestMethod]
    public void Test7()
    {
        BasicTestHelper(new string[,]
        {
            { "N", "and", "N", "on", "N", "is", "Q" }
        },
        "((N AND N) on N) is Q"
        );
    }

    [TestMethod]
    public void Test8()
    {
        BasicTestHelper(new string[,]
        {
            { "N", "on", "N", "and", "N", "is", "Q" }
        },
        "(N on (N AND N)) is Q"
        );
    }

    [TestMethod]
    public void Test9()
    {
        BasicTestHelper(new string[,]
        {
            { "N", "not", "on", "N", "is", "Q" }
        },
        "(N NOT on N) is Q"
        );
    }

    [TestMethod]
    public void Test10()
    {
        BasicTestHelper(new string[,]
        {
            { "N", "and", "N", "on", "N", "and", "N", "is", "Q" }
        },
        "((N AND N) on (N AND N)) is Q"
        );
    }

    // TODO support this
    //[TestMethod]
    //public void Test11()
    //{
    //    BasicTestHelper(new string[,]
    //    {
    //        { "N", "on", "N", "and", "N", "on", "N", "is", "Q" }
    //    },
    //    "N on N is Q"
    //    );
        
    //}
}
