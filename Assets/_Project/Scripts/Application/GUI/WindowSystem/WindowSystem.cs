using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Application.Asset;
using _Project.Application.Input;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace _Project.Application.GUI.WindowsSystem
{
    public class WindowSystem : IWindowSystem, IDisposable
    {
        [Inject] private IGUISystem _guiSystem;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IAssetLoader _assetLoader;
        [Inject] private IInputSwitcher _inputSwitcher;

        private readonly List<IWindow> _windows = new();
        private readonly HashSet<Type> _pendingWindowTypes = new();
        private readonly CompositeDisposable _disposables = new();

        private RectTransform _windowsContainer;

        int IWindowSystem.WindowsCount => _windows.Count;

        public Subject<IWindow> OnWindowOpened { get; } = new();
        public Subject<IWindow> OnWindowClosed { get; } = new();

        public WindowSystem()
        {
            OnWindowOpened.AddTo(_disposables);
            OnWindowClosed.AddTo(_disposables);
        }

        async UniTask<T> IWindowSystem.Show<T>()
        {
            var windowType = typeof(T);
            if (!_pendingWindowTypes.Add(windowType))
            {
                await UniTask.WaitUntil(() => !_pendingWindowTypes.Contains(windowType));
                var existingWindow = _windows.LastOrDefault(window => window is T) as T;
                if (existingWindow != null)
                {
                    return existingWindow;
                }

                throw new InvalidOperationException($"Window '{windowType.Name}' failed to open.");
            }

            UpdateInputBlock();

            try
            {
                await EnsureInitialized();

                var key = windowType.Name;
                var prefab = await _assetLoader.LoadAsync<GameObject>(key);
                var gameObject = _objectResolver.Instantiate(prefab, _windowsContainer);
                var window = gameObject.GetComponent<T>();

                if (window == null)
                {
                    Object.Destroy(gameObject);
                    _assetLoader.Release(key);
                    throw new InvalidOperationException(
                        $"Window prefab '{key}' does not contain component {windowType.Name}.");
                }

                if (_windows.Count > 0)
                {
                    _windows[^1].SetHidden(true);
                }

                var windowContract = (IWindow)window;
                windowContract.Initialize(this);
                windowContract.OnClosed += HandleWindowClosed;
                _windows.Add(windowContract);
                windowContract.Show();
                OnWindowOpened.OnNext(windowContract);
                return window;
            }
            finally
            {
                _pendingWindowTypes.Remove(windowType);
                UpdateInputBlock();
            }
        }

        void IWindowSystem.ShowAndForget<T>()
        {
            ShowAndForget<T>().Forget();
        }

        void IWindowSystem.Close(IWindow window)
        {
            Close(window);
        }

        void IWindowSystem.Close<T>()
        {
            var window = _windows.LastOrDefault(window => window is T);
            Close(window);
        }

        void IWindowSystem.CloseAll()
        {
            foreach (var window in _windows.ToArray())
            {
                window.Close();
            }
        }

        void IWindowSystem.CloseLast()
        {
            Close(_windows.LastOrDefault());
        }

        bool IWindowSystem.IsShowing<T>()
        {
            return _pendingWindowTypes.Contains(typeof(T)) || _windows.Any(window => window is T);
        }

        private async UniTaskVoid ShowAndForget<T>() where T : WindowBase
        {
            try
            {
                await ((IWindowSystem)this).Show<T>();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private async UniTask EnsureInitialized()
        {
            if (_windowsContainer != null)
            {
                return;
            }

            await _guiSystem.EnsureInitialized();
            _windowsContainer ??= _guiSystem.Canvas.CreateContainer("Windows");
        }

        private static void Close(IWindow window)
        {
            window?.Close();
        }

        private void HandleWindowClosed(WindowBase window)
        {
            window.OnClosed -= HandleWindowClosed;
            _windows.Remove(window);
            OnWindowClosed.OnNext(window);
            _assetLoader.Release(window.GetType().Name);
            Object.Destroy(window.gameObject);

            if (_windows.Count > 0)
            {
                _windows[^1].SetHidden(false);
            }

            UpdateInputBlock();
        }

        private void UpdateInputBlock()
        {
            var shouldBlockInput = _windows.Count > 0 || _pendingWindowTypes.Count > 0;
            _inputSwitcher.EnableInput(InputBlockMask.Windows, shouldBlockInput);
        }

        void IDisposable.Dispose()
        {
            ((IWindowSystem)this).CloseAll();
            _disposables.Dispose();
        }
    }
}
