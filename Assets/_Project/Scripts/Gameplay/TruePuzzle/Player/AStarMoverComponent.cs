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
        [Inject] private IPublisher<ReachedInteractableEvent> _onReachedInteractablePub;
        
        private IAstarAI _astarAI;
        private bool _isBlocked;
        private float _lastClickTime;
        private bool _isClickInteractable;
        private InteractableType _interactableType;
        private bool _isMoving;
        private bool _isMoved;
        
        void IComponent.Init()
        {
            _astarAI = GetComponent<IAstarAI>();
            _astarAI.onSearchPath += SetDestination;
            
            _inputSystem.OnPuzzleMouseClick.DelayFrame(1).Subscribe(SetTarget).AddTo(this);
            _onClickInteractableSub.Subscribe(OnClickInteractable).AddTo(this);
            _astarAI.canSearch = false;
        }

        private void SetTarget(InputVectorActionData data)
        {
            if (data.Phase is not InputActionPhase.Performed) return;
            
            if (Time.time - _lastClickTime <= DOUBLE_CLICK_TIME && _isBlocked)
            {
                _hasStartedMoving = true;
                TeleportToPos(_target);
            }
            _lastClickTime = Time.time;
            if (_isBlocked) return;
            
            var screenPos = data.Vector;
            var ray = _camera.ScreenPointToRay(screenPos);

            var hit = Physics2D.GetRayIntersection(ray, 100, _moveMask);
            if (hit.collider != null)
            {
                _target.position = new Vector3(hit.point.x, hit.point.y, 0);
                _astarAI.SearchPath();
                _isBlocked = true;
                _isClickInteractable = false;
                _astarAI.isStopped = false;
            }
        }

        public void TeleportToPos(Transform target)
        {
            _target.position = target.position;
            _astarAI.Teleport(target.position);
        }

        private void OnClickInteractable(OnClickInteractableEvent msg)
        {
            if (_isBlocked) return;

            _isBlocked = true;
            if (Vector3.Distance(_target.position, msg.DestinationPoint.position) <= 0.1f)
            {
                _hasStartedMoving = true;
            }
            _target.position = msg.DestinationPoint.position;
            _astarAI.SearchPath();
            _isClickInteractable = true;
            _interactableType = msg.InteractableType;
            _astarAI.isStopped = false;
        }

        public void BlockMovement(bool isBlocked)
        {
            _isBlocked = isBlocked;
        }
        
        private bool _hasStartedMoving;
        
        private void Update ()
        {
            if (!_target || _astarAI == null)
                return;
            SetDestination();

            if (!_hasStartedMoving)
            {
                if (!_astarAI.pathPending && _astarAI.velocity.sqrMagnitude > 0.01f)
                {
                    _hasStartedMoving = true;
                }
                else if (!_astarAI.pathPending && !_astarAI.hasPath)
                {
                    OnArrived();
                }
                return;
            }
            
            if (_astarAI.reachedEndOfPath && !_astarAI.pathPending)
            {
                OnArrived();
            }
        }
        
        private void OnArrived()
        {
            _hasStartedMoving = false;
            _astarAI.isStopped = true;
            _isBlocked = false;

            if (_isClickInteractable)
            {
                _onReachedInteractablePub.Publish(new ReachedInteractableEvent(_interactableType));
                _isClickInteractable = false;
            }
        }

        private void SetDestination()
        {
            _astarAI.destination = _target.position;
        }

        private void OnDestroy()
        {
            _astarAI.onSearchPath -= SetDestination;
        }
    }
}