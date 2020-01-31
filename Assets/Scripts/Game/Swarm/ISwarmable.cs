using UnityEngine;

namespace pdxpartyparrot.Game.Swarm
{
    public interface ISwarmable
    {
        Transform Transform { get; }

        bool CanJoinSwarm { get; }

        bool IsInSwarm { get; }

        void JoinSwarm(Swarm swarm, float radius);

        void RemoveFromSwarm();

    }
}
