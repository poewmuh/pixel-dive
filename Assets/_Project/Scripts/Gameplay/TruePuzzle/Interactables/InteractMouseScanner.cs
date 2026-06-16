using System.Collections.Generic;
using _Project.Application.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public sealed class InteractMouseScanner : MonoBehaviour
    {
        [Inject] private Camera _camera;
        [Inject] private IInputSwitcher _inputSwitcher;

        private readonly List<RaycastResult> _raycastResults = new();

        private IInteractable _currentInteractable;
        private PointerEventData _pointerEventData;
        private int _interactableMask;

        private void Start()
        {
            _pointerEventData = new PointerEventData(EventSystem.current);
            _interactableMask = LayerMask.GetMask("Interactable");
        }

        private void Update()
        {
            if (!CanScan())
            {
                SetCurrentInteractable(null);
                return;
            }

            SetCurrentInteractable(FindInteractable(Input.mousePosition));
        }

        private bool CanScan()
        {
            return _inputSwitcher.Blockers.Value is InputBlockMask.None &&
                   _inputSwitcher.InputMap.Value is InputMapMode.Puzzle;
        }

        private IInteractable FindInteractable(Vector2 screenPosition)
        {
            var uiObject = GetUIObjectUnderPointer(screenPosition);
            if (uiObject)
            {
                uiObject.TryGetComponent(out IInteractable uiInteractable);
                return uiInteractable;
            }

            var ray = _camera.ScreenPointToRay(screenPosition);
            var hit = Physics2D.GetRayIntersection(ray, 100f, _interactableMask);
            if (!hit.collider) return null;

            hit.collider.TryGetComponent(out IInteractable worldInteractable);
            return worldInteractable;
        }

        private void SetCurrentInteractable(IInteractable interactable)
        {
            if (ReferenceEquals(_currentInteractable, interactable)) return;

            _currentInteractable?.OnHoverExit();
            _currentInteractable = interactable;
            _currentInteractable?.OnHoverEnter();
        }

        private GameObject GetUIObjectUnderPointer(Vector2 screenPosition)
        {
            _raycastResults.Clear();
            _pointerEventData.position = screenPosition;
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

            return _raycastResults.Count > 0 ? _raycastResults[0].gameObject : null;
        }

        private void OnDisable()
        {
            SetCurrentInteractable(null);
        }
    }
}
