using System;
using System.Collections.Generic;
using _Project.Application.Input;
using R3;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using VContainer;

namespace _Project.Gameplay.Platforming.Level
{
    public class SecretInputListener : MonoBehaviour
    {
        [Inject] private IInputSystem _inputSystem;
        [SerializeField] private float _timeBetweenInput = 1;
        [SerializeField] private List<Vector2> _needInput;

        [SerializeField] private UnityEvent _onPuzzleSolve;

        private readonly CompositeDisposable _disposable = new ();
        private int _currentIndex;
        private float _lastTapTime;

        private void Start()
        {
            _inputSystem.OnMove.Subscribe(OnMove).AddTo(_disposable);
        }

        private void OnMove(InputVectorActionData data)
        {
            if (Time.time - _lastTapTime > _timeBetweenInput)
            {
                _currentIndex = 0;
            }
            
            if (data.Phase == InputActionPhase.Performed)
            {
                var currentVector = data.Vector.normalized;
                if (currentVector == _needInput[_currentIndex])
                {
                    _currentIndex++;
                }
            }

            if (_currentIndex >= _needInput.Count)
            {
                _disposable.Dispose();
                _currentIndex = 0;
                _onPuzzleSolve?.Invoke();
            }

            _lastTapTime = Time.time;
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}
