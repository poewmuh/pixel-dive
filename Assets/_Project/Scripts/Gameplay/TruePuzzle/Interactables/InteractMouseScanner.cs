using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class InteractMouseScanner : MonoBehaviour
    {
        [Inject] private Camera _camera;
        
        private IInteractable _current;
        private List<RaycastResult> _raycastResults = new();
        private PointerEventData _pointerEventData;

        private void Start()
        {
            _pointerEventData = new PointerEventData(EventSystem.current);
        }

        private void Update()
        {
            var mousePosition = Input.mousePosition;
            var uiObject = GetUIObjectUnderPointer(mousePosition);
            if (uiObject != null)
            {
                if (uiObject.TryGetComponent(out IInteractable interactable))
                {
                    if (interactable != _current)
                    {
                        _current?.OnHoverExit();
                        _current = interactable;
                        _current?.OnHoverEnter();
                    }
                }
                else
                {
                    _current?.OnHoverExit();
                    _current = null;
                }
                
                return;
            }
            
            var ray = _camera.ScreenPointToRay(mousePosition);
            var hit = Physics2D.GetRayIntersection(ray, 100, LayerMask.GetMask("Interactable"));
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    if (interactable != _current)
                    {
                        _current?.OnHoverExit();
                        _current = interactable;
                        _current?.OnHoverEnter();
                    }
                }
            }
            else
            {
                _current?.OnHoverExit();
                _current = null;
            }
        }
        
        private GameObject GetUIObjectUnderPointer(Vector2 screenPos)
        {
            _raycastResults.Clear();
            _pointerEventData.position = screenPos;
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

            return _raycastResults.Count > 0 ? _raycastResults[0].gameObject : null;
        }
    }
}