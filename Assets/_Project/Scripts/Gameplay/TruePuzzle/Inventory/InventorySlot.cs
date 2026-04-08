using System;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Inventory
{
    public class InventorySlot : MonoBehaviour, IDropTarget
    {
        [Inject] private ISubscriber<DragEventMessage> _dragEventSubscriber;
        public bool IsFree => Item is null;
        public InventoryItem Item { get; private set; }

        private Image _raycastImage;

        private void Awake()
        {
            TryGetComponent(out _raycastImage);
            _raycastImage.enabled = false;
            _dragEventSubscriber.Subscribe(OnDragChanged).AddTo(this);
        }

        public void SetItem(InventoryItem item)
        {
            Item = item;
        }
        
        public void RemoveItem()
        {
            Item = null;
        }

        private void OnDragChanged(DragEventMessage msg)
        {
            if (IsFree)
            {
                _raycastImage.enabled = msg.IsStarted;
            }
        }

        public void OnDrop(IDraggable draggable)
        {
            SetItem(draggable as InventoryItem);
        }

        public bool CanDrop(IDraggable draggable)
        {
            return IsFree && draggable is InventoryItem;
        }
    }
}