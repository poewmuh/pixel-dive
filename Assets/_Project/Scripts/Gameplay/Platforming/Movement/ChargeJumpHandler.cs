using System;
using R3;
using UnityEngine;

namespace _Project.Gameplay.Platforming.Movement
{
    public sealed class ChargeJumpHandler : IJumpHandler
    {
        public ReactiveProperty<JumpState> State { get; } = new(JumpState.None);

        private readonly Rigidbody2D _rb;
        private readonly CollisionChecker _collisionChecker;
        private readonly MovementConfig _config;

        private bool _isCharging;
        private float _chargeTime;

        public ChargeJumpHandler(Rigidbody2D rb, MovementConfig config, CollisionChecker collisionChecker)
        {
            _rb = rb;
            _config = config;
            _collisionChecker = collisionChecker;
        }

        void IJumpHandler.JumpPressed()
        {
            if (_isCharging || !_collisionChecker.IsGrounded.Value) return;

            _isCharging = true;
            _chargeTime = 0f;
            SetJumpState(JumpState.Charging);
        }

        void IJumpHandler.JumpReleased()
        {
            if (!_isCharging) return;

            var chargeProgress = Mathf.Clamp01(_chargeTime / _config.ChargeTime);
            var jumpForce = Mathf.Lerp(_config.MinJumpForce, _config.MaxJumpForce, chargeProgress);

            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _isCharging = false;
            _chargeTime = 0f;
            SetJumpState(JumpState.Jumping);
        }

        void IJumpHandler.Tick()
        {
            if (_isCharging)
            {
                _chargeTime += Time.fixedDeltaTime;
                return;
            }

            if (!_collisionChecker.IsGrounded.Value && _rb.linearVelocity.y < -0.1f)
            {
                SetJumpState(JumpState.Falling);
            }
            else if (_collisionChecker.IsGrounded.Value)
            {
                SetJumpState(JumpState.None);
            }
        }

        private void SetJumpState(JumpState state)
        {
            if (State.Value == state) return;
            State.Value = state;
        }

        void IDisposable.Dispose()
        {
            State.Dispose();
        }
    }
}
