using System.Collections.Generic;
using _Project.Gameplay.TruePuzzle;
using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace _Project.Gameplay.TruePuzzle
{
    public sealed class PointerHitTester
    {
        private const float RayDistance = 100f;

        private readonly Camera _camera;
        private readonly List<RaycastResult> _raycastResults;

        public PointerHitTester(Camera camera, List<RaycastResult> raycastResults)
        {
            _camera = camera;
            _raycastResults = raycastResults;
        }

        public bool TryGetUIComponent<T>(
            PointerEventData pointerEventData,
            out T component,
            GameObject ignoredObject = null)
        {
            component = default;
            var eventSystem = EventSystem.current;
            if (eventSystem == null) return false;

            _raycastResults.Clear();
            eventSystem.RaycastAll(pointerEventData, _raycastResults);

            foreach (var result in _raycastResults)
            {
                var gameObject = result.gameObject;
                if (gameObject == null || gameObject == ignoredObject) continue;
                
                if (gameObject.TryGetComponent(out component))
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryGetWorldComponent<T>(
            Vector2 screenPosition,
            int layerMask,
            out T component)
        {
            component = default;
            if (_camera == null) return false;

            var ray = _camera.ScreenPointToRay(screenPosition);
            var hit = Physics2D.GetRayIntersection(ray, RayDistance, layerMask);
            if (!hit.collider) return false;

            return hit.collider.TryGetComponent(out component);
        }
    }
}

namespace _Project.Gameplay.TruePuzzle.Inventory
{
    public sealed class DragDropController : MonoBehaviour
    {
        [Inject] private IPublisher<DragEventMessage> _dragEventPublisher;
        [Inject] private IPublisher<DragFailedMessage> _dragFailedPublisher;
        [Inject] private Camera _camera;

        private readonly List<RaycastResult> _raycastResults = new();

        private IDraggable _currentDraggable;
        private PointerHitTester _hitTester;
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
            _hitTester ??= new PointerHitTester(_camera, _raycastResults);
            
            if (_hitTester.TryGetUIComponent(
                    eventData,
                    out IDropTarget uiTarget,
                    _currentDraggable.Transform.gameObject))
            {
                return uiTarget;
            }

            return _hitTester.TryGetWorldComponent(
                eventData.position,
                _interactableMask,
                out IDropTarget worldTarget)
                ? worldTarget
                : null;
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
