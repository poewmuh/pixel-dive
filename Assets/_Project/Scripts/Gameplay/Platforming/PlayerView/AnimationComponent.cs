using _Project.Gameplay.Platforming.Movement;
using R3;
using UnityEngine;

namespace _Project.Gameplay.Platforming.PlayerView
{
    [RequireComponent(typeof(MovementComponent))]
    public class AnimationComponent : MonoBehaviour, IComponent
    {
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
            _animator.SetBool(AnimatorHelper.IsMoving, isMoving);
        }

        private void OnGroundedChange(bool isGrounded)
        {
            _animator.SetBool(AnimatorHelper.IsGrounded, isGrounded);
            if (isGrounded)
            {
                _animator.SetBool(AnimatorHelper.IsJumping, false);
                _animator.SetBool(AnimatorHelper.IsFalling, false);
            }
        }

        private void JumpStateChange(JumpState state)
        {
            switch (state)
            {
                case JumpState.Charging:
                    _animator.SetBool(AnimatorHelper.IsCharging, true);
                    break;
                case JumpState.Falling:
                    _animator.SetBool(AnimatorHelper.IsFalling, true);
                    _animator.SetBool(AnimatorHelper.IsJumping, false);
                    _animator.SetBool(AnimatorHelper.IsCharging, false);
                    break;
                case JumpState.Jumping:
                    _animator.SetBool(AnimatorHelper.IsCharging, false);
                    _animator.SetBool(AnimatorHelper.IsJumping, true);
                    break;
            }
        }
    }
}