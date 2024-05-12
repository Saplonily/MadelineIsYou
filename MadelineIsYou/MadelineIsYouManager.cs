using Celeste.Mod.MadelineIsYou.Analyze;

namespace Celeste.Mod.MadelineIsYou;

[Tracked]
public sealed class MadelineIsYouManager : Entity, IBoardProvider
{
    private Level level;
    private Vector2 levelPos;
    private Analyzer analyzer;
    private AnalyzeResult lastResult;

    public MadelineIsYouManager()
    {
        analyzer = new();
        analyzer.RegisterNoun("madeline");
        analyzer.RegisterNoun("dashes");
        analyzer.RegisterVerb("is");
        analyzer.RegisterQuality("you");
        analyzer.RegisterQuality("two");
        analyzer.RegisterQuality("one");
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = (Level)scene;
        levelPos = level.LevelOffset;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        TryAnalyzeScene();
    }

    public override void Update()
    {
        base.Update();
        if (Scene.OnInterval(0.1f))
            ApplyRules();
    }

    public void ApplyRules()
    {
    }

    public void TryAnalyzeScene()
    {
        var words = level.Tracker.GetEntities<WordBlock>();
        bool anyMoving = false;
        foreach (var entity in words)
        {
            var word = (WordBlock)entity;
            if (word.Moving)
                anyMoving = true;
        }
        if (anyMoving)
            return;

        lastResult = analyzer.Analyze(this);
    }

    IEnumerable<LocatedWord> IBoardProvider.EnumerateWords()
    {
        var words = level.Tracker.GetEntities<WordBlock>();
        foreach (var entity in words)
        {
            var word = (WordBlock)entity;
            int x = (int)((word.X - levelPos.X) / 16f);
            int y = (int)((word.Y - levelPos.Y) / 16f);
            yield return new LocatedWord(x, y, word.Word);
        }
    }
}
