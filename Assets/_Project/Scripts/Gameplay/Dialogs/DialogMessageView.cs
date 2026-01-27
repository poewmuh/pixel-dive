using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Gameplay.Dialogs
{
    public class DialogMessageView : MonoBehaviour
    {
        private const string _punctuationMarks = ".,!?;";
        
        public event Action OnCharWrote;
        
        public bool IsTyping => _animationTokenSource is { IsCancellationRequested: false };
        
        [SerializeField] private TextMeshProUGUI _speakerName;
        [SerializeField] private TextMeshProUGUI _dialogMessage;
        [SerializeField] private Image _speakerIcon;
        
        [SerializeField] private float _letterDelay = 0.05f;

        private CancellationTokenSource _animationTokenSource;
        private string _currentFullMessage;

        public void Setup(string speakerName, string dialogMessage, Sprite speakerSprite, float letterDelay = -1)
        {
            _currentFullMessage = dialogMessage;
            _speakerIcon.gameObject.SetActive(speakerSprite != null);
            _speakerName.text = speakerName;
            _speakerIcon.sprite = speakerSprite;

            if (_animationTokenSource != null)
            {
                _animationTokenSource.Cancel();
            }
            _animationTokenSource = new CancellationTokenSource();
            var delay = letterDelay > 0 ? letterDelay : _letterDelay;
            TypeText(delay, _animationTokenSource.Token).Forget();
        }

        private async UniTaskVoid TypeText(float letterDelay, CancellationToken cancellationToken)
        {
            var startTime = Time.time;
            Debug.Log($"letterDelay: {letterDelay} currentTime = {startTime}");
            _dialogMessage.text = "";
            
            foreach (char letter in _currentFullMessage)
            {
                _dialogMessage.text += letter;
                OnCharWrote?.Invoke();
            
                if (_punctuationMarks.Contains(letter.ToString()))
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(letterDelay * 10), cancellationToken : cancellationToken);
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(letterDelay), cancellationToken : cancellationToken);
                }
            }

            Debug.Log($"currentTime = {Time.time} delta: {Time.time - startTime}");
            _animationTokenSource = null;
        }
        
        public void SkipTypewriter()
        {
            if (IsTyping)
            {
                _dialogMessage.text = _currentFullMessage;
                _animationTokenSource.Cancel();
                _animationTokenSource = null;
            }
        }
    }
}