using System.Collections.Generic;
using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Inventory
{
    public sealed class DragDropController : MonoBehaviour
    {
        [Inject] private IPublisher<DragEventMessage> _dragEventPublisher;
        [Inject] private IPublisher<DragFailedMessage> _dragFailedPublisher;
        [Inject] private Camera _camera;

        private readonly List<RaycastResult> _raycastResults = new();

        private IDraggable _currentDraggable;
        private Vector2 _dragOffset;
        private int _interactableMask;

        private void Awake()
        {
            _interactableMask = LayerMask.GetMask("Interactable");
        }

        public bool BeginDrag(IDraggable draggable, PointerEventData eventData)
        {
            if (_currentDraggable != null) return false;

            _currentDraggable = draggable;
            _dragOffset = (Vector2)draggable.Transform.position - eventData.position;
            _dragEventPublisher.Publish(new DragEventMessage(draggable, true));
            return true;
        }

        public void UpdateDrag(IDraggable draggable, PointerEventData eventData)
        {
            if (!ReferenceEquals(_currentDraggable, draggable)) return;

            draggable.Transform.position = eventData.position + _dragOffset;
        }

        public void EndDrag(IDraggable draggable, PointerEventData eventData)
        {
            if (!ReferenceEquals(_currentDraggable, draggable)) return;

            var target = FindDropTarget(eventData);
            var wasAccepted = target != null && target.TryDrop(draggable);

            if (!wasAccepted)
            {
                draggable.ReturnToSlot();
                var reason = target == null
                    ? DragFailedReason.MissingTarget
                    : DragFailedReason.InvalidTarget;
                _dragFailedPublisher.Publish(new DragFailedMessage(reason));
            }

            _dragEventPublisher.Publish(new DragEventMessage(draggable, false));
            _currentDraggable = null;
        }

        private IDropTarget FindDropTarget(PointerEventData eventData)
        {
            _raycastResults.Clear();
            EventSystem.current.RaycastAll(eventData, _raycastResults);

            foreach (var result in _raycastResults)
            {
                if (result.gameObject == _currentDraggable.Transform.gameObject) continue;
                if (result.gameObject.TryGetComponent(out IDropTarget target))
                {
                    return target;
                }
            }

            var ray = _camera.ScreenPointToRay(eventData.position);
            var hit = Physics2D.GetRayIntersection(ray, 100f, _interactableMask);
            if (hit.collider && hit.collider.TryGetComponent(out IDropTarget worldTarget))
            {
                return worldTarget;
            }

            return null;
        }
    }

    public sealed class DragEventMessage
    {
        public IDraggable Draggable { get; }
        public bool IsStarted { get; }

        public DragEventMessage(IDraggable draggable, bool isStarted)
        {
            Draggable = draggable;
            IsStarted = isStarted;
        }
    }

    public sealed class DragFailedMessage
    {
        public DragFailedReason Reason { get; }

        public DragFailedMessage(DragFailedReason reason)
        {
            Reason = reason;
        }
    }

    public enum DragFailedReason
    {
        MissingTarget,
        InvalidTarget
    }
}
