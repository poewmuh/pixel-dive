using UnityEngine;

namespace _Project.Gameplay.Platforming.PlayerView
{
    public static class AnimatorHelper
    {
        public static readonly int IsMoving = Animator.StringToHash("IsMoving");
        public static readonly int IsCharging = Animator.StringToHash("IsCharging");
        public static readonly int IsJumping = Animator.StringToHash("IsJumping");
        public static readonly int IsFalling = Animator.StringToHash("IsFalling");
        public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    }
}