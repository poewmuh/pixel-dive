using System;
using _Project.Application.Asset;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace _Project.Application.GUI
{
    public class GUISystem : IGUISystem, IDisposable
    {
        [Inject] private IAssetLoader _assetLoader;

        private UniTaskCompletionSource _initializationSource;

        public CanvasBehaviour Canvas { get; private set; }

        async UniTask IGUISystem.EnsureInitialized()
        {
            if (Canvas != null)
            {
                return;
            }

            if (_initializationSource != null)
            {
                await _initializationSource.Task;
                return;
            }

            _initializationSource = new UniTaskCompletionSource();

            try
            {
                var prefab = await _assetLoader.LoadAsync<GameObject>(nameof(CanvasBehaviour));
                Canvas = Object.Instantiate(prefab).GetComponent<CanvasBehaviour>();
                Canvas.name = "GUI_Root";
                Object.DontDestroyOnLoad(Canvas);
                _initializationSource.TrySetResult();
            }
            catch (Exception exception)
            {
                _initializationSource.TrySetException(exception);
                _initializationSource = null;
                throw;
            }
        }

        void IDisposable.Dispose()
        {
            if (Canvas != null)
            {
                Object.Destroy(Canvas.gameObject);
            }

            _assetLoader.Release(nameof(CanvasBehaviour));
        }
    }
}
