using System;
using _Project.Application.Asset;
using _Project.Application.Audio;
using _Project.Application.Input;
using cherrydev;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace _Project.Application.GUI.DialogeSystem
{
    public class DialogSystem : IInitializable, IDialogeSystem
    {
        [Inject] private IGUISystem _guiSystem;
        [Inject] private IAssetLoader _assetLoader;
        [Inject] private IAudioSystem _audioSystem;
        [Inject] private IInputSystem _inputSystem;
        
        public DialogBehaviour DialogBehaviour { get; private set; }
        
        void IInitializable.Initialize()
        {
            var dialogParent = _guiSystem.Canvas.CreateContainer("DialogGUI");
            var instance = _assetLoader.LoadImmediate<GameObject>("DialogGUI");
            
            DialogBehaviour = Object.Instantiate(instance, dialogParent).GetComponent<DialogBehaviour>();
        }

        void IDialogeSystem.DialogeStart(DialogNodeGraph graph, Action onDialogEnded)
        {
            _inputSystem.EnablePlayerInput(false);
            DialogBehaviour.StartDialog(graph, (_) => Debug.Log("Dialog intialized"), _ =>
            {
                _inputSystem.EnablePlayerInput(true);
                onDialogEnded?.Invoke();
            });
            DialogBehaviour.DialogTextCharWrote += OnCharWrote;
        }

        private void OnCharWrote()
        {
            _audioSystem.PlayOneShot("typeText");
        }
    }

    public interface IDialogeSystem
    {
        void DialogeStart(DialogNodeGraph graph, Action onDialogEnded);
    }
}
