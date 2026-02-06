using _Project.Application.Audio;
using _Project.Application.GUI.WindowsSystem;
using _Project.Application.Scene;
using _Project.Windows.MainMenu;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace _Project.Windows.Pause
{
    public class PauseMenuWindow : WindowBase
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _backToMainMenuButton;
        [SerializeField] private Button _settingsButton;
        
        [Inject] private ISceneSystem _sceneSystem;
        [Inject] private IWindowSystem _windowSystem;
        [Inject] private IAudioSystem _audioSystem;
        
        private void Start()
        {
            _continueButton.OnClickAsObservable().Subscribe(_ => ((IWindow)this).Close()).AddTo(this);
            _settingsButton.OnClickAsObservable().Subscribe(_ => _windowSystem.Show<SettingsWindow>());
            _backToMainMenuButton.OnClickAsObservable().Subscribe(_ =>
            {
                ((IWindow)this).Close();
                _sceneSystem.LoadScene(SceneType.Bootstrap, LoadSceneMode.Single, Color.black);
            }).AddTo(this);
        }

        protected override void InternalShow(bool isHidden)
        {
            base.InternalShow(isHidden);
            if (isHidden) return;
            Time.timeScale = 0;
            _audioSystem.SetLowpassValue(.1f);
        }

        protected override void InternalClose(bool isHidden)
        {
            base.InternalClose(isHidden);
            if (isHidden) return;
            Time.timeScale = 1;
            _audioSystem.SetLowpassValue(1);
        }
    }
}
