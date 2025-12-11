using System;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace _Project.Application.Asset
{
    public interface IAssetLoader : IDisposable
    {
        UniTask<T> LoadAsync<T>(string key) where T : UnityEngine.Object;
        UniTask<SceneInstance> LoadSceneAsync(string key, LoadSceneMode mode);
        T LoadImmediate<T>(string key) where T : UnityEngine.Object;
        void Release(string key);
    }
}
