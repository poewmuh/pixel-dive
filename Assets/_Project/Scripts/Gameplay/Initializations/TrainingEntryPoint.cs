using _Project.Application.GUI.WindowsSystem;
using _Project.Windows.MainMenu;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace _Project.Gameplay.Initializations
{
    public class TrainingEntryPoint : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onPlayButtonPressed;
        
        [Inject] private IWindowSystem _windowSystem;
        
        private void Awake()
        {
            var window = _windowSystem.Show<MainMenuWindow>();
            window.OnClosed += _ => _onPlayButtonPressed.Invoke();
        }
    }
}