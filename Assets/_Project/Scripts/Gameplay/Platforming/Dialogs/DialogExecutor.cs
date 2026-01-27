using _Project.Application.GUI.DialogeSystem;
using _Project.Gameplay.Dialogs.Core;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace _Project.Gameplay.Platforming.Dialogs
{
    public class DialogExecutor : MonoBehaviour
    {
        [SerializeField] private RuntimeDialogGraph _dialogNode;
        [SerializeField] private UnityEvent _onDialogEnded;
        [SerializeField] private float _delay = 0;
        
        [Inject] private IDialogeSystem _dialogeSystem;
        
        private RuntimeVariableStorage _variables;

        private void Awake()
        {
            _variables = new RuntimeVariableStorage();
            _variables.SetVariable("interactCount", -1);
        }

        public void ExecuteDialog()
        {
            _variables.IncrementInt("interactCount");
            _dialogeSystem.DelayDialogStart(_dialogNode, () => _onDialogEnded.Invoke(), _delay, _variables);
        }

        public void EnablePuzzleMode()
        {
            _variables.SetVariable("puzzleMode", 1);
        }
        
        public void DisablePuzzleMode()
        {
            _variables.SetVariable("puzzleMode", 0);
        }

        public void SetPuzzleNeedId(int order)
        {
            _variables.SetVariable("puzzleNeedId", order);
        }
    }
}