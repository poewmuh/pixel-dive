using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class OnClickInteractableEvent
    {
        public InteractableType InteractableType { get; }
        public Transform DestinationPoint { get; }

        public OnClickInteractableEvent(InteractableType interactableType, Transform destinationPoint)
        {
            InteractableType = interactableType;
            DestinationPoint = destinationPoint;
        }
    }

    public class ReachedInteractableEvent
    {
        public InteractableType InteractableType { get; }

        public ReachedInteractableEvent(InteractableType interactableType)
        {
            InteractableType = interactableType;
        }
    }
}