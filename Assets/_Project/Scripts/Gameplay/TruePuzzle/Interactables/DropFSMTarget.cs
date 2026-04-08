using _Project.Gameplay.TruePuzzle.Inventory;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class DropFSMTarget : MonoBehaviour, IDropTarget
    {
        private FSMOwner _fsmOwner;
        private Blackboard _blackboard;
        private IInteractable _interactable;
        private bool _isActive;
        
        private void Awake()
        {
            TryGetComponent(out _fsmOwner);
            TryGetComponent(out _interactable);
            TryGetComponent(out _blackboard);
            _isActive = true;
        }

        public bool CanDrop(IDraggable draggable)
        {
            if (!_isActive) return false;
            
            if (draggable.InteractableType is InteractableType.GreenKey)
            {
                if (_interactable.InteractableType is InteractableType.GreenDoor)
                {
                    return true;
                }
            }

            _fsmOwner.SendEvent("ItemIncorrect");
            return false;
        }

        public void OnDrop(IDraggable draggable)
        {
            _fsmOwner.SendEvent("ItemActivate");
            _interactable.OnClick();
            Destroy(draggable.Transform.gameObject);
            _isActive = false;
        }
    }
}