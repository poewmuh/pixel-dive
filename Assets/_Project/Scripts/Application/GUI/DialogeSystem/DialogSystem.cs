using System;
using _Project.Application.Asset;
using _Project.Application.Audio;
using _Project.Application.Input;
using _Project.Application.Messages;
using _Project.Gameplay.Dialogs;
using _Project.Gameplay.Dialogs.Core;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [Inject] private IInputSwitcher _inputSwitcher;
        [Inject] private ISubscriber<SceneChangeEvent> _sceneChangeSub;

        private DialogBehaviour _dialogBehaviour;
        private IDisposable _uiClickSubscription;
        private IDisposable _sceneChangeSubscription;

        public bool IsDialogActive { get; private set; }

        void IInitializable.Initialize()
        {
            var dialogParent = _guiSystem.Canvas.CreateContainer("DialogGUI");
            var instance = _assetLoader.LoadImmediate<GameObject>("DialogGUI");
            
            _dialogBehaviour = Object.Instantiate(instance, dialogParent).GetComponent<DialogBehaviour>();
            _sceneChangeSubscription = _sceneChangeSub.Subscribe(OnSceneChange);
        }

        void IDialogeSystem.DialogeStart(RuntimeDialogGraph graph, Action onDialogEnded)
        {
            _inputSwitcher.EnableInput(InputBlockMask.Dialogs, true);
            _uiClickSubscription = _inputSystem.OnUIClick.Subscribe(OnSkipDialog);
            _dialogBehaviour.DialogMessageView.OnCharWrote += OnCharWrote;
            _dialogBehaviour.StartDialog(graph, () =>
            {
                _uiClickSubscription?.Dispose();
                _inputSwitcher.EnableInput(InputBlockMask.Dialogs, false);
                _dialogBehaviour.DialogMessageView.OnCharWrote -= OnCharWrote;
                onDialogEnded?.Invoke();
            });
        }
        
        private void OnSceneChange(SceneChangeEvent msg)
        {
            _dialogBehaviour.DialogMessageView.OnCharWrote -= OnCharWrote;
            _dialogBehaviour.CloseAllDialogs();
        }

        private void OnSkipDialog(InputActionData data)
        {
            _dialogBehaviour.SkipDialog();
        }

        private void OnCharWrote()
        {
            _audioSystem.PlayOneShot("typeText");
        }
    }

    public interface IDialogeSystem
    {
        bool IsDialogActive { get; }
        void DialogeStart(RuntimeDialogGraph graph, Action onDialogEnded);
    }
}
