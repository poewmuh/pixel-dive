using _Project.Gameplay.TruePuzzle.Interactable;

namespace _Project.Gameplay.TruePuzzle.Cursor
{
    public class InteractableHoverEvent
    {
        public IInteractable Interactable { get; }
        public bool IsHovering { get; }

        public InteractableHoverEvent(bool isHovering, IInteractable interactable)
        {
            IsHovering = isHovering;
            Interactable = interactable;
        }
    }
}