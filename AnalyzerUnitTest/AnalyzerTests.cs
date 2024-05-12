using Celeste.Mod.MadelineIsYou.Analyze;

namespace AnalyzerUnitTest;

[TestClass]
public class AnalyzerTests
{
    private readonly Analyzer analyzer;

    public AnalyzerTests()
    {
        analyzer = new();
        analyzer.RegisterNoun("madeline");
        analyzer.RegisterNoun("theo");
        analyzer.RegisterCondition("on");
        analyzer.RegisterVerb("is");
        analyzer.RegisterQuality("happy");
        analyzer.RegisterQuality("you");
        analyzer.RegisterQuality("evil");
    }

    [TestMethod]
    public void NullTest()
    {
        Assert.ThrowsException<ArgumentNullException>(() => analyzer.Analyze(null));
    }

    [TestMethod]
    public void EmptyTest()
    {
        string[,] board = new string[,]
        {
            { "" }
        };
        var result = analyzer.Analyze(new ArrayBoardProvider(board));
        Assert.AreEqual(0, result.Rules.Count);
    }

    [TestMethod]
    public void BasicTest()
    {
        string[,] board = new string[,]
        {
            { "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "madeline", "is", "you", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "" }
        };
        var result = analyzer.Analyze(new ArrayBoardProvider(board));
        Assert.AreEqual(1, result.Rules.Count);
        var rule = result.Rules[0];
        Assert.IsTrue(rule.Subject is NounSubject { Word: "madeline" });
        Assert.IsTrue(rule.Verb is Verb { Word: "is" });
        Assert.IsTrue(rule.Object is QualityObject { Word: "you" });
    }

    [TestMethod]
    public void BasicTest2()
    {
        string[,] board = new string[,]
        {
            { "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "madeline", "is", "you", "", "", "", "", "", "" },
            { "", "", "is", "", "", "", "", "", "", "", "" },
            { "", "", "you", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "" }
        };
        var result = analyzer.Analyze(new ArrayBoardProvider(board));
        Assert.AreEqual(2, result.Rules.Count);
        var rule = result.Rules[0];
        Assert.IsTrue(rule.Subject is NounSubject { Word: "madeline" });
        Assert.IsTrue(rule.Verb is Verb { Word: "is" });
        Assert.IsTrue(rule.Object is QualityObject { Word: "you" });
        rule = result.Rules[1];
        Assert.IsTrue(rule.Subject is NounSubject { Word: "madeline" });
        Assert.IsTrue(rule.Verb is Verb { Word: "is" });
        Assert.IsTrue(rule.Object is QualityObject { Word: "you" });
    }

    [TestMethod]
    public void BasicTest3()
    {
        string[,] board = new string[,]
        {
            { "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "theo", "is", "you", "", "", "", "", "", "" },
            { "", "", "", "madeline", "is", "happy", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "" }
        };
        var result = analyzer.Analyze(new ArrayBoardProvider(board));
        Assert.AreEqual(2, result.Rules.Count);
        var rule = result.Rules[0];
        Assert.IsTrue(rule.Subject is NounSubject { Word: "theo" });
        Assert.IsTrue(rule.Verb is Verb { Word: "is" });
        Assert.IsTrue(rule.Object is QualityObject { Word: "you" });
        rule = result.Rules[1];
        Assert.IsTrue(rule.Subject is NounSubject { Word: "madeline" });
        Assert.IsTrue(rule.Verb is Verb { Word: "is" });
        Assert.IsTrue(rule.Object is QualityObject { Word: "happy" });
    }

    [TestMethod]
    public void AndTest()
    {
        string[,] board = new string[,]
        {
            { "madeline", "and", "theo", "is", "you", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "" }
        };
        var result = analyzer.Analyze(new ArrayBoardProvider(board));
        Assert.AreEqual(1, result.Rules.Count);
        var rule = result.Rules[0];
        Assert.IsTrue(rule.Subject is AndSubject);
        var asbj = (AndSubject)rule.Subject;
        Assert.IsTrue(asbj.Subjects.Count == 2);
        Assert.IsTrue(asbj.Subjects[0] is NounSubject { Word: "madeline" });
        Assert.IsTrue(asbj.Subjects[1] is NounSubject { Word: "theo" });
        Assert.IsTrue(rule.Verb is Verb { Word: "is" });
        Assert.IsTrue(rule.Object is QualityObject { Word: "you" });
    }

    [TestMethod]
    public void ComplexTest1()
    {
        string[,] board = new string[,]
        {
            { "madeline", "and", "theo", "is", "happy", "and", "theo", "and", "happy" },
            { "theo", "and", "theo", "and", "theo", "and", "is", "is", "happy" },
            { "and", "madeline", "is", "happy", "", "", "evil", "", "" },
            { "madeline", "", "", "", "", "", "", "", "" },
            { "and", "", "", "madeline", "you", "", "", "", "" },
            { "theo", "", "", "", "", "", "is", "", "" },
            { "is", "", "", "on", "", "", "madeline", "is", "you" },
            { "happy", "", "", "", "", "", "", "", "" }
        };
        var result = analyzer.Analyze(new ArrayBoardProvider(board));
        Assert.AreEqual(5, result.Rules.Count);
        var rules = result.Rules;

        // TODO any better way to do the test?
        Assert.AreEqual("(madeline AND theo) is (happy AND theo AND happy)", rules[0].ToString());
        Assert.AreEqual("theo is evil", rules[1].ToString());
        Assert.AreEqual("(theo AND madeline AND theo) is happy", rules[2].ToString());
        Assert.AreEqual("madeline is happy", rules[3].ToString());
        Assert.AreEqual("madeline is you", rules[4].ToString());
    }

    [TestMethod]
    public void ComplexTest2()
    {
        string[,] board = new string[,]
        {
            { "madeline", "is", "you" },
            { "theo", "", "" },
            { "and", "", "" },
            { "madeline", "", "" },
            { "is", "", "" },
            { "happy", "", "" },
            { "", "", "" },
        };
        var result = analyzer.Analyze(new ArrayBoardProvider(board));
        Assert.AreEqual(2, result.Rules.Count);
        var rules = result.Rules;

        Assert.AreEqual("madeline is you", rules[0].ToString());
        Assert.AreEqual("(theo AND madeline) is happy", rules[1].ToString());
    }

    [TestMethod]
    public void ConditionTest1()
    {
        string[,] board = new string[,]
        {
            { "not", "", "" },
            { "theo", "", "" },
            { "on", "", "" },
            { "theo", "", "" },
            { "theo", "and", "theo" },
            { "theo", "", "" },
            { "and", "", "" },
            { "theo", "", "" },
            { "is", "", "" },
            { "happy", "", "" },
        };
        var result = analyzer.Analyze(new ArrayBoardProvider(board));
        Assert.AreEqual(1, result.Rules.Count);
        var rules = result.Rules;

        Assert.AreEqual("(theo AND theo) is happy", rules[0].ToString());
    }

    [TestMethod]
    public void ConditionTest2()
    {
        string[,] board = new string[,]
        {
            { "not", "", "" },
            { "theo", "", "" },
            { "on", "", "" },
            { "theo", "", "" },
            { "and", "and", "theo" },
            { "theo", "", "" },
            { "and", "", "" },
            { "theo", "", "" },
            { "is", "", "" },
            { "happy", "", "" },
        };
        var result = analyzer.Analyze(new ArrayBoardProvider(board));
        Assert.AreEqual(1, result.Rules.Count);
        var rules = result.Rules;

        Assert.AreEqual("((NOT theo) on (theo AND theo AND theo)) is happy", rules[0].ToString());
    }

    [TestMethod]
    public void ConditionTest3()
    {
        string[,] board = new string[,]
        {
            { "not", "", "" },
            { "not", "", "" },
            { "theo", "", "" },
            { "not", "", "" },
            { "not", "", "" },
            { "on", "", "" },
            { "not", "", "" },
            { "theo", "", "" },
            { "is", "", "" },
            { "happy", "", "" },
        };
        var result = analyzer.Analyze(new ArrayBoardProvider(board));
        Assert.AreEqual(1, result.Rules.Count);
        var rules = result.Rules;

        Assert.AreEqual("(theo on (NOT theo)) is happy", rules[0].ToString());
    }
}
