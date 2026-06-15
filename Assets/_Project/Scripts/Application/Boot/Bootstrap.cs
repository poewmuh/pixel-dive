using System;
using _Project.Application.Audio;
using _Project.Application.Persistence;
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
        [Inject] private IGameSaveMediator _gameSaveMediator;
        [Inject] private IGameSaveSystem _gameSaveSystem;
        
        private void Start()
        {
            LoadMainMenu().Forget();
        }

        private async UniTaskVoid LoadMainMenu()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            _audioSystem.PlayMusic("mainmenu_theme");
            await _logoView.StartShowLogo();

            if (_gameSaveSystem.MetaData.Saves.Count > 0)
            {
                _gameSaveSystem.LoadSave(_gameSaveSystem.MetaData.Saves[0]);
            }
            else
            {
                _gameSaveSystem.CreateNewSave();
            }
            var savedScene = _gameSaveMediator.GetSavedScene();
            await _sceneSystem.LoadScene(savedScene, LoadSceneMode.Single, Color.black);
        }
    }
}
