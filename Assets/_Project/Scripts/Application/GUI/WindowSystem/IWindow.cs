using System;
using UnityEngine;

namespace _Project.Application.GUI.WindowsSystem
{
    public interface IWindow
    {
        event Action<WindowBase> OnClosed;
        
        void Initialize(IWindowSystem windowSystem);
        void Show();
        void Close();
        void SetHidden(bool hidden);
        GameObject GetDefaultSelectable();
    }
}
