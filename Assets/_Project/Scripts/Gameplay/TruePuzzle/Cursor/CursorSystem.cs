using System;
using _Project.Application.Input;
using _Project.Gameplay.TruePuzzle.Interactable;
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

        private bool _hasBlocker;
        private bool _isPressed;
        private bool _isHovering;
        private bool _isBlocked;
        private IInteractable _currentInteractable;
        private CompositeDisposable _disposables = new ();
        
        void IInitializable.Initialize()
        {
            _inputSystem.OnUIMouseClick.Subscribe(OnMouseClickPerformed).AddTo(_disposables);
            _inputSystem.OnPuzzleMouseClick.Subscribe(OnMouseClickPuzzle).AddTo(_disposables);
            _hoverSubscriber.Subscribe(OnHoverChange).AddTo(_disposables);
            _inputSwitcher.Blockers.Subscribe(OnBlockersChange).AddTo(_disposables);
            _inputSwitcher.InputMap.Subscribe(OnInputMapChanged).AddTo(_disposables);
        }

        private void OnMouseClickPerformed(InputVectorActionData data)
        {
            _isPressed = data.Phase is InputActionPhase.Started or InputActionPhase.Performed;
            UpdateCurrentState();
        }

        private void OnMouseClickPuzzle(InputVectorActionData data)
        {
            OnMouseClickPerformed(data);
            
            if (data.Phase is not InputActionPhase.Performed) return;
            if (_hasBlocker) return;

            if (_isHovering && _currentInteractable != null)
            {
                _currentInteractable.OnClick();
            }
        }

        private void OnBlockersChange(InputBlockMask blockers)
        {
            _hasBlocker = blockers != InputBlockMask.None;
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
            
            UpdateCurrentState();
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
            _currentInteractable = msg.Interactable;
            _isHovering = msg.IsHovering;
            UpdateCurrentState();
        }

        private void UpdateCurrentState()
        {
            if (_isBlocked) return;
            if (_hasBlocker)
            {
                CurrentState.Value = (_isHovering, _isPressed) switch
                {
                    (false, false) => CursorState.Default,
                    (false, true)  => CursorState.DefaultPressed,
                    (true, false)  => CursorState.Default,
                    (true, true)   => CursorState.DefaultPressed
                };
            }
            else
            {
                CurrentState.Value = (_isHovering, _isPressed) switch
                {
                    (false, false) => CursorState.Default,
                    (false, true)  => CursorState.DefaultPressed,
                    (true, false)  => CursorState.Interactive,
                    (true, true)   => CursorState.InteractivePressed
                };
            }
            
        }

        void IDisposable.Dispose()
        {
            CurrentState.Value = CursorState.Disabled;
            _disposables.Dispose();
        }
    }
}