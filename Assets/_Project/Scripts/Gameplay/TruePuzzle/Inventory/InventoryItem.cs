using _Project.Gameplay.TruePuzzle.Interactable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Gameplay.TruePuzzle.Inventory
{
    public class InventoryItem : MonoBehaviour, IDraggable
    {
        [SerializeField] private Image _image;
        public InteractableType InteractableType { get; private set; }

        private DragDropController _dragDropController;
        public Transform Transform => transform;

        private InventorySlot _currentSlot;

        bool IDraggable.IsMultipleUse =>
            InteractableType is InteractableType.GreenKey or InteractableType.TubeKey;

        public void Setup(InteractableType interactableType, Sprite icon, DragDropController dragDropController, InventorySlot slot)
        {
            InteractableType = interactableType;
            _image.sprite = icon;
            _dragDropController = dragDropController;
            SetNewSlot(slot);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _image.raycastTarget = false;
            _currentSlot.RemoveItem();
            _dragDropController.BeginDrag(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragDropController.UpdateDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _image.raycastTarget = true;
            _dragDropController.EndDrag(eventData);
        }

        public void ReturnToSlot()
        {
            _currentSlot.SetItem(this);
            transform.SetParent(_currentSlot.transform);
            transform.localPosition = Vector3.zero;
        }

        public void SetNewSlot(InventorySlot newSlot)
        {
            _currentSlot = newSlot;
            ReturnToSlot();
        }
    }

    public interface IDraggable : IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        InteractableType InteractableType { get; }
        Transform Transform { get; }
        void ReturnToSlot();
        void SetNewSlot(InventorySlot newSlot);
        bool IsMultipleUse { get; }
    }

    public interface IDropTarget
    {
        void OnDrop(IDraggable draggable);
        bool CanDrop(IDraggable draggable);
    }
}