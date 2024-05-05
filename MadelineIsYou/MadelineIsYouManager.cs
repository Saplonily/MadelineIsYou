using Celeste.Mod.MadelineIsYou.Lexer;

namespace Celeste.Mod.MadelineIsYou;

[Tracked]
public sealed class MadelineIsYouManager : Entity, IBoardProvider
{
    private Level level;
    private Vector2 levelPos;
    private Lexer.Lexer lexer;
    private AnalyzeResult lastResult;

    public MadelineIsYouManager()
    {
        lexer = new();
        lexer.RegisterNoun("madeline");
        lexer.RegisterNoun("dashes");
        lexer.RegisterVerb("is");
        lexer.RegisterAdjective("you");
        lexer.RegisterAdjective("two");
        lexer.RegisterAdjective("one");
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
        if (HasFeature("dashes", "is", "two"))
        {
            level.Session.Inventory.Dashes = 2;
        }
        if (HasFeature("dashes", "is", "one"))
        {
            level.Session.Inventory.Dashes = 1;
        }
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

        lastResult = lexer.Analyze(this);
        foreach (var entity in words)
        {
            var word = (WordBlock)entity;
            word.BeginNotify();
        }
        foreach (var rule in lastResult.Rules)
        {
            foreach (var w in rule.RelatedWord)
            {
                var entity = (WordBlock)w.Parameter;
                entity.NotifyActive();
            }
        }
        foreach (var entity in words)
        {
            var word = (WordBlock)entity;
            word.EndNotify();
        }
    }

    IEnumerable<BoardWord> IBoardProvider.EnumerateWords()
    {
        var words = level.Tracker.GetEntities<WordBlock>();
        foreach (var entity in words)
        {
            var word = (WordBlock)entity;
            int x = (int)((word.X - levelPos.X) / 16f);
            int y = (int)((word.Y - levelPos.Y) / 16f);
            yield return new BoardWord(x, y, word.Word, word);
        }
    }

    public bool HasFeature(string entity, string verb, string adjective)
    {
        if (lastResult is null) return false;
        foreach (var rule in lastResult.Rules)
        {
            bool s = rule.Subject.AtomSubjects.Any(s => s is NounSubject nsbj && nsbj.Word == entity);
            bool v = rule.Verb.Word == verb;
            bool o = rule.Object.AtomObjects.Any(s => s is AdjectiveObject aobj && aobj.Word == adjective);

            return s && v && o;
        }
        return false;
    }
}
