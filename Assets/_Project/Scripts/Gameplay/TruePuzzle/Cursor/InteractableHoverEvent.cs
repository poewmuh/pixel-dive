namespace _Project.Gameplay.TruePuzzle.Cursor
{
    public class InteractableHoverEvent
    {
        public bool IsHovering { get; }

        public InteractableHoverEvent(bool isHovering)
        {
            IsHovering = isHovering;
        }
    }
}