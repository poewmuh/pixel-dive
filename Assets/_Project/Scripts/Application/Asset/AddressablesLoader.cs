using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace _Project.Application.Asset
{
    public class AddressablesLoader : IAssetLoader
    {
        private readonly Dictionary<string, AsyncOperationHandle> _handles = new();
        
        public async UniTask<T> LoadAsync<T>(string key) where T : UnityEngine.Object
        {
            if (_handles.TryGetValue(key, out var existHandle))
            {
                if (existHandle.IsValid()) return existHandle.Result as T;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _handles[key] = handle;
                return handle.Result;
            }
            
            Debug.LogError($"[AddressablesLoader] Failed to load asset with key: {key}.");
            return null;
        }
        
        public async UniTask<SceneInstance> LoadSceneAsync(string key, LoadSceneMode mode)
        {
            if (_handles.TryGetValue(key, out var existHandle))
            {
                if (existHandle.IsValid()) return (SceneInstance)existHandle.Result;
            }

            var handle = Addressables.LoadSceneAsync(key, mode);
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _handles[key] = handle;
                return handle.Result;
            }
            
            Debug.LogError($"[AddressablesLoader] Failed to load scene with key: {key}.");
            return default;
        }
        
        public T LoadImmediate<T>(string key) where T : UnityEngine.Object
        {
            if (_handles.TryGetValue(key, out var existHandle))
            {
                if (existHandle.IsValid()) return existHandle.Result as T;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _handles[key] = handle;
                return handle.Result;
            }
            
            Debug.LogError($"[AddressablesLoader] Failed to load asset with key: {key}.");
            return null;
        }
        
        public void Release(string key)
        {
            if (!_handles.TryGetValue(key, out var handle)) return;
            
            if (handle.IsValid())
                Addressables.Release(handle);

            _handles.Remove(key);
        }
        
        public void Dispose()
        {
            foreach (var handle in _handles.Values)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
            
            _handles.Clear();
        }
    }
}
