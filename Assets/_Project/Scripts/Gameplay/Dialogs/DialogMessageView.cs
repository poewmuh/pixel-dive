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
        [SerializeField] private float _punctuationDelay = 0.15f;

        private CancellationTokenSource _animationTokenSource;
        private string _currentFullMessage;

        public void Setup(string speakerName, string dialogMessage, Sprite speakerSprite)
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
            TypeText(_animationTokenSource.Token).Forget();
        }

        private async UniTaskVoid TypeText(CancellationToken cancellationToken)
        {
            _dialogMessage.text = "";
            
            foreach (char letter in _currentFullMessage)
            {
                _dialogMessage.text += letter;
                OnCharWrote?.Invoke();
            
                if (_punctuationMarks.Contains(letter.ToString()))
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_punctuationDelay), cancellationToken : cancellationToken);
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_letterDelay), cancellationToken : cancellationToken);
                }
            }

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