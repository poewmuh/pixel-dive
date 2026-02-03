using System.Collections.Generic;
using System.Linq;
using _Project.Gameplay.Platforming.Input;
using _Project.Gameplay.Platforming.Interaction;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Playables;

namespace _Project.Gameplay.Platforming.Stage
{
    public class StatuePuzzleDirector : LevelDirector
    {
        [SerializeField] private InputComponent _playerInput;
        [SerializeField] private SerializedDictionary<StatueType, StatueLightAnimator> _statueLights;
        [SerializeField] private List<StatueType> _rightOrder;
        
        [SerializeField] private PlayableDirector _playableDirector;
        [SerializeField] private List<PlayableAsset> _playableAssets;
        
        private readonly List<StatueType> _interactStatues = new ();
        private readonly List<List<StatueType>> _alreadyUseCombinations = new();

        private int interactCount;
        
        public override void StartStage()
        {
            _alreadyUseCombinations.Clear();
            _interactStatues.Clear();
            blackboard.SetVariableValue("IsHaveError", false);
            interactCount = 0;
            SetCurrentNeedStatue(_rightOrder[0]);
            base.StartStage();
        }

        public override void CompleteStage()
        {
            base.CompleteStage();
        }

        public void TogglePlayerInput(bool isEnabled)
        {
            _playerInput.ToggleInput(isEnabled);
        }

        public void InteractWithStatue(int statueId)
        {
            interactCount++;
            var interactStatueType = (StatueType)statueId;
            blackboard.SetVariableValue("InteractStatue", interactStatueType);

            if (_interactStatues.Contains(interactStatueType))
            {
                fsmOwner.SendEvent("OnDoubleClick");
                return;
            }
            
            _interactStatues.Add(interactStatueType);

            if (_alreadyUseCombinations.Any(combination => combination.SequenceEqual(_interactStatues)))
            {
                fsmOwner.SendEvent("OnRepeatCombination");
                return;
            }
            
            if (_interactStatues.SequenceEqual(_rightOrder))
            {
                CompleteStage();
                return;
            }

            if (interactCount >= _rightOrder.Count)
            {
                fsmOwner.SendEvent("OnWrongAnswer");
                blackboard.SetVariableValue("IsHaveError", true);
                _alreadyUseCombinations.Add(new List<StatueType>(_interactStatues));
                return;
            }
            
            fsmOwner.SendEvent("OnInteract");
            
        }

        public void AppearLight(StatueType statueType)
        {
            _statueLights[statueType].AppearAnimation();
        }

        public void SetupNextNeedStatue()
        {
            if (interactCount >= _rightOrder.Count) return;
            
            SetCurrentNeedStatue(_rightOrder[interactCount]);
        }

        private void SetCurrentNeedStatue(StatueType statueType)
        {
            blackboard.SetVariableValue("CurrentNeed", statueType);
        }

        public void ResetAllStatues()
        {
            interactCount = 0;
            SetCurrentNeedStatue(_rightOrder[0]);
            _interactStatues.Clear();
            
            foreach (var statueLight in _statueLights.Values)
            {
                statueLight.DisappearAnimation();
            }
        }

        public void PlayCutscene(int index)
        {
            _playableDirector.time = 0;
            _playableDirector.Play(_playableAssets[index]);
        }

        public void StopCutscene()
        {
            _playableDirector.time = _playableDirector.duration;
        }
    }
}