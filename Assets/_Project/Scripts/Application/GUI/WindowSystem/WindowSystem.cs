using System.Collections.Generic;
using System.Linq;
using _Project.Application.Asset;
using R3;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Application.GUI.WindowsSystem
{
    public class WindowSystem : IWindowSystem, IInitializable
    {
        [Inject] private IGUISystem _guiSystem;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IAssetLoader _assetLoader;
        
        private readonly List<IWindow> _windowList = new();
        private RectTransform _windowsContainer;
        
        int IWindowSystem.WindowsCount => _windowList.Count;
        
        public Subject<IWindow> OnWindowOpened { get; } = new();
        public Subject<IWindow> OnWindowClosed { get; } = new();

        void IInitializable.Initialize()
        {
            _windowsContainer = _guiSystem.Canvas.CreateContainer("Windows");
        }

        T IWindowSystem.Show<T>()
        {
            var key = typeof(T).Name;
            var instance = _assetLoader.LoadImmediate<GameObject>(key);
            IWindow window = Object.Instantiate(instance, _windowsContainer).GetComponent<IWindow>();
            _objectResolver.Inject(window);
            window.Initialize(this);
            window.OnClosed += HandleOnWindowClosed;
            window.Show();
            
            _windowList.Add(window);
            OnWindowOpened.OnNext(window);
            return (T)window;
        }
        
        void IWindowSystem.Close(IWindow window) => Close(window);
        
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

        private void Close(IWindow window)
        {
            window.Close();
        }

        private void HandleOnWindowClosed(IWindow window)
        {
            window.OnClosed -= HandleOnWindowClosed;
            _windowList.Remove(window);
            OnWindowClosed.OnNext(window);
            _assetLoader.Release(nameof(window));
        }
    }
}
