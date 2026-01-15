using _Project.Application.Asset;
using _Project.Application.Messages;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine.SceneManagement;
using VContainer;

namespace _Project.Application.Scene
{
    public class SceneSystem : ISceneSystem
    {
        [Inject] private IAssetLoader _assetLoader;
        [Inject] private IPublisher<SceneChangeEvent> _sceneChangePub;
        
        private string _currentSceneKey = string.Empty;
        
        async UniTask ISceneSystem.LoadScene(SceneType scene, LoadSceneMode sceneMode)
        {
            _sceneChangePub?.Publish(new SceneChangeEvent(scene));
            var sceneKey = scene.ToString();
            await _assetLoader.LoadSceneAsync(sceneKey, sceneMode);
            _assetLoader.Release(_currentSceneKey);
            _currentSceneKey = sceneKey;
        }
    }
}
