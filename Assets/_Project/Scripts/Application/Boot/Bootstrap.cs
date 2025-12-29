using System;
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
        
        private void Start()
        {
            LoadMainMenu().Forget();
        }

        private async UniTaskVoid LoadMainMenu()
        {
            await _logoView.StartShowLogo();
            await _sceneSystem.LoadScene(SceneType.TrainingLocation, LoadSceneMode.Single);
        }
    }
}
