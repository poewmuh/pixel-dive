using _Project.Gameplay.TruePuzzle.Inventory;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class DropFSMTarget : MonoBehaviour, IDropTarget
    {
        private const string ItemActivateEventName = "ItemActivate";
        private const string ItemIncorrectEventName = "ItemIncorrect";
        
        private FSMOwner _fsmOwner;
        private IInteractable _interactable;
        private bool _isActive;
        
        private void Awake()
        {
            TryGetComponent(out _fsmOwner);
            TryGetComponent(out _interactable);
            _isActive = true;
        }

        public bool TryDrop(IDraggable draggable)
        {
            if (!_isActive || draggable == null || _interactable == null) return false;
            
            switch (draggable.InteractableType)
            {
                case InteractableType.GreenKey when _interactable.InteractableType is InteractableType.GreenDoor:
                case InteractableType.TubeKey when _interactable.InteractableType is InteractableType.RedDoor:
                case InteractableType.DiamondKey when _interactable.InteractableType is InteractableType.MiddleDoor:
                    ApplyDrop(draggable);
                    return true;
                default:
                    _fsmOwner?.SendEvent(ItemIncorrectEventName);
                    return false;
            }
        }

        private void ApplyDrop(IDraggable draggable)
        {
            _fsmOwner?.SendEvent(ItemActivateEventName);
            _interactable.OnClick();
            if (!draggable.IsMultipleUse)
            {
                Destroy(draggable.Transform.gameObject);
            }
            else
            {
                draggable.ReturnToSlot();
            }

            _isActive = false;
        }
    }
}
