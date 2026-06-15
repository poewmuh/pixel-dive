using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Inventory
{
    public sealed class InventorySlot : MonoBehaviour, IDropTarget
    {
        [Inject] private ISubscriber<DragEventMessage> _dragEventSubscriber;

        public bool IsFree => Item == null;
        public InventoryItem Item { get; private set; }

        private Image _raycastImage;

        private void Awake()
        {
            _raycastImage = GetComponent<Image>();
            _raycastImage.enabled = false;
            _dragEventSubscriber.Subscribe(OnDragChanged).AddTo(this);
        }

        public void SetItem(InventoryItem item)
        {
            Item = item;
        }

        public void RemoveItem(InventoryItem item)
        {
            if (Item == item)
            {
                Item = null;
            }
        }

        public void RemoveItem()
        {
            Item = null;
        }

        private void OnDragChanged(DragEventMessage message)
        {
            _raycastImage.enabled = message.IsStarted && IsFree;
        }

        bool IDropTarget.TryDrop(IDraggable draggable)
        {
            if (!IsFree || draggable is not InventoryItem item) return false;

            item.MoveToSlot(this);
            return true;
        }
    }
}
