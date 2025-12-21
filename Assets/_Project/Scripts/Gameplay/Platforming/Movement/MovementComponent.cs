using _Project.Gameplay.Platforming.Input;
using R3;
using UnityEngine;

namespace _Project.Gameplay.Platforming.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(InputComponent))]
    public class MovementComponent : MonoBehaviour, IComponent
    {
        [SerializeField] private MovementConfig _movementConfig;
        [Header("Links")]
        [SerializeField] private Collider2D _headCollider;
        [SerializeField] private Collider2D _groundCollider;
        
        public MoveHandler MoveHandler { get; private set; }
        public IJumpHandler JumpHandler { get; private set; }
        public CollisionChecker CollisionChecker { get; private set; }
        
        private Rigidbody2D _rb;
        private InputComponent _inputComponent;
        private float _horizontalInput;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _inputComponent = GetComponent<InputComponent>();
            
            CollisionChecker = new CollisionChecker(_movementConfig.GroundLayer, _groundCollider,  _headCollider);
            MoveHandler = new MoveHandler(_rb, _movementConfig, CollisionChecker);
            JumpHandler = new ChargeJumpHandler(_rb, _movementConfig, CollisionChecker);
        }

        void IComponent.Init()
        {
            _inputComponent.JumpState.Subscribe(OnJump).AddTo(this);
            _inputComponent.MoveState.Subscribe(OnMove).AddTo(this);
        }
        
        private void OnMove(float horizontalInput)
        {
            _horizontalInput = horizontalInput;
        }

        private void OnJump(bool performed)
        {
            if (performed)
            {
                JumpHandler.JumpPressed();
                MoveHandler.SetEnableMovement(false);
            }
            else
            {
                MoveHandler.SetEnableMovement(true);
                JumpHandler.JumpReleased();
            }
        }

        private void FixedUpdate()
        {
            MoveHandler.UpdateMove(_horizontalInput);
            JumpHandler.Tick();
        }

        private void OnDestroy()
        {
            CollisionChecker.Dispose();
        }
    }
}
