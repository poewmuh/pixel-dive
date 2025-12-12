using System;
using UnityEngine;

namespace _Project.Application.GUI.WindowsSystem
{
    public interface IWindow
    {
        event Action<IWindow> OnClosed;
        
        void Initialize(IWindowSystem windowSystem);
        void Show();
        void Close();
        GameObject GetDefaultSelectable();
    }
}
