using System.Collections.Generic;
using System.Linq;
using _Project.Gameplay.Platforming.Dialogs;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Gameplay.Platforming.Stage
{
    public class StatuePuzzleDirector : LevelDirector
    {
        [SerializeField] private DialogExecutor _resetStatueDoubleClick;
        [SerializeField] private DialogExecutor _resetStatueExist;
        [SerializeField] private DialogExecutor _perfectComplete;
        [SerializeField] private DialogExecutor _errorComplete;
        [SerializeField] private List<DialogExecutor> _dialogExecutors;
        [SerializeField] private List<int> _rightOrder;

        [SerializeField] private UnityEvent[] _errorEvent;

        private int _currentPuzzleStage;
        private int _rightAnswersCount;
        private int _resetCount;

        private List<int> _currentPlayerCombination = new(4);
        private List<List<int>> _allPlayersCombinations = new(4);
        
        public override void StartStage()
        {
            _currentPlayerCombination.Clear();
            _allPlayersCombinations.Clear();

            _resetCount = 0;
            _rightAnswersCount = 0;
            _currentPuzzleStage = 0;
            base.StartStage();
            foreach (var dialog in _dialogExecutors)
            {
                dialog.EnablePuzzleMode();
                dialog.SetPuzzleNeedId(_rightOrder[_currentPuzzleStage]);
            }
        }

        public override void CompleteStage()
        {
            base.CompleteStage();
            _currentPlayerCombination.Clear();
            _allPlayersCombinations.Clear();
            
            foreach (var dialog in _dialogExecutors)
            {
                dialog.DisablePuzzleMode();
            }
        }

        public void InteractWithStatue(int statudeId)
        {
            if (!IsActive) return;
            if (_currentPlayerCombination.Contains(statudeId))
            {
                _resetStatueDoubleClick.ExecuteDialog();
                ResetPuzzle();
                return;
            }
            _currentPlayerCombination.Add(statudeId);
            
            foreach (var dialog in _dialogExecutors)
            {
                dialog.SetPuzzleNeedId(_rightOrder[_currentPuzzleStage]);
            }

            _currentPuzzleStage++;

            if (_currentPlayerCombination.Count == 4)
            {
                if (_allPlayersCombinations.Any(combination => combination.SequenceEqual(_currentPlayerCombination)))
                {
                    _resetStatueExist.ExecuteDialog();
                    ResetPuzzle();
                    return;
                }
                
                _allPlayersCombinations.Add(_currentPlayerCombination);
                if (_currentPlayerCombination.SequenceEqual(_rightOrder))
                {
                    if (_allPlayersCombinations.Count == 1 && _resetCount <= 0)
                    {
                        _perfectComplete.ExecuteDialog();
                    }
                    else
                    {
                        _errorComplete.ExecuteDialog();
                    }
                    
                    CompleteStage();
                }
                else
                {
                    _errorEvent[_allPlayersCombinations.Count-1]?.Invoke();
                    ResetPuzzle();
                }
            }
        }

        private void ResetPuzzle()
        {
            _resetCount++;
            _currentPlayerCombination.Clear();
            _currentPuzzleStage = 0;
        }

        public void OnInteractDialogEnd()
        {
        }
    }
}