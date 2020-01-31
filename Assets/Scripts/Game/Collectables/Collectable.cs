using pdxpartyparrot.Game.Data;

namespace pdxpartyparrot.Game.Collectables
{
    // TODO: this as an interface should be temporary
    // really what we want is a behavior that also requires an Actor
    // to marker something as a Collectable and to give us hooks into Collectable behaviors
    public interface ICollectable
    {
        bool CanBeCollected { get; }

        void Initialize(CollectableData data);
    }
}
