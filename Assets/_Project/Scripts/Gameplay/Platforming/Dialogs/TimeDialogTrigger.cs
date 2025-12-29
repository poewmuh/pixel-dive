using System;
using _Project.Application.GUI.DialogeSystem;
using cherrydev;
using R3;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using Random = UnityEngine.Random;

namespace _Project.Gameplay.Platforming.Dialogs
{
    public class TimeDialogTrigger : MonoBehaviour
    {
        [SerializeField] private DialogNodeGraph[] _dialogNodes;
        [SerializeField] private float _delayTime = 10f;
        [SerializeField] private bool _disableOnExecute = true;
        [SerializeField] private bool _startTimerOnAwake = true;
        [SerializeField] private UnityEvent _onDialogEnded;
        
        [Inject] private IDialogeSystem _dialogeSystem;

        private void Awake()
        {
            if (_startTimerOnAwake)
            {
                StartTimer();
            }
        }

        public void StartTimer()
        {
            Observable.Interval(TimeSpan.FromSeconds(_delayTime)).Take(1).Subscribe(_ => OnTimeEnded()).AddTo(this);
        }

        private void OnTimeEnded()
        {
            var dialog = _dialogNodes[Random.Range(0, _dialogNodes.Length)];
            _dialogeSystem.DialogeStart(dialog, OnDialogEnded);
        }

        private void OnDialogEnded()
        {
            _onDialogEnded?.Invoke();
            if (_disableOnExecute)
            {
                gameObject.SetActive(false);
            }
            else
            {
                StartTimer();
            }
        }
    }
}