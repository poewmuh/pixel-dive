using _Project.Application.Asset;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using VContainer;

namespace _Project.Application.Scene
{
    public class SceneSystem : ISceneSystem
    {
        [Inject] private IAssetLoader _assetLoader;
        
        private string _currentSceneKey = string.Empty;
        
        async UniTask ISceneSystem.LoadScene(SceneType scene, LoadSceneMode sceneMode)
        {
            var sceneKey = scene.ToString();
            await _assetLoader.LoadSceneAsync(sceneKey, sceneMode);
            _assetLoader.Release(_currentSceneKey);
            _currentSceneKey = sceneKey;
        }
    }
}
