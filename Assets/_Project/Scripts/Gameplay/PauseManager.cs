using System;
using _Project.Application.GUI.WindowsSystem;
using _Project.Application.Input;
using _Project.Windows.Pause;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace _Project.Gameplay
{
    public class PauseManager : IInitializable, IDisposable
    {
        [Inject] private IWindowSystem _windowSystem;
        [Inject] private IInputSystem _inputSystem;

        private readonly CompositeDisposable _disposables = new();

        void IInitializable.Initialize()
        {
            _inputSystem.OnPause.Subscribe(OnPause).AddTo(_disposables);
        }

        private void OnPause(InputActionData data)
        {
            if (data.Phase == InputActionPhase.Performed)
            {
                if (_windowSystem.IsShowing<PauseMenuWindow>())
                {
                    _windowSystem.CloseLast();
                }
                else
                {
                    _windowSystem.Show<PauseMenuWindow>().Forget();
                }
            }
        }

        void IDisposable.Dispose()
        {
            _disposables.Dispose();
        }
    }
}
