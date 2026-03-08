using System;
using _Project.Gameplay.TruePuzzle.Cursor;
using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private InteractableType _interactableType;
        [SerializeField] private Transform _destinationPoint;
        
        [Inject] private IPublisher<InteractableHoverEvent> _hoverSubscriber;
        [Inject] private IPublisher<OnClickInteractableEvent> _onClickInteractableSub;
        
        InteractableType IInteractable.InteractableType => _interactableType;

        void IInteractable.OnHoverEnter()
        {
            _hoverSubscriber.Publish(new InteractableHoverEvent(true, this));
        }

        void IInteractable.OnHoverExit()
        {
            _hoverSubscriber.Publish(new InteractableHoverEvent(false, this));
        }

        void IInteractable.OnClick()
        {
            if (_interactableType is not InteractableType.None)
            {
                _onClickInteractableSub.Publish(new OnClickInteractableEvent(_interactableType, _destinationPoint));
            }
        }
    }
}
