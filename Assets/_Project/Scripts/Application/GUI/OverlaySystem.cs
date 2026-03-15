using System;
using _Project.Application.Asset;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace _Project.Application.GUI
{
    public class OverlaySystem : IInitializable, IOverlaySystem, IDisposable
    {
        [Inject] private IGUISystem _guiSystem;
        [Inject] private IAssetLoader _assetLoader;
        [Inject] private IObjectResolver _objectResolver;
        
        private Transform _overlaysParent;
        
        void IInitializable.Initialize()
        {
            _overlaysParent = _guiSystem.Canvas.CreateContainer("OverlaysGUI");
        }
        
        void IOverlaySystem.CreateOverlay(OverlayType type)
        {
            var instance = _assetLoader.LoadImmediate<GameObject>(type.ToString());
            _objectResolver.Instantiate(instance, _overlaysParent);
        }

        void IDisposable.Dispose()
        {
            Object.Destroy(_overlaysParent.gameObject);
        }
    }

    public enum OverlayType
    {
        DialogGUI,
        CursorGUI,
        InventoryGUI,
        FadeGUI
    }
}
