using System;
using _Project.Application.GUI.DialogeSystem;
using _Project.Gameplay.Dialogs.Core;
using R3;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace _Project.Gameplay.Platforming.Dialogs
{
    public class TimeDialogTrigger : MonoBehaviour
    {
        [SerializeField] private RuntimeDialogGraph _dialogNode;
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
            _dialogeSystem.DialogeStart(_dialogNode, OnDialogEnded);
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