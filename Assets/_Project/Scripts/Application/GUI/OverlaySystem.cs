using System;
using System.Collections.Generic;
using _Project.Application.Asset;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace _Project.Application.GUI
{
    public class OverlaySystem : IOverlaySystem, IDisposable
    {
        [Inject] private IGUISystem _guiSystem;
        [Inject] private IAssetLoader _assetLoader;
        [Inject] private IObjectResolver _objectResolver;

        private readonly List<string> _loadedOverlayKeys = new();
        private Transform _overlaysParent;

        async UniTask IOverlaySystem.CreateOverlay(OverlayType type)
        {
            await EnsureInitialized();

            var key = type.ToString();
            var prefab = await _assetLoader.LoadAsync<GameObject>(key);
            _objectResolver.Instantiate(prefab, _overlaysParent);
            _loadedOverlayKeys.Add(key);
        }

        private async UniTask EnsureInitialized()
        {
            if (_overlaysParent != null)
            {
                return;
            }

            await _guiSystem.EnsureInitialized();
            _overlaysParent ??= _guiSystem.Canvas.CreateContainer("OverlaysGUI");
        }

        void IDisposable.Dispose()
        {
            foreach (var key in _loadedOverlayKeys)
            {
                _assetLoader.Release(key);
            }

            _loadedOverlayKeys.Clear();

            if (_overlaysParent != null)
            {
                Object.Destroy(_overlaysParent.gameObject);
            }
        }
    }

    public enum OverlayType
    {
        DialogGUI,
        CursorGUI,
        InventoryGUI,
        FadeGUI,
        OilGUI
    }
}
