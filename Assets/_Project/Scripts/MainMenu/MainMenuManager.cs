using _Project.Application.GUI.WindowsSystem;
using UnityEngine;
using VContainer;

namespace _Project.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Inject] private IWindowSystem _windowSystem;

        private void Start()
        {
            _windowSystem.Show<MainMenuWindow>();
        }
    }
}
