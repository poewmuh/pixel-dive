using UnityEngine;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class InteractMouseScanner : MonoBehaviour
    {
        [Inject] private Camera _camera;
        
        private IInteractable _current;
        
        private void Update()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.GetRayIntersection(ray, 100, LayerMask.GetMask("Interactable"));
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
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
    }
}