using System;
using _Project.Application.Input;
using _Project.Gameplay.TruePuzzle.Interactable;
using MessagePipe;
using Pathfinding;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Player
{
    public class AStarMoverComponent : MonoBehaviour, IComponent
    {
        private const float DOUBLE_CLICK_TIME = .2f;
        
        [SerializeField] private Transform _target;
        [SerializeField] private LayerMask _moveMask;
        
        [Inject] private IInputSystem _inputSystem;
        [Inject] private Camera _camera;
        [Inject] private ISubscriber<OnClickInteractableEvent> _onClickInteractableSub;
        
        private IAstarAI _astarAI;
        private bool _isBlocked;
        private float _lastClickTime;
        
        void IComponent.Init()
        {
            _astarAI = GetComponent<IAstarAI>();
            
            if (_astarAI != null) 
                _astarAI.onSearchPath += Update;
            
            _inputSystem.OnPuzzleMouseClick.DelayFrame(1).Subscribe(SetTarget).AddTo(this);
            _onClickInteractableSub.Subscribe(OnClickInteractable).AddTo(this);
        }

        private void SetTarget(InputVectorActionData data)
        {
            if (data.Phase is not InputActionPhase.Performed) return;
            if (Time.time - _lastClickTime <= DOUBLE_CLICK_TIME && _isBlocked)
            {
                _astarAI.Teleport(_target.position);
            }
            _lastClickTime = Time.time;
            if (_isBlocked) return;
            
            var screenPos = data.Vector;
            var ray = _camera.ScreenPointToRay(screenPos);

            var hit = Physics2D.GetRayIntersection(ray, 100, _moveMask);
            if (hit.collider != null)
            {
                _target.position = new Vector3(hit.point.x, hit.point.y, 0);
                _isBlocked = true;
            }
        }

        private void OnClickInteractable(OnClickInteractableEvent msg)
        {
            if (_isBlocked) return;
            _isBlocked = true;
            _target.position = msg.DestinationPoint.position;
        }

        public void BlockMovement(bool isBlocked)
        {
            _isBlocked = isBlocked;
        }

        private void OnDestroy()
        {
            _astarAI.onSearchPath -= Update;
        }
        
        private void Update ()
        {
            if (!_target || _astarAI == null)
                return;
            
            _astarAI.destination = _target.position;
            if (_astarAI.reachedEndOfPath)
            {
                _isBlocked = false;
            }
        }
    }
}