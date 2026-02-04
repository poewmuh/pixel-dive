using _Project.Application.Asset;
using _Project.Application.GUI.WindowsSystem;
using _Project.Application.Messages;
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
        
        private string _currentSceneKey = string.Empty;
        
        async UniTask ISceneSystem.LoadScene(SceneType scene, LoadSceneMode sceneMode, Color fadeColor)
        {
            _sceneChangePub?.Publish(new SceneChangeEvent(scene));
            var sceneKey = scene.ToString();
            var loadingWindow = _windowSystem.Show<LoadingWindow>();
            await loadingWindow.StartShowAnimation(fadeColor);
            await _assetLoader.LoadSceneAsync(sceneKey, sceneMode);
            if (scene is not SceneType.Bootstrap)
            {
                await loadingWindow.StartHideAnimation();
            }

            _windowSystem.Close(loadingWindow);
            _assetLoader.Release(_currentSceneKey);
            _currentSceneKey = sceneKey;
        }
    }
}
