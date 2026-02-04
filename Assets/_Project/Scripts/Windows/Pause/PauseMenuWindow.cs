using _Project.Application.GUI.WindowsSystem;
using _Project.Application.Scene;
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
        
        [Inject] private ISceneSystem _sceneSystem;
        
        private void Start()
        {
            _continueButton.OnClickAsObservable().Subscribe(_ => ((IWindow)this).Close()).AddTo(this);
            _backToMainMenuButton.OnClickAsObservable().Subscribe(_ =>
            {
                ((IWindow)this).Close();
                _sceneSystem.LoadScene(SceneType.Bootstrap, LoadSceneMode.Single, Color.black);
            }).AddTo(this);
        }
    }
}
