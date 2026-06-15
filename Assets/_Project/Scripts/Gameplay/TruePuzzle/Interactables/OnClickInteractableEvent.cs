using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public sealed class OnClickInteractableEvent
    {
        public IInteractable Interactable { get; }
        public Transform DestinationPoint { get; }

        public OnClickInteractableEvent(IInteractable interactable, Transform destinationPoint)
        {
            Interactable = interactable;
            DestinationPoint = destinationPoint;
        }
    }

    public sealed class ReachedInteractableEvent
    {
        public IInteractable Interactable { get; }

        public ReachedInteractableEvent(IInteractable interactable)
        {
            Interactable = interactable;
        }
    }
}
