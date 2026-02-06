using System;
using _Project.Application.Audio;
using _Project.Application.Input;
using _Project.Application.Localization;
using _Project.GUI;
using NodeCanvas.DialogueTrees;
using NodeCanvas.DialogueTrees.UI.Examples;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using VContainer;

namespace _Project.Gameplay.Dialogs
{
    [RequireComponent(typeof(DialogueUGUI))]
    public class DialogOverlay : BaseOverlay
    {
        [Inject] private IInputSwitcher _inputSwitcher;
        [Inject] private IInputSystem _inputSystem;
        [Inject] private IAudioSystem _audioSystem;

        private DialogueUGUI _dialogueUGUI;

        private void Awake()
        {
            _dialogueUGUI = GetComponent<DialogueUGUI>();
            _dialogueUGUI.SetupAudioSource(_audioSystem.GetSFXSource());
            UpdateLanguage();
        }

        private void Start()
        {
            DialogueTree.OnDialogueStarted += OnDialogueStarted;
            DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest;
            DialogueTree.OnDialogueFinished += OnDialogueFinished;
            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChange;
            _inputSystem.OnSkipClicked.Subscribe(OnSkipClicked).AddTo(this);
        }

        private void OnSelectedLocaleChange(Locale locale)
        {
            UpdateLanguage();
        }

        private void OnDialogueStarted(DialogueTree dialogueTree)
        {
            _inputSwitcher.EnableInput(InputBlockMask.Dialogs, true);
        }

        private void OnSubtitlesRequest(SubtitlesRequestInfo info)
        {
            _inputSwitcher.EnableInput(InputBlockMask.Dialogs, true);
        }

        private void OnSkipClicked(InputActionData action)
        {
            _dialogueUGUI.SetAnyKeyDown();
        }

        private void UpdateLanguage()
        {
            var localizationIndex = PlayerPrefs.GetInt("LocalizationIndex", 1);
            _dialogueUGUI.language = (Locales)localizationIndex;
        }

        private void OnDialogueFinished(DialogueTree dialogueTree)
        {
             _inputSwitcher.EnableInput(InputBlockMask.Dialogs, false);
        }

        private void OnDestroy()
        {
            DialogueTree.OnDialogueStarted -= OnDialogueStarted;
            DialogueTree.OnSubtitlesRequest -= OnSubtitlesRequest;
            DialogueTree.OnDialogueFinished -= OnDialogueFinished;
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChange;
        }
    }
}