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
    public sealed class AStarMoverComponent : MonoBehaviour, IComponent
    {
        private const float DoubleClickTime = 0.2f;
        private const float MovementSqrThreshold = 0.01f;

        [SerializeField] private Transform _target;
        [SerializeField] private LayerMask _moveMask;

        [Inject] private IInputSystem _inputSystem;
        [Inject] private Camera _camera;
        [Inject] private ISubscriber<OnClickInteractableEvent> _interactableClickSubscriber;
        [Inject] private IPublisher<ReachedInteractableEvent> _reachedInteractablePublisher;

        private IAstarAI _astarAI;
        private MovementState _state;
        private IInteractable _targetInteractable;
        private float _lastClickTime;
        private bool _hasStartedMoving;

        void IComponent.Init()
        {
            _astarAI = GetComponent<IAstarAI>();
            _astarAI.onSearchPath += UpdateDestination;
            _astarAI.canSearch = false;

            _inputSystem.OnPuzzleMouseClick
                .DelayFrame(1)
                .Subscribe(OnMovementClick)
                .AddTo(this);
            _interactableClickSubscriber
                .Subscribe(OnInteractableClick)
                .AddTo(this);
        }

        private void OnMovementClick(InputVectorActionData input)
        {
            if (input.Phase is not InputActionPhase.Performed) return;

            if (IsMoving())
            {
                if (ConsumeDoubleClick())
                {
                    TeleportToCurrentDestination();
                }

                return;
            }

            if (_state is MovementState.Blocked) return;
            
            ConsumeDoubleClick();

            if (TryGetMovementPoint(input.Vector, out var destination))
            {
                StartMovement(destination, null);
            }
        }

        private void OnInteractableClick(OnClickInteractableEvent message)
        {
            if (_state is not MovementState.Idle) return;
            if (message.DestinationPoint == null) return;

            StartMovement(message.DestinationPoint.position, message.Interactable);
        }

        private void StartMovement(Vector3 destination, IInteractable interactable)
        {
            _target.position = new Vector3(destination.x, destination.y, 0f);
            _targetInteractable = interactable;
            _hasStartedMoving = false;
            _state = interactable == null
                ? MovementState.MovingToPoint
                : MovementState.MovingToInteractable;

            _astarAI.isStopped = false;
            UpdateDestination();
            _astarAI.SearchPath();
        }

        private void Update()
        {
            if (!IsMoving()) return;
            if (_astarAI.pathPending) return;

            if (!_hasStartedMoving)
            {
                UpdateMovementStart();
                return;
            }

            if (_astarAI.reachedEndOfPath)
            {
                CompleteMovement();
            }
        }

        private void UpdateMovementStart()
        {
            if (_astarAI.reachedEndOfPath || !_astarAI.hasPath)
            {
                CompleteMovement();
                return;
            }

            if (_astarAI.velocity.sqrMagnitude > MovementSqrThreshold)
            {
                _hasStartedMoving = true;
            }
        }

        private bool TryGetMovementPoint(Vector2 screenPosition, out Vector3 destination)
        {
            destination = default;
            var ray = _camera.ScreenPointToRay(screenPosition);
            var hit = Physics2D.GetRayIntersection(ray, 100f, _moveMask);
            if (!hit.collider) return false;

            destination = hit.point;
            return true;
        }

        private bool ConsumeDoubleClick()
        {
            var currentTime = Time.time;
            var isDoubleClick = _lastClickTime > 0f &&
                                currentTime - _lastClickTime <= DoubleClickTime;
            _lastClickTime = currentTime;
            return isDoubleClick;
        }

        private bool IsMoving()
        {
            return _state is MovementState.MovingToPoint or MovementState.MovingToInteractable;
        }

        private void TeleportToCurrentDestination()
        {
            _astarAI.Teleport(_target.position);
            CompleteMovement();
        }

        private void CompleteMovement()
        {
            var reachedInteractable = _targetInteractable;

            ResetMovement();

            if (reachedInteractable != null)
            {
                _reachedInteractablePublisher.Publish(new ReachedInteractableEvent(reachedInteractable));
            }
        }

        private void UpdateDestination()
        {
            _astarAI.destination = _target.position;
        }

        public void TeleportToPos(Transform target)
        {
            _target.position = target.position;
            _astarAI.Teleport(target.position);
            ResetMovement();
        }

        public void BlockMovement(bool isBlocked)
        {
            if (isBlocked)
            {
                StopCurrentMovement();
                _state = MovementState.Blocked;
            }
            else if (_state is MovementState.Blocked)
            {
                _state = MovementState.Idle;
            }
        }

        private void ResetMovement()
        {
            StopCurrentMovement();
            _state = MovementState.Idle;
        }

        private void StopCurrentMovement()
        {
            _astarAI.isStopped = true;
            _targetInteractable = null;
            _hasStartedMoving = false;
        }

        private void OnDestroy()
        {
            if (_astarAI != null)
            {
                _astarAI.onSearchPath -= UpdateDestination;
            }
        }

        private enum MovementState
        {
            Idle,
            MovingToPoint,
            MovingToInteractable,
            Blocked
        }
    }
}
