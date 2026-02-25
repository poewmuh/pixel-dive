using System;
using _Project.Application.Input;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace _Project.Gameplay.TruePuzzle.Cursor
{
    public class CursorSystem : ICursorSystem, IInitializable, IDisposable
    {
        public ReactiveProperty<CursorState> CurrentState { get; } = new(CursorState.Disabled);
        
        [Inject] private IInputSystem _inputSystem;
        [Inject] private IInputSwitcher _inputSwitcher;
        [Inject] private ISubscriber<InteractableHoverEvent> _hoverSubscriber;

        private bool _isPressed;
        private bool _isHovering;
        private bool _isBlocked;
        private CompositeDisposable _disposables = new ();
        
        void IInitializable.Initialize()
        {
            _inputSystem.OnUIMouseClick.Subscribe(OnMouseClickPerformed).AddTo(_disposables);
            _inputSystem.OnPuzzleMouseClick.Subscribe(OnMouseClickPerformed).AddTo(_disposables);
            _hoverSubscriber.Subscribe(OnHoverChange).AddTo(_disposables);
            _inputSwitcher.Blockers.Subscribe(OnBlockersChange).AddTo(_disposables);
            _inputSwitcher.InputMap.Subscribe(OnInputMapChanged).AddTo(_disposables);
        }

        private void OnMouseClickPerformed(InputVectorActionData data)
        {
            _isPressed = data.Phase is InputActionPhase.Started or InputActionPhase.Performed;
            UpdateCurrentState();
        }

        private void OnBlockersChange(InputBlockMask blockers)
        {
            if (!blockers.HasFlag(InputBlockMask.Windows) && !IsCursorGameMode())
            {
                CurrentState.Value = CursorState.Disabled;
                _isBlocked = true;
            }
            else
            {
                CurrentState.Value = CursorState.Default;
                _isBlocked = false;
            }
        }

        private void OnInputMapChanged(InputMapMode mode)
        {
            OnBlockersChange(_inputSwitcher.Blockers.Value);
        }

        private bool IsCursorGameMode()
        {
            return _inputSwitcher.InputMap.Value is InputMapMode.Puzzle;
        }

        private void OnHoverChange(InteractableHoverEvent msg)
        {
            _isHovering = msg.IsHovering;
            UpdateCurrentState();
        }

        private void UpdateCurrentState()
        {
            if (_isBlocked) return;
            CurrentState.Value = (_isHovering, _isPressed) switch
            {
                (false, false) => CursorState.Default,
                (false, true)  => CursorState.DefaultPressed,
                (true, false)  => CursorState.Interactive,
                (true, true)   => CursorState.InteractivePressed
            };
        }

        void IDisposable.Dispose()
        {
            CurrentState.Value = CursorState.Disabled;
            _disposables.Dispose();
        }
    }
}