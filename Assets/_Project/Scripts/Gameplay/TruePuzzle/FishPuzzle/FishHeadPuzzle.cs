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
        private bool _timersComplete = false;
        private bool _allPuzzleComplete;

        public void Press(int index)
        {
            fsmOwner.SendEvent("OnPressed");
            if (!_delayTimerStarted && !_fakeTimerStarted && !_timersComplete) _delayTimerStarted = true;
            
            foreach (var headIndex in _puzzleRules[index])
            {
                _heads[headIndex].Toggle(_audioTimer);
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
            _audioTimer += Time.deltaTime;
            if (_audioTimer >= _heads[0].MaxAudioLength)
            {
                _audioTimer = 0;
            }
            if (_allPuzzleComplete) return;
            if (_delayTimerStarted)
            {
                _puzzleFakeTime += Time.deltaTime;
                
                if (_puzzleFakeTime >= _timerStartDelay)
                {
                    _puzzleFakeTime = 0;
                    fsmOwner.SendEvent("OnStartFakeTimer");
                    _delayTimerStarted = false;
                    foreach (var head in _heads)
                    {
                        head.SetOpen(_audioTimer);
                    }
                }
            }

            if (_fakeTimerStarted)
            {
                _puzzleFakeTime += Time.deltaTime;
                _timerText.text = (_timerTime - _puzzleFakeTime).ToString("F1");
                if (_puzzleFakeTime >= _timerTime)
                {
                    fsmOwner.SendEvent("OnFakeTimerEnd");
                    HideFakeTimer();
                    _fakeTimerStarted = false;
                    _timersComplete = true;
                }
            }
        }

        public void StartGameTimer()
        {
            _puzzleFakeTime = 0f;
            _fakeTimerStarted = true;
        }

        public void ShowFakeTimer()
        {
            _timerText.text = _timerTime.ToString("F1");
            _timerGroup.SetActive(true);
        }

        public void HideFakeTimer()
        {
            _timerGroup.SetActive(false);
        }

        public bool IsSolved()
        {
            foreach (var h in _heads)
                if (h.IsOpen)
                    return false;

            return true;
        }
    }
}