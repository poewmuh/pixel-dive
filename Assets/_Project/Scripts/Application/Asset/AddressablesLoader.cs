using System;
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
        private readonly Dictionary<AssetKey, AssetHandleEntry> _assetHandles = new();
        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _sceneHandles = new();

        public async UniTask<T> LoadAsync<T>(string key) where T : UnityEngine.Object
        {
            var assetKey = new AssetKey(key, typeof(T));
            if (TryGetAsset(assetKey, out T cachedAsset))
            {
                return cachedAsset;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            try
            {
                await handle.ToUniTask();
            }
            catch (Exception exception)
            {
                ReleaseFailedHandle(handle);
                throw CreateLoadException("asset", key, exception);
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                var exception = handle.OperationException;
                ReleaseFailedHandle(handle);
                throw CreateLoadException("asset", key, exception);
            }

            _assetHandles[assetKey] = new AssetHandleEntry(handle);
            return handle.Result;
        }

        public async UniTask<SceneInstance> LoadSceneAsync(string key, LoadSceneMode mode)
        {
            if (_sceneHandles.TryGetValue(key, out var cachedHandle) && cachedHandle.IsValid())
            {
                return cachedHandle.Result;
            }

            _sceneHandles.Remove(key);
            var handle = Addressables.LoadSceneAsync(key, mode);

            try
            {
                await handle.ToUniTask();
            }
            catch (Exception exception)
            {
                ReleaseFailedHandle(handle);
                throw CreateLoadException("scene", key, exception);
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                var exception = handle.OperationException;
                ReleaseFailedHandle(handle);
                throw CreateLoadException("scene", key, exception);
            }

            _sceneHandles[key] = handle;
            return handle.Result;
        }

        public void Release(string key)
        {
            ReleaseAssets(key);
            ReleaseScene(key);
        }

        public void Dispose()
        {
            foreach (var entry in _assetHandles.Values)
            {
                ReleaseHandle(entry.Handle);
            }

            foreach (var handle in _sceneHandles.Values)
            {
                ReleaseHandle(handle);
            }

            _assetHandles.Clear();
            _sceneHandles.Clear();
        }

        private bool TryGetAsset<T>(AssetKey assetKey, out T asset) where T : UnityEngine.Object
        {
            if (_assetHandles.TryGetValue(assetKey, out var entry))
            {
                if (entry.Handle.IsValid() && entry.Handle.Result is T cachedAsset)
                {
                    entry.AddReference();
                    asset = cachedAsset;
                    return true;
                }

                _assetHandles.Remove(assetKey);
            }

            asset = null;
            return false;
        }

        private void ReleaseAssets(string key)
        {
            var matchingKeys = new List<AssetKey>();
            foreach (var assetKey in _assetHandles.Keys)
            {
                if (assetKey.Key == key)
                {
                    matchingKeys.Add(assetKey);
                }
            }

            foreach (var assetKey in matchingKeys)
            {
                var entry = _assetHandles[assetKey];
                if (entry.RemoveReference() > 0)
                {
                    continue;
                }

                ReleaseHandle(entry.Handle);
                _assetHandles.Remove(assetKey);
            }
        }

        private void ReleaseScene(string key)
        {
            if (!_sceneHandles.Remove(key, out var handle))
            {
                return;
            }

            ReleaseHandle(handle);
        }

        private static void ReleaseFailedHandle<T>(AsyncOperationHandle<T> handle)
        {
            ReleaseHandle(handle);
        }

        private static void ReleaseHandle(AsyncOperationHandle handle)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

        private static InvalidOperationException CreateLoadException(
            string resourceType,
            string key,
            Exception innerException)
        {
            return new InvalidOperationException(
                $"[AddressablesLoader] Failed to load {resourceType} with key '{key}'.",
                innerException);
        }

        private readonly struct AssetKey : IEquatable<AssetKey>
        {
            public AssetKey(string key, Type type)
            {
                Key = key;
                Type = type;
            }

            public string Key { get; }
            private Type Type { get; }

            public bool Equals(AssetKey other)
            {
                return Key == other.Key && Type == other.Type;
            }

            public override bool Equals(object obj)
            {
                return obj is AssetKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Key, Type);
            }
        }

        private sealed class AssetHandleEntry
        {
            private int _referenceCount = 1;

            public AssetHandleEntry(AsyncOperationHandle handle)
            {
                Handle = handle;
            }

            public AsyncOperationHandle Handle { get; }

            public void AddReference()
            {
                _referenceCount++;
            }

            public int RemoveReference()
            {
                return --_referenceCount;
            }
        }
    }
}
