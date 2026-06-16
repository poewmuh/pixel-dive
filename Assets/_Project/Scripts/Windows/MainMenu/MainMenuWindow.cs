using _Project.Application.GUI.WindowsSystem;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Windows.MainMenu
{
    public class MainMenuWindow : WindowBase
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitGameButton;
        
        private void Start()
        {
            _startGameButton.OnClickAsObservable().Subscribe(OnStartGame).AddTo(this);
            _settingsButton.OnClickAsObservable().Subscribe(OnSettings).AddTo(this);
            _exitGameButton.OnClickAsObservable().Subscribe(OnExitGame).AddTo(this);
        }

        private void OnStartGame(Unit _)
        {
            WindowSystem.Close(this);
        }

        private void OnSettings(Unit _)
        {
            WindowSystem.ShowAndForget<SettingsWindow>();
        }

        private void OnExitGame(Unit _)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
			UnityEngine.Application.Quit();
#endif
        }
    }
}
