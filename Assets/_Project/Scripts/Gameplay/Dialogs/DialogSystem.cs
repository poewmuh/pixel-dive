using _Project.Application.Asset;
using _Project.Application.GUI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Gameplay.Dialogs
{
    public class DialogSystem : IInitializable
    {
        [Inject] private IGUISystem _guiSystem;
        [Inject] private IAssetLoader _assetLoader;
        [Inject] private IObjectResolver _objectResolver;
        
        private DialogOverlay _dialogOverlay;
        
        void IInitializable.Initialize()
        {
            var dialogParent = _guiSystem.Canvas.CreateContainer("DialogGUI");
            var instance = _assetLoader.LoadImmediate<GameObject>("DialogGUI");
            
            _dialogOverlay = _objectResolver.Instantiate(instance, dialogParent).GetComponent<DialogOverlay>();
        }
    }
}
