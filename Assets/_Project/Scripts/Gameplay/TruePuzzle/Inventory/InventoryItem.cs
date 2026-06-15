using _Project.Gameplay.TruePuzzle.Interactable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Gameplay.TruePuzzle.Inventory
{
    public sealed class InventoryItem : MonoBehaviour, IDraggable
    {
        [SerializeField] private Image _image;

        public InteractableType InteractableType { get; private set; }
        public Transform Transform => transform;

        bool IDraggable.IsMultipleUse =>
            InteractableType is InteractableType.GreenKey or InteractableType.TubeKey;

        private DragDropController _dragDropController;
        private InventorySlot _currentSlot;

        public void Setup(
            InteractableType interactableType,
            Sprite icon,
            DragDropController dragDropController,
            InventorySlot slot)
        {
            InteractableType = interactableType;
            _image.sprite = icon;
            _dragDropController = dragDropController;
            MoveToSlot(slot);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _currentSlot.RemoveItem(this);

            if (!_dragDropController.BeginDrag(this, eventData))
            {
                ReturnToSlot();
                return;
            }

            _image.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragDropController.UpdateDrag(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _image.raycastTarget = true;
            _dragDropController.EndDrag(this, eventData);
        }

        public void ReturnToSlot()
        {
            MoveToSlot(_currentSlot);
        }

        public void MoveToSlot(InventorySlot slot)
        {
            _currentSlot = slot;
            _currentSlot.SetItem(this);
            transform.SetParent(_currentSlot.transform);
            transform.localPosition = Vector3.zero;
        }
    }

    public interface IDraggable : IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        InteractableType InteractableType { get; }
        Transform Transform { get; }
        bool IsMultipleUse { get; }
        void ReturnToSlot();
    }

    public interface IDropTarget
    {
        bool TryDrop(IDraggable draggable);
    }
}
