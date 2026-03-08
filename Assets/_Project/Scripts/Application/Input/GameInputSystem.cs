using System;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utils;
using VContainer.Unity;

namespace _Project.Application.Input
{
    public class GameInputSystem : IInitializable, IInputSystem, IDisposable
    {
        public Subject<InputActionData> OnJump { get; } = new();
        public Subject<InputVectorActionData> OnMove { get; }  = new();
        public Subject<InputActionData> OnPause { get; } = new();
        public Subject<InputActionData> OnSkipClicked { get; } = new();
        public Subject<InputActionData> OnInteract { get; } = new();
        public Subject<InputVectorActionData> OnUIMouseClick { get; } = new();
        public Subject<InputVectorActionData> OnPuzzleMouseClick { get; } = new();

        public ReactiveProperty<string> CurrentControlScheme { get; } = new();

        InputAction IInputSystem.PlatformingInteractAction => _inputActions.PlayerPlatforming.Interact;

        private readonly CompositeDisposable _disposables = new ();
        private PlayerInputActions _inputActions;

        public void Initialize()
        {
            _inputActions = new PlayerInputActions();

            _inputActions.PlayerPlatforming.Move.PerformedAsObservable()
                .Merge( _inputActions.PlayerPlatforming.Move.StartedAsObservable())
                .Merge(_inputActions.PlayerPlatforming.Move.CanceledAsObservable())
                .Subscribe(OnMovePerformed).AddTo(_disposables);
            _inputActions.PlayerPlatforming.Jump.PerformedAsObservable()
                .Merge(_inputActions.PlayerPlatforming.Jump.StartedAsObservable())
                .Merge(_inputActions.PlayerPlatforming.Jump.CanceledAsObservable())
                .Subscribe(OnJumpPerformed).AddTo(_disposables);
            _inputActions.PlayerPlatforming.Interact.StartedAsObservable()
                .Subscribe(OnInteractPerformed).AddTo(_disposables);
            _inputActions.PlayerPlatforming.Pause.PerformedAsObservable()
                .Subscribe(OnPausePerformed).AddTo(_disposables);
            
            _inputActions.UI.Skip.PerformedAsObservable()
                .Merge( _inputActions.UI.Skip.StartedAsObservable())
                .Merge(_inputActions.UI.Skip.CanceledAsObservable())
                .Subscribe(OnSkipPressed).AddTo(_disposables);
            _inputActions.UI.Pause.PerformedAsObservable()
                .Subscribe(OnPausePerformed).AddTo(_disposables);
            _inputActions.UI.Click.PerformedAsObservable()
                .Merge( _inputActions.UI.Click.StartedAsObservable())
                .Merge(_inputActions.UI.Click.CanceledAsObservable())
                .Subscribe(OnUIMouseClickPerformed).AddTo(_disposables);
            _inputActions.PlayerPuzzle.Click.PerformedAsObservable()
                .Merge( _inputActions.PlayerPuzzle.Click.StartedAsObservable())
                .Merge(_inputActions.PlayerPuzzle.Click.CanceledAsObservable())
                .Subscribe(OnPuzzleMouseClickPerformed).AddTo(_disposables);
            _inputActions.PlayerPuzzle.Pause.PerformedAsObservable()
                .Subscribe(OnPausePerformed).AddTo(_disposables);
        }

        void IInputSystem.EnablePlatformingInput()
        {
            _inputActions.PlayerPlatforming.Enable();
            _inputActions.UI.Disable();
            _inputActions.PlayerPuzzle.Disable();
        }
        
        void IInputSystem.EnablePuzzleInput()
        {
            _inputActions.PlayerPlatforming.Disable();
            _inputActions.UI.Disable();
            _inputActions.PlayerPuzzle.Enable();
        }
        
        void IInputSystem.EnableUIInput()
        {
            _inputActions.PlayerPuzzle.Disable();
            _inputActions.PlayerPlatforming.Disable();
            _inputActions.UI.Enable();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            OnMove?.OnNext(new InputVectorActionData
            {
                Phase = context.phase,
                Vector = context.action.ReadValue<Vector2>()
            });
            SetupCurrentControlScheme(context);
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            OnInteract?.OnNext(new InputActionData
            {
                Phase = context.phase
            });
        }
        
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            OnJump?.OnNext(new InputActionData
            {
                Phase = context.phase
            });
            SetupCurrentControlScheme(context);
        }

        private void OnSkipPressed(InputAction.CallbackContext context)
        {
            OnSkipClicked?.OnNext(new InputActionData()
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

        private void OnPuzzleMouseClickPerformed(InputAction.CallbackContext context)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            OnPuzzleMouseClick?.OnNext(new InputVectorActionData()
            {
                Phase = context.phase,
                Vector = _inputActions.PlayerPuzzle.Point.ReadValue<Vector2>()
            });
        }
        
        private void OnUIMouseClickPerformed(InputAction.CallbackContext context)
        {
            OnUIMouseClick?.OnNext(new InputVectorActionData()
            {
                Phase = context.phase,
                Vector = _inputActions.PlayerPuzzle.Point.ReadValue<Vector2>()
            });
        }
        
        private void SetupCurrentControlScheme(InputAction.CallbackContext context)
        {
            var device = context.control.device;
            if (device is Keyboard || device is Mouse)
                CurrentControlScheme.Value = "Keyboard";
            else if (device is Gamepad)
                CurrentControlScheme.Value = "Gamepad";
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
