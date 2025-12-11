using _Project.Application.Scene;
using _Project.Application.WindowSystem;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace _Project.MainMenu
{
    public class MainMenuWindow : WindowBase
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _exitGameButton;
        
        [Inject] private ISceneSystem _sceneSystem;

        private void Start()
        {
            _startGameButton.OnClickAsObservable().Subscribe(OnStartGame).AddTo(this);
            _exitGameButton.OnClickAsObservable().Subscribe(OnExitGame).AddTo(this);
        }

        private void OnStartGame(Unit _)
        {
            _sceneSystem.LoadScene(SceneType.TrainingLocation, LoadSceneMode.Single);
            WindowSystem.Close(this);
        }

        private void OnExitGame(Unit _)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
        }
    }
}
