using System;
using System.Collections.Generic;
using System.Threading;
using _Project.Application.Audio;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace _Project.Gameplay.Dialogs
{
    public class CustomDialogShower : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private TMPTextAnimator _textAnimator;
        
        [Header("Timing")]
        [SerializeField] private float _characterDelay = 0.05f;
        [SerializeField] private float _punctuationDelay = 0.2f;

        [Header("Typing Sounds")]
        [SerializeField] private List<AudioClip> _typingSounds;
        [SerializeField] private float _minVolume = 0.6f;
        [SerializeField] private float _maxVolume = 1f;

        private CancellationTokenSource _cts;
        private AudioSource _audioSource;

        [Inject] private IAudioSystem _audioSystem;

        private void Awake()
        {
            _audioSource = _audioSystem.GetSFXSource();
        }

        private void OnEnable()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            StartShowingDialog(_cts.Token).Forget();
        }

        private void OnDisable()
        {
            _cts?.Cancel();
        }

        private async UniTaskVoid StartShowingDialog(CancellationToken ct)
        {
            try
            {
                _message.ForceMeshUpdate();
                var totalCharacters = _message.textInfo.characterCount;
                
                _message.maxVisibleCharacters = 0;

                for (var i = 0; i < totalCharacters; i++)
                {
                    if (ct.IsCancellationRequested) return;
                    
                    _message.maxVisibleCharacters = i + 1;
                    _textAnimator?.AnimateCharacter(i);

                    var c = _message.textInfo.characterInfo[i].character;
                    
                    if (!char.IsWhiteSpace(c))
                    {
                        PlayTypingSound();
                    }

                    var delay = IsPunctuation(c) ? _punctuationDelay : _characterDelay;
                    
                    await UniTask.Delay((int)(delay * 1000), cancellationToken: ct);
                }
            }
            catch (OperationCanceledException)
            {
                // Cancellation is the expected path when the player skips the typewriter effect.
            }
        }

        private void PlayTypingSound()
        {
            if (_audioSource == null || _typingSounds == null || _typingSounds.Count == 0) return;
            
            var clip = _typingSounds[Random.Range(0, _typingSounds.Count)];
            if (clip != null)
            {
                _audioSource.PlayOneShot(clip, Random.Range(_minVolume, _maxVolume));
            }
        }

        private bool IsPunctuation(char c)
        {
            return c is '.' or '!' or '?' or ',' or ';' or ':';
        }

        public void ShowAll()
        {
            _cts?.Cancel();
            _message.maxVisibleCharacters = int.MaxValue;
            _textAnimator?.ResetAnimations();
            _message.ForceMeshUpdate();
        }
    }
}
