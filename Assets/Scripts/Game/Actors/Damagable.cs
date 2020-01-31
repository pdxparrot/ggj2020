using pdxpartyparrot.Core.Actors;

namespace pdxpartyparrot.Game.Actors
{
    public abstract class DamageData
    {
        public Actor Source { get; set; }
    }

    public interface IDamagable
    {
        // returns true if the actor took any damage
        bool Damage(DamageData damageData);
    }
}
