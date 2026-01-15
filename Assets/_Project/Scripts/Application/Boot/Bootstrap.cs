using System;
using _Project.Application.Audio;
using _Project.Application.Scene;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace _Project.Application.Boot
{
    public class Bootstrap : MonoBehaviour
    {
        [Inject] private ISceneSystem _sceneSystem;
        [Inject] private LogoView _logoView;
        [Inject] private IAudioSystem _audioSystem;
        
        private void Start()
        {
            LoadMainMenu().Forget();
        }

        private async UniTaskVoid LoadMainMenu()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            _audioSystem.PlayMusic("mainmenu_theme");
            await _logoView.StartShowLogo();
            await _sceneSystem.LoadScene(SceneType.TrainingLocation, LoadSceneMode.Single);
        }
    }
}
