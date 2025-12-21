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

        public UnityEvent OnPuzzleSolve;

        private int _currentIndex;
        private float _lastTapTime;

        private void Start()
        {
            _inputSystem.OnMove.Subscribe(OnMove).AddTo(this);
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
                OnPuzzleSolve?.Invoke();
            }

            _lastTapTime = Time.time;
        }
    }
}
