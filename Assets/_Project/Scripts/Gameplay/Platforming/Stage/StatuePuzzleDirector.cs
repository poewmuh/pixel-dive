using System.Collections.Generic;
using _Project.Gameplay.Dialogs;
using _Project.Gameplay.Platforming.Dialogs;
using UnityEngine;

namespace _Project.Gameplay.Platforming.Stage
{
    public class StatuePuzzleDirector : LevelDirector
    {
        [SerializeField] private DialogExecutor _fullComplete;
        [SerializeField] private DialogExecutor _almostComplete;
        [SerializeField] private List<DialogExecutor> _dialogExecutors;
        [SerializeField] private List<int> _rightOrder;

        private int _currentPuzzleStage;
        private int _rightAnswersCount;
        
        public override void StartStage()
        {
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
            
            foreach (var dialog in _dialogExecutors)
            {
                dialog.DisablePuzzleMode();
            }
        }

        public void InteractWithStatue(int statudeId)
        {
            if (!IsActive) return;
            
            if (statudeId == _rightOrder[_currentPuzzleStage])
            {
                _rightAnswersCount++;
            }
            
            _currentPuzzleStage++;
            if (_currentPuzzleStage < _rightOrder.Count)
            {
                foreach (var dialog in _dialogExecutors)
                {
                    dialog.SetPuzzleNeedId(_rightOrder[_currentPuzzleStage]);
                }
            }
        }

        public void OnInteractDialogEnd()
        {
            if (!IsActive) return;
            
            if (_currentPuzzleStage >= _rightOrder.Count)
            {
                if (_rightAnswersCount == 0)
                {
                    BrokeStage();
                }
                else
                {
                    CompleteStage();
                    if (_rightAnswersCount >= _rightOrder.Count)
                    {
                        _fullComplete.ExecuteDialog();
                    }
                    else
                    {
                        _almostComplete.ExecuteDialog();
                    }
                }
            }
        }
    }
}