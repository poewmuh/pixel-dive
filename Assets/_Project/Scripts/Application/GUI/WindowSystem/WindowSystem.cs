using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Application.Asset;
using _Project.Application.Input;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace _Project.Application.GUI.WindowsSystem
{
    public class WindowSystem : IWindowSystem, IInitializable, IDisposable
    {
        [Inject] private IGUISystem _guiSystem;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IAssetLoader _assetLoader;
        [Inject] private IInputSwitcher _inputSwitcher;
        
        private readonly List<IWindow> _windowList = new();
        private RectTransform _windowsContainer;
        
        int IWindowSystem.WindowsCount => _windowList.Count;
        
        public Subject<IWindow> OnWindowOpened { get; } = new();
        public Subject<IWindow> OnWindowClosed { get; } = new();
        
        private readonly CompositeDisposable _disposables = new();

        void IInitializable.Initialize()
        {
            _windowsContainer = _guiSystem.Canvas.CreateContainer("Windows");
            
            OnWindowOpened.AddTo(_disposables);
            OnWindowClosed.AddTo(_disposables);
        }

        T IWindowSystem.Show<T>()
        {
            _inputSwitcher.EnableInput(InputBlockMask.Windows, true);
            var key = typeof(T).Name;
            var instance = _assetLoader.LoadImmediate<GameObject>(key);
            IWindow window = _objectResolver.Instantiate(instance, _windowsContainer).GetComponent<IWindow>();
            window.Initialize(this);
            window.OnClosed += HandleOnWindowClosed;
            window.Show();

            if (_windowList.Count > 0)
            {
                var lastWindow = _windowList.Last();
                lastWindow.SetHidden(true);
            }
            
            _windowList.Add(window);
            OnWindowOpened.OnNext(window);
            return (T)window;
        }
        
        void IWindowSystem.Close(IWindow window) => Close(window);

        void IWindowSystem.SetHidden<T>(bool isHidden)
        {
            foreach (var window in _windowList)
            {
                if (window is T)
                {
                    window.SetHidden(isHidden);
                    return;
                }
            }
        }
        
        void IWindowSystem.Close<T>()
        {
            foreach (var window in _windowList)
            {
                if (window is T)
                {
                    Close(window);
                    return;
                }
            }
        }
        
        void IWindowSystem.CloseAll()
        {
            var windowsToClose = new List<IWindow>(_windowList);
            foreach (var window in windowsToClose)
            {
                window.Close();
            }

            _windowList.Clear();
        }
        
        void IWindowSystem.CloseLast()
        {
            if (_windowList.Count <= 0) return;
            var lastWindow = _windowList.Last();
            Close(lastWindow);
        }

        bool IWindowSystem.IsShowing<T>()
        {
            foreach (var window in _windowList)
            {
                if (window is T)
                {
                    return true;
                }
            }

            return false;
        }

        private void Close(IWindow window)
        {
            window.Close();
            
            if (_windowList.Count > 0)
            {
                var lastWindow = _windowList.Last();
                lastWindow.SetHidden(false);
            }
        }

        private void HandleOnWindowClosed(WindowBase window)
        {
            window.OnClosed -= HandleOnWindowClosed;
            _windowList.Remove(window);
            OnWindowClosed.OnNext(window);
            _assetLoader.Release(window.GetType().Name);
            Object.Destroy(window.gameObject);

            if (((IWindowSystem)this).WindowsCount == 0)
            {
                _inputSwitcher.EnableInput(InputBlockMask.Windows, false);
            }
        }

        void IDisposable.Dispose()
        {
            _disposables.Dispose();
        }
    }
}
