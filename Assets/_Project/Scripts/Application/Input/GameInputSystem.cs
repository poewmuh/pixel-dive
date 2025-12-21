using System;
using _Project.Application.GUI.WindowsSystem;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using VContainer;
using VContainer.Unity;

namespace _Project.Application.Input
{
    public class GameInputSystem : IInitializable, IInputSystem, IDisposable
    {
        public Subject<InputActionData> OnJump { get; } = new();
        public Subject<InputVectorActionData> OnMove { get; }  = new();
        public Subject<InputActionData> OnPause { get; } = new();

        private readonly CompositeDisposable _disposables = new ();
        private PlayerInputActions _inputActions;

        public void Initialize()
        {
            _inputActions = new PlayerInputActions();

            _inputActions.Player.Move.PerformedAsObservable()
                .Merge( _inputActions.Player.Move.StartedAsObservable())
                .Merge(_inputActions.Player.Move.CanceledAsObservable())
                .Subscribe(OnMovePerformed).AddTo(_disposables);
            _inputActions.Player.Jump.PerformedAsObservable()
                .Merge(_inputActions.Player.Jump.StartedAsObservable())
                .Merge(_inputActions.Player.Jump.CanceledAsObservable())
                .Subscribe(OnJumpPerformed).AddTo(_disposables);
            _inputActions.Player.Pause.PerformedAsObservable()
                .Subscribe(OnPausePerformed).AddTo(_disposables);

            ((IInputSystem)this).EnablePlayerInput(true);
        }

        void IInputSystem.EnablePlayerInput(bool isEnabled)
        {
            if (isEnabled)
            {
                _inputActions.Player.Enable();
                _inputActions.UI.Disable();
            }
            else
            {
                _inputActions.Player.Disable();
                _inputActions.UI.Enable();
            }
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            OnMove?.OnNext(new InputVectorActionData
            {
                Phase = context.phase,
                Vector = context.action.ReadValue<Vector2>()
            });
        }
        
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            OnJump?.OnNext(new InputActionData
            {
                Phase = context.phase
            });
        }
        
        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            OnPause?.OnNext(new InputActionData
            {
                Phase = context.phase
            });
            
            
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
