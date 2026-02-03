using NodeCanvas.DialogueTrees;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace _Project.Gameplay.Platforming.Dialogs
{
    [RequireComponent(typeof(DialogueTreeController))]
    public class DialogExecutor : MonoBehaviour
    {
        private DialogueTreeController _dialogTree;
        
        [SerializeField] private UnityEvent _onDialogEnded;

        private void Awake()
        {
            _dialogTree = GetComponent<DialogueTreeController>();
        }

        public void ExecuteDialog()
        {
            _dialogTree.StartDialogue(_=> _onDialogEnded?.Invoke());
        }
    }
}