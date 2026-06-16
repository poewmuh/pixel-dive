using System.Collections.Generic;
using _Project.Application.Input;
using _Project.Gameplay.TruePuzzle;
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
        private PointerHitTester _hitTester;
        private int _interactableMask;

        private void Start()
        {
            _pointerEventData = new PointerEventData(EventSystem.current);
            _hitTester = new PointerHitTester(_camera, _raycastResults);
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
            _pointerEventData.position = screenPosition;
            if (_hitTester.TryGetUIComponent(_pointerEventData, out IInteractable uiInteractable))
            {
                return uiInteractable;
            }

            return _hitTester.TryGetWorldComponent(
                screenPosition,
                _interactableMask,
                out IInteractable worldInteractable)
                ? worldInteractable
                : null;
        }

        private void SetCurrentInteractable(IInteractable interactable)
        {
            if (ReferenceEquals(_currentInteractable, interactable)) return;

            _currentInteractable?.OnHoverExit();
            _currentInteractable = interactable;
            _currentInteractable?.OnHoverEnter();
        }

        private void OnDisable()
        {
            SetCurrentInteractable(null);
        }
    }
}
