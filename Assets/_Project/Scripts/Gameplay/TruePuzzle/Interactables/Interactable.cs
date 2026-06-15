using _Project.Gameplay.TruePuzzle.Cursor;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public sealed class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private InteractableType _interactableType;
        [SerializeField] private Transform _destinationPoint;

        [Inject] private IPublisher<InteractableHoverEvent> _hoverPublisher;
        [Inject] private IPublisher<OnClickInteractableEvent> _clickPublisher;

        InteractableType IInteractable.InteractableType => _interactableType;

        void IInteractable.OnHoverEnter()
        {
            _hoverPublisher.Publish(new InteractableHoverEvent(true, this));
        }

        void IInteractable.OnHoverExit()
        {
            _hoverPublisher.Publish(new InteractableHoverEvent(false, this));
        }

        void IInteractable.OnClick()
        {
            if (_interactableType is InteractableType.None || !_destinationPoint) return;

            _clickPublisher.Publish(new OnClickInteractableEvent(this, _destinationPoint));
        }
    }
}
