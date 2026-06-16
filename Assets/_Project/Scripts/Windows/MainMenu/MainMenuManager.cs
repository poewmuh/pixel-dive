using _Project.Application.GUI.WindowsSystem;
using UnityEngine;
using VContainer;

namespace _Project.Windows.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Inject] private IWindowSystem _windowSystem;

        private void Start()
        {
            _windowSystem.ShowAndForget<MainMenuWindow>();
        }
    }
}
