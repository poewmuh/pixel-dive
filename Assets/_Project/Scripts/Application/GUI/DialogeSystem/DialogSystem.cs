using System;
using _Project.Application.Asset;
using _Project.Application.Audio;
using _Project.Application.Input;
using _Project.Application.Messages;
using _Project.Gameplay.Dialogs;
using _Project.Gameplay.Dialogs.Core;
using Cysharp.Threading.Tasks;
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
        
        private bool _cutsceneDialogSkipped;
        private Action _cutsceneDialogEndedCallback;

        void IInitializable.Initialize()
        {
            var dialogParent = _guiSystem.Canvas.CreateContainer("DialogGUI");
            var instance = _assetLoader.LoadImmediate<GameObject>("DialogGUI");
            
            _dialogBehaviour = Object.Instantiate(instance, dialogParent).GetComponent<DialogBehaviour>();
            _sceneChangeSub.Subscribe(OnSceneChange);
        }

        void IDialogeSystem.DialogStart(RuntimeDialogGraph graph, Action onDialogEnded, RuntimeVariableStorage variables = null)
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
            }, variables);
        }
        
        void IDialogeSystem.DelayDialogStart(RuntimeDialogGraph graph, Action onDialogEnded, float delay, RuntimeVariableStorage variables = null)
        {
            _inputSwitcher.EnableInput(InputBlockMask.Dialogs, true);
            StartWithDelay(graph, onDialogEnded, delay, variables).Forget();
        }

        private async UniTaskVoid StartWithDelay(RuntimeDialogGraph graph, Action onDialogEnded, float delay, RuntimeVariableStorage variables = null)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            ((IDialogeSystem)this).DialogStart(graph, onDialogEnded, variables);
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
        
        void IDialogeSystem.CutsceneDialogStart(string actorName, string message, EmotionType emotion, float letterDelay)
        {
            _dialogBehaviour.FinishDialogs();
            _inputSwitcher.EnableInput(InputBlockMask.Dialogs, true);
            _uiClickSubscription = _inputSystem.OnUIClick.Subscribe(OnSkipCutsceneDialog);
            _dialogBehaviour.DialogMessageView.OnCharWrote += OnCharWrote;
            _dialogBehaviour.StartCutsceneDialog(actorName, message, emotion, letterDelay, () =>
            {
                _uiClickSubscription?.Dispose();
                _inputSwitcher.EnableInput(InputBlockMask.Dialogs, false);
                _dialogBehaviour.DialogMessageView.OnCharWrote -= OnCharWrote;
            });
        }

        private void OnSkipCutsceneDialog(InputActionData data)
        {
            _dialogBehaviour.SkipCutsceneDialog();
        }

        bool IDialogeSystem.IsTyping()
        {
            return _dialogBehaviour.DialogMessageView.IsTyping;
        }
        
        bool IDialogeSystem.IsSkipped()
        {
            return !_dialogBehaviour.DialogMessageView.IsTyping && !_dialogBehaviour.DialogMessageView.gameObject.activeSelf;
        }
    }

    public interface IDialogeSystem
    {
        void DialogStart(RuntimeDialogGraph graph, Action onDialogEnded, RuntimeVariableStorage variables = null);
        void DelayDialogStart(RuntimeDialogGraph graph, Action onDialogEnded, float delay, RuntimeVariableStorage variables = null);
        void CutsceneDialogStart(string actorName, string message, EmotionType emotion, float letterDelay = -1);
        bool IsTyping();
        bool IsSkipped();
    }
}
