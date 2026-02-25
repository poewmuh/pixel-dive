using System;
using _Project.Application.Input;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Player
{
    public class TargetClickMover : MonoBehaviour
    {
        [SerializeField] private LayerMask _teleportMask;
        
        [Inject] private IInputSystem _inputSystem;
        [Inject] private Camera _camera;

        private void Start()
        {
            _inputSystem.OnPuzzleMouseClick.Subscribe(TryTeleport).AddTo(this);
        }
        
        private void TryTeleport(InputVectorActionData data)
        {
            if (data.Phase is not InputActionPhase.Performed) return;
            
            var screenPos = data.Vector;
            var ray = _camera.ScreenPointToRay(screenPos);

            var hit = Physics2D.GetRayIntersection(ray, 100, _teleportMask);
            if (hit.collider != null)
            {
                transform.position = new Vector3(hit.point.x, hit.point.y, 0);
            }
        }
    }
}
