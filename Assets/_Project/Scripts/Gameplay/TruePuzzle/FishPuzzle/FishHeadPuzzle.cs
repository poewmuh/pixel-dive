using System;
using _Project.Gameplay.Platforming.Stage;
using NodeCanvas.Tasks.Actions;
using TMPro;
using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.FishPuzzle
{
    public class FishHeadPuzzle : LevelDirector
    {
        [SerializeField] private float _timerStartDelay = 5f;
        [SerializeField] private float _timerTime = 30f;
        [SerializeField] private FishHead[] _heads;
        [SerializeField] private TextMeshPro _timerText;
        [SerializeField] private GameObject _timerGroup;
        
        private readonly int[][] _puzzleRules =
        {
            new []{0, 1, 2},
            new []{1, 0, 3},
            new []{2, 0, 3},
            new []{3, 1, 2}
        };

        private float _audioTimer;
        private float _puzzleFakeTime;
        private bool _fakeTimerStarted;
        private bool _delayTimerStarted;
        private bool _timersComplete;
        private bool _allPuzzleComplete;
        private int _lastDisplayedTimerTenths = int.MinValue;

        public void Press(int index)
        {
            if (index < 0 || index >= _puzzleRules.Length)
            {
                Debug.LogWarning($"{nameof(FishHeadPuzzle)} received invalid press index {index}.", this);
                return;
            }
            
            fsmOwner.SendEvent("OnPressed");
            if (!_delayTimerStarted && !_fakeTimerStarted && !_timersComplete) _delayTimerStarted = true;
            
            foreach (var headIndex in _puzzleRules[index])
            {
                if (TryGetHead(headIndex, out var head))
                {
                    head.Toggle(_audioTimer);
                }
            }

            if (IsSolved() && !_allPuzzleComplete)
            {
                CompleteStage();
            }
        }

        public override void CompleteStage()
        {
            base.CompleteStage();
            HideFakeTimer();
            _delayTimerStarted = false;
            _fakeTimerStarted = false;
            _allPuzzleComplete = true;
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            UpdateAudioTimer(deltaTime);
            
            if (_allPuzzleComplete) return;
            
            if (_delayTimerStarted)
            {
                UpdateStartDelayTimer(deltaTime);
            }

            if (_fakeTimerStarted)
            {
                UpdateFakeTimer(deltaTime);
            }
        }

        private void UpdateAudioTimer(float deltaTime)
        {
            if (_heads == null || _heads.Length == 0 || _heads[0] == null) return;
            
            _audioTimer += deltaTime;
            if (_audioTimer >= _heads[0].MaxAudioLength)
            {
                _audioTimer = 0f;
            }
        }

        private void UpdateStartDelayTimer(float deltaTime)
        {
            _puzzleFakeTime += deltaTime;
            
            if (_puzzleFakeTime < _timerStartDelay) return;
            
            _puzzleFakeTime = 0f;
            fsmOwner.SendEvent("OnStartFakeTimer");
            _delayTimerStarted = false;
            
            foreach (var head in _heads)
            {
                if (head != null)
                {
                    head.SetOpen(_audioTimer);
                }
            }
        }

        private void UpdateFakeTimer(float deltaTime)
        {
            _puzzleFakeTime += deltaTime;
            UpdateTimerText(_timerTime - _puzzleFakeTime);
            
            if (_puzzleFakeTime < _timerTime) return;
            
            fsmOwner.SendEvent("OnFakeTimerEnd");
            HideFakeTimer();
            _fakeTimerStarted = false;
            _timersComplete = true;
        }

        public void StartGameTimer()
        {
            _puzzleFakeTime = 0f;
            _lastDisplayedTimerTenths = int.MinValue;
            _fakeTimerStarted = true;
        }

        public void ShowFakeTimer()
        {
            UpdateTimerText(_timerTime);
            _timerGroup.SetActive(true);
        }

        public void HideFakeTimer()
        {
            _timerGroup.SetActive(false);
        }

        public bool IsSolved()
        {
            foreach (var h in _heads)
                if (h != null && h.IsOpen)
                    return false;

            return true;
        }

        private void UpdateTimerText(float remainingTime)
        {
            if (_timerText == null) return;
            
            var tenths = Mathf.Max(0, Mathf.RoundToInt(remainingTime * 10f));
            if (_lastDisplayedTimerTenths == tenths) return;
            
            _lastDisplayedTimerTenths = tenths;
            _timerText.text = (tenths / 10f).ToString("F1");
        }

        private bool TryGetHead(int index, out FishHead head)
        {
            head = null;
            if (_heads == null || index < 0 || index >= _heads.Length) return false;
            
            head = _heads[index];
            return head != null;
        }
    }
}
