namespace pdxpartyparrot.Game.Characters
{
    // TODO: if CharacterBehavior used composition, this probably could as well
    public interface ICharacterMovement
    {
        bool IsComponentControlling { get; set; }

        void Jump(float height);

        void EnableDynamicCollisionDetection(bool enable);
    }
}
