#pragma warning disable IDE0028

namespace Celeste.Mod.MadelineIsYou;

public sealed class MadelineIsYouModule : EverestModule
{
    public static MadelineIsYouModule Instance { get; private set; }

    public override Type SessionType => typeof(MadelineIsYouSession);
    public static MadelineIsYouSession Session => (MadelineIsYouSession)Instance._Session;

    public MadelineIsYouModule()
    {
        Instance = this;
    }

    public override void Load()
    {
        Everest.Events.Level.OnLoadEntity += Level_OnLoadEntity;
    }

    private bool Level_OnLoadEntity(Level level, LevelData levelData, Vector2 offset, EntityData entityData)
    {
        if (entityData.Name == "MadelineIsYou/WordBlock")
        {
            if (level.Tracker.GetEntity<MadelineIsYouManager>() is null)
                level.Add(new MadelineIsYouManager());
            WordBlock wb = new(entityData, offset);
            level.Add(wb);
            return true;
        }
        return false;
    }

    public override void Unload()
    {
        Everest.Events.Level.OnLoadEntity -= Level_OnLoadEntity;
    }
}