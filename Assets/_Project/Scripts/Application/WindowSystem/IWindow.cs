using System;
using UnityEngine;

namespace _Project.Application.WindowSystem
{
    public interface IWindow
    {
        event Action<WindowBase> OnClosed;
        
        void Initialize(IWindowSystem windowSystem);
        void Show();
        void Close();
        GameObject GetDefaultSelectable();
    }
}
