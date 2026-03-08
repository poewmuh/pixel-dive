using MessagePipe;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using R3;
using UnityEngine;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(FSMOwner))]
    public class InteractableHandler : MonoBehaviour
    {
        [Inject] private ISubscriber<ReachedInteractableEvent> _onReachedInteractableSub;
        
        protected FSMOwner _fsmOwner;
        protected Blackboard _blackboard;
        private InteractableType _interactableType;
        
        public void Start()
        {
            _fsmOwner = GetComponent<FSMOwner>();
            _blackboard = GetComponent<Blackboard>();
            _onReachedInteractableSub.Subscribe(OnReachedInteractable).AddTo(this);

            if (TryGetComponent(out IInteractable interactable))
            {
                _interactableType = interactable.InteractableType;
            }

            Init();
        }
        
        protected virtual void Init() { }
        
        private void OnReachedInteractable(ReachedInteractableEvent msg)
        {
            if (msg.InteractableType != _interactableType) return;
            
            _fsmOwner.SendEvent("OnHandle");
        }
    }
}