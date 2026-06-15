using _Project.Application.Asset;
using _Project.Application.GUI.WindowsSystem;
using _Project.Application.Messages;
using _Project.Application.Persistence;
using _Project.Windows.Loading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace _Project.Application.Scene
{
    public class SceneSystem : ISceneSystem
    {
        [Inject] private IAssetLoader _assetLoader;
        [Inject] private IPublisher<SceneChangeEvent> _sceneChangePub;
        [Inject] private IWindowSystem _windowSystem;
        [Inject] private IGameSaveMediator _gameSaveMediator;
        
        private string _currentSceneKey = string.Empty;
        private bool _isSceneLoading;
        
        async UniTask ISceneSystem.LoadScene(SceneType scene, LoadSceneMode sceneMode, Color fadeColor)
        {
            if (_isSceneLoading) return;

            _isSceneLoading = true;
            LoadingWindow loadingWindow = null;

            try
            {
                _sceneChangePub?.Publish(new SceneChangeEvent(scene));
                var sceneKey = scene.ToString();
                loadingWindow = _windowSystem.Show<LoadingWindow>();

                await loadingWindow.StartShowAnimation(fadeColor);
                await _assetLoader.LoadSceneAsync(sceneKey, sceneMode);

                if (scene is not SceneType.Bootstrap)
                {
                    _gameSaveMediator.GetLevelSaveData().SetSceneType(scene);
                    _gameSaveMediator.Save();
                    await loadingWindow.StartHideAnimation();
                }

                _assetLoader.Release(_currentSceneKey);
                _currentSceneKey = sceneKey;
            }
            finally
            {
                if (loadingWindow != null)
                {
                    _windowSystem.Close(loadingWindow);
                }

                _isSceneLoading = false;
            }
        }
    }
}
