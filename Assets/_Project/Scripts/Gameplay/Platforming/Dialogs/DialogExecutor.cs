using System;
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
        
        [Inject] private IDialogeSystem _dialogeSystem;
        
        private RuntimeVariableStorage _variables;

        private void Awake()
        {
            _variables = new RuntimeVariableStorage();
            _variables.SetVariable("interactCount", 0);
        }

        public void ExecuteDialog()
        {
            _dialogeSystem.DialogeStart(_dialogNode, () => _onDialogEnded.Invoke(), _variables);
            _variables.IncrementInt("interactCount");
        }

        public void EnablePuzzleMode()
        {
            _variables.SetVariable("puzzleMode", 1);
        }
        
        public void DisablePuzzleMode()
        {
            _variables.SetVariable("puzzleMode", 0);
        }
    }
}