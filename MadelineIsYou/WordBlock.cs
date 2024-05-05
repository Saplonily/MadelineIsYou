using Celeste.Mod.Entities;

namespace Celeste.Mod.MadelineIsYou;

[CustomEntity("MadelineIsYou/WordBlock"), Tracked]
public class WordBlock : Solid
{
    public bool Moving { get; private set; }
    public string Word { get; }
    public int ActiveCount { get; set; }

    public WordBlock(Vector2 position, string word)
        : base(position, 16, 16, false)
    {
        Word = word;
        OnDashCollide = DashCollided;
    }

    public WordBlock(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Attr("word", "null"))
    {
    }

    public DashCollisionResults DashCollided(Player player, Vector2 direction)
    {
        Vector2 dir = direction.FourWayNormal();
        if (!Moving && Move(dir))
            return DashCollisionResults.Rebound;
        else
            return DashCollisionResults.NormalCollision;
    }

    public bool Move(Vector2 direction)
    {
        Vector2 position = Position;
        Vector2 target = Position + direction * 16f;

        var list = CollideAll<Solid>(target);
        if (list.Any(e => e is not WordBlock))
            return false;
        var words = list.OfType<WordBlock>();
        var first = words.FirstOrDefault();
        if (first is not null && !first.Moving && !first.Move(direction)) return false;

        Moving = true;
        Tween.Set(this, Tween.TweenMode.Oneshot, 0.2f, Ease.CubeOut, t =>
        {
            Position = Vector2.Lerp(position, target, t.Eased);
        }, t =>
        {
            Moving = false;
            Scene.Tracker.GetEntity<MadelineIsYouManager>().TryAnalyzeScene();
        });
        return true;
    }

    public void BeginNotify() => ActiveCount = 0;
    public void NotifyActive() => ActiveCount += 1;
    public void EndNotify() { }

    public override void Render()
    {
        base.Render();
        Color c = ActiveCount > 0 ? Color.Yellow : Color.White;
        Draw.HollowRect(Collider, c);
        ActiveFont.Draw(Word, Position, Vector2.Zero, Vector2.One * 0.2f, Color.Red);
    }
}
