using System;
using _Project.Application.Asset;
using UnityEngine;
using Utils;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace _Project.Application.GUI
{
    public class GUISystem : IGUISystem, IInitializable, IDisposable
    {
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IAssetLoader _assetLoader;
        
        public CanvasBehaviour Canvas { get; private set; }

        void IInitializable.Initialize()
        {
            var instance = _assetLoader.LoadImmediate<GameObject>(nameof(CanvasBehaviour));
            Canvas = Object.Instantiate(instance).GetComponent<CanvasBehaviour>();
            Canvas.name = "GUI_Root";
            
            Object.DontDestroyOnLoad(Canvas);
        }

        void IDisposable.Dispose()
        {
            _assetLoader.Release(nameof(CanvasBehaviour));
        }
    }
}
