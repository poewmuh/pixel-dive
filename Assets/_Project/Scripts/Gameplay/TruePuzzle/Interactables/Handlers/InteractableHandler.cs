using MessagePipe;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using R3;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class InteractableHandler : MonoBehaviour
    {
        protected const string OnHandleEventName = "OnHandle";
        
        [SerializeField] private UnityEvent _onInteract;
        [SerializeField] private UnityEvent _onCompleteInteraction;

        [Inject] private ISubscriber<ReachedInteractableEvent> _reachedInteractableSubscriber;
        [Inject] private IPublisher<OnInteractionCompleteEvent> _interactionCompletePublisher;

        protected FSMOwner _fsmOwner;
        protected Blackboard _blackboard;

        private IInteractable _interactable;
        private InteractableType _interactableType;

        private void Start()
        {
            _fsmOwner = GetComponent<FSMOwner>();
            _blackboard = GetComponent<Blackboard>();
            _reachedInteractableSubscriber.Subscribe(OnReachedInteractable).AddTo(this);

            if (!TryGetComponent(out _interactable))
            {
                Debug.LogWarning($"{GetType().Name} requires an {nameof(IInteractable)} on the same GameObject.", this);
                Init();
                return;
            }

            _interactableType = _interactable.InteractableType;
            Init();
        }

        protected virtual void Init() { }

        private void OnReachedInteractable(ReachedInteractableEvent message)
        {
            if (!ReferenceEquals(message.Interactable, _interactable)) return;

            _onInteract?.Invoke();
            _fsmOwner?.SendEvent(OnHandleEventName);
        }

        public void SendCompleteInteraction()
        {
            _onCompleteInteraction?.Invoke();
            _interactionCompletePublisher.Publish(new OnInteractionCompleteEvent(_interactableType, transform));
        }
    }

    public sealed class OnInteractionCompleteEvent
    {
        public InteractableType InteractableType { get; }
        public Transform InteractableTransform { get; }

        public OnInteractionCompleteEvent(InteractableType interactableType, Transform interactableTransform)
        {
            InteractableType = interactableType;
            InteractableTransform = interactableTransform;
        }
    }
}
