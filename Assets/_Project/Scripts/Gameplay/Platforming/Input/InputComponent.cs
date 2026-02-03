using _Project.Application.Input;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace _Project.Gameplay.Platforming.Input
{
    public class InputComponent : MonoBehaviour, IComponent
    {
        [Inject] private IInputSystem _inputSystem;
        
        public Subject<float> MoveState { get; } = new();
        public Subject<bool> JumpState { get; } = new();
        public Subject<Unit> InteractState { get; } = new();

        private bool _isInputEnabled = true;

        void IComponent.Init()
        {
            _inputSystem.OnMove.Subscribe(OnMovePerformed).AddTo(this);
            _inputSystem.OnJump.Subscribe(OnJumpPerformed).AddTo(this);
            _inputSystem.OnInteract.Subscribe(OnInteractPerformed).AddTo(this);
        }

        private void OnMovePerformed(InputVectorActionData data)
        {
            if (!_isInputEnabled) return;
            
            MoveState.OnNext(data.Vector.x);
        }
        
        private void OnJumpPerformed(InputActionData data)
        {
            if (!_isInputEnabled) return;
            
            switch (data.Phase)
            {
                case InputActionPhase.Started:
                    JumpState.OnNext(true);
                    break;
                case InputActionPhase.Canceled:
                    JumpState.OnNext(false);
                    break;
            }
        }

        private void OnInteractPerformed(InputActionData data)
        {
            if (!_isInputEnabled) return;
            
            InteractState.OnNext(Unit.Default);
        }

        public void ToggleInput(bool isEnabled)
        {
            _isInputEnabled = isEnabled;
            JumpState.OnNext(false);
            MoveState.OnNext(0);
        }
    }
}
