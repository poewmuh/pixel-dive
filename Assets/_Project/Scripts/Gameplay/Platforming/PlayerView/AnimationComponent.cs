using _Project.Gameplay.Platforming.Movement;
using R3;
using UnityEngine;

namespace _Project.Gameplay.Platforming.PlayerView
{
    [RequireComponent(typeof(MovementComponent))]
    public class AnimationComponent : MonoBehaviour, IComponent
    {
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsCharging = Animator.StringToHash("IsCharging");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");
        private static readonly int IsFalling = Animator.StringToHash("IsFalling");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        
        
        [SerializeField] private Animator _animator;
        
        private MovementComponent _movementComponent;

        private void Awake()
        {
            _movementComponent = GetComponent<MovementComponent>();
        }

        void IComponent.Init()
        {
            _movementComponent.MoveHandler.IsMoving.Subscribe(OnMoving).AddTo(this);
            _movementComponent.JumpHandler.State.Subscribe(JumpStateChange).AddTo(this);
            _movementComponent.CollisionChecker.isGrounded.Subscribe(OnGroundedChange).AddTo(this);
        }

        private void OnMoving(bool isMoving)
        {
            _animator.SetBool(IsMoving, isMoving);
        }

        private void OnGroundedChange(bool isGrounded)
        {
            _animator.SetBool(IsGrounded, isGrounded);
            if (isGrounded)
            {
                _animator.SetBool(IsJumping, false);
                _animator.SetBool(IsFalling, false);
            }
        }

        private void JumpStateChange(JumpState state)
        {
            switch (state)
            {
                case JumpState.Charging:
                    _animator.SetBool(IsCharging, true);
                    break;
                case JumpState.Falling:
                    _animator.SetBool(IsFalling, true);
                    _animator.SetBool(IsJumping, false);
                    _animator.SetBool(IsCharging, false);
                    break;
                case JumpState.Jumping:
                    _animator.SetBool(IsCharging, false);
                    _animator.SetBool(IsJumping, true);
                    break;
            }
        }
    }
}