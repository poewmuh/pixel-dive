using System;
using TriInspector;
using UnityEngine;

namespace _Project.Application.GUI.WindowsSystem
{
    public abstract class WindowBase : MonoBehaviour, IWindow
    {
        public event Action<WindowBase> OnClosed;
        
        [SerializeField, Required] private GameObject _defaultSelectable;
        
        protected IWindowSystem WindowSystem { get; private set; }
        
        void IWindow.Initialize(IWindowSystem windowSystem)
        {
            WindowSystem = windowSystem;
        }

        void IWindow.Show()
        {
            InternalShow();
        }

        void IWindow.Close()
        {
            InternalClose();
            OnClosed?.Invoke(this);
        }

        void IWindow.SetHidden(bool hidden)
        {
            if (hidden)
            {
                InternalClose();
            }
            else
            {
                InternalShow();
            }
        }
        
        protected virtual void InternalShow()
        {
            gameObject.SetActive(true);
        }
        
        public virtual void InternalClose()
        {
            gameObject.SetActive(false);
        }
        
        protected virtual void OnDestroy()
        {
            OnClosed = null;
        }
        
        GameObject IWindow.GetDefaultSelectable() => _defaultSelectable;
    }
}
