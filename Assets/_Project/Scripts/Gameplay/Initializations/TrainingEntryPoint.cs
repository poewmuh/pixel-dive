using _Project.Application.GUI.WindowsSystem;
using _Project.Windows.MainMenu;
using Cysharp.Threading.Tasks;
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
            ShowMainMenu().Forget();
        }

        private async UniTaskVoid ShowMainMenu()
        {
            var window = await _windowSystem.Show<MainMenuWindow>();
            window.OnClosed += _ => _onPlayButtonPressed.Invoke();
        }
    }
}
