using System;
using _Project.Application.Audio;
using _Project.Application.Input;
using _Project.Application.Localization;
using _Project.GUI;
using Cysharp.Threading.Tasks;
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
    public class DialogueOverlay : BaseOverlay
    {
        [Inject] private IInputSwitcher _inputSwitcher;
        [Inject] private IInputSystem _inputSystem;
        [Inject] private IAudioSystem _audioSystem;

        private DialogueUGUI _dialogueUGUI;
        private TMPTextAnimator _textAnimator;
        private SubtitlesRequestInfo _currentSubtitlesInfo;
        private string _currentSubtitleText;
        private bool _isWaitingManualContinue;

        private void Awake()
        {
            _dialogueUGUI = GetComponent<DialogueUGUI>();
            _textAnimator = _dialogueUGUI.actorSpeech.GetComponent<TMPTextAnimator>();
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
            _currentSubtitlesInfo = info;
            _currentSubtitleText = info.statement.GetLocalizedText(_dialogueUGUI.language);
            _isWaitingManualContinue = false;
        }
        
        private void OnSkipClicked(InputActionData action)
        {
            if (action.Phase is not InputActionPhase.Canceled) return;

            if (TryRevealCurrentSubtitle())
            {
                return;
            }
            
            if (_isWaitingManualContinue)
            {
                ContinueCurrentSubtitle();
                return;
            }
            
            _dialogueUGUI.SetAnyKeyDown();
        }

        private void UpdateLanguage()
        {
            var localizationIndex = PlayerPrefs.GetInt("LocalizationIndex", 1);
            _dialogueUGUI.language = (Locales)localizationIndex;
        }

        private void OnDialogueFinished(DialogueTree dialogueTree)
        {
            _currentSubtitlesInfo = null;
            _currentSubtitleText = null;
            _isWaitingManualContinue = false;
            _inputSwitcher.EnableInput(InputBlockMask.Dialogs, false);
        }

        private bool TryRevealCurrentSubtitle()
        {
            if (_currentSubtitlesInfo == null || string.IsNullOrEmpty(_currentSubtitleText))
            {
                return false;
            }
            
            var textFullyAssigned = _dialogueUGUI.actorSpeech.text == _currentSubtitleText;
            var hasActiveTextAnimation = _textAnimator != null && _textAnimator.IsAnimating;
            
            if (textFullyAssigned && !hasActiveTextAnimation)
            {
                return false;
            }
            
            _dialogueUGUI.StopAllCoroutines();
            _textAnimator?.ResetAnimations();
            _dialogueUGUI.actorSpeech.maxVisibleCharacters = int.MaxValue;
            _dialogueUGUI.actorSpeech.text = _currentSubtitleText;
            _dialogueUGUI.actorSpeech.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();
            _dialogueUGUI.subtitlesGroup.gameObject.SetActive(true);
            _isWaitingManualContinue = true;
            
            if (!_dialogueUGUI.waitForInput)
            {
                ContinueCurrentSubtitleAfterDelay().Forget();
            }
            
            return true;
        }

        private async UniTaskVoid ContinueCurrentSubtitleAfterDelay()
        {
            await UniTask.WaitForSeconds(_dialogueUGUI.subtitleDelays.finalDelay);
            if (_isWaitingManualContinue)
            {
                ContinueCurrentSubtitle();
            }
        }

        private void ContinueCurrentSubtitle()
        {
            var subtitlesInfo = _currentSubtitlesInfo;
            _currentSubtitlesInfo = null;
            _currentSubtitleText = null;
            _isWaitingManualContinue = false;
            _dialogueUGUI.subtitlesGroup.gameObject.SetActive(false);
            subtitlesInfo?.Continue();
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
