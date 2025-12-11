using System;
using _Project.Application.Scene;
using _Project.Application.WindowSystem;
using UnityEngine;
using VContainer;

namespace _Project.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Inject] private IWindowSystem  _windowSystem;

        private void Start()
        {
            _windowSystem.Show<MainMenuWindow>();
        }
    }
}
