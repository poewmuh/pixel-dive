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
        
        private void Start()
        {
            LoadMainMenu().Forget();
        }

        private async UniTaskVoid LoadMainMenu()
        {
            await _sceneSystem.LoadScene(SceneType.MainMenu, LoadSceneMode.Single);
        }
    }
}
