using System.Collections.Generic;
using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Inventory
{
    public class DragDropController : MonoBehaviour
    {
        [Inject] private IPublisher<DragEventMessage> _dragEventPublisher;
        [Inject] private IPublisher<DragFailedMessage> _dragFailedPublisher;
        [Inject] private Camera _camera;
        
        private IDraggable _currentDraggable;
        private Vector2 _dragOffset;
        
        public void BeginDrag(IDraggable draggable, PointerEventData eventData)
        {
            _currentDraggable = draggable;
            _dragOffset = (Vector2)_currentDraggable.Transform.position - eventData.position;
            _dragEventPublisher.Publish(new DragEventMessage(_currentDraggable, true));
        }

        public void UpdateDrag(PointerEventData eventData)
        {
            if (_currentDraggable == null) return;
            _currentDraggable.Transform.position = eventData.position + _dragOffset;
        }

        public void EndDrag(PointerEventData eventData)
        {
            if (_currentDraggable == null) return;

            var target = FindDropTarget(eventData);

            if (target != null && target.CanDrop(_currentDraggable))
            {
                target.OnDrop(_currentDraggable);
                if (target is InventorySlot slot)
                {
                    _currentDraggable.SetNewSlot(slot);
                }
            }
            else
            {
                var reason = DragFailedReason.MissingTarget;
                if (target != null)
                {
                    reason = DragFailedReason.InvalidTarget;
                }
                _currentDraggable.ReturnToSlot();
                
                _dragFailedPublisher.Publish(new DragFailedMessage(reason));
            }

            _dragEventPublisher.Publish(new DragEventMessage(_currentDraggable, false));
            _currentDraggable = null;
        }
        
        private IDropTarget FindDropTarget(PointerEventData eventData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject == _currentDraggable.Transform.gameObject) continue;
                if (result.gameObject.TryGetComponent<IDropTarget>(out var target))
                    return target;
            }

            var ray = _camera.ScreenPointToRay(eventData.position);
            var hit = Physics2D.GetRayIntersection(ray, 100, LayerMask.GetMask("Interactable"));
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out IDropTarget target))
                {
                    return target;
                }
            }
            
            return null;
        }
    }

    public class DragEventMessage
    {
        public DragEventMessage(IDraggable draggable, bool isStarted)
        {
            Draggable = draggable;
            IsStarted = isStarted;
        }
        
        public IDraggable Draggable { get; }
        public bool IsStarted { get; }
    }
    
    public class DragFailedMessage
    {
        public DragFailedMessage(DragFailedReason reason)
        {
            Reason = reason;
        }
        
        public DragFailedReason Reason { get; }
    }
    
    public enum DragFailedReason
    {
        MissingTarget,
        InvalidTarget,
    }
}