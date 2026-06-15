using System;
using R3;
using UnityEngine;
using Utils._Project.Utils.Extensions;

namespace _Project.Gameplay.Platforming.Movement
{
    public sealed class MoveHandler : IDisposable
    {
        public ReactiveProperty<bool> IsMoving { get; } = new();

        private readonly Rigidbody2D _rb;
        private readonly CollisionChecker _collisionChecker;
        private readonly MovementConfig _config;

        private bool _isMovementEnabled = true;

        private float CurrentAcceleration =>
            _collisionChecker.IsGrounded.Value ? _config.Acceleration : _config.AirAcceleration;

        private float CurrentDeceleration =>
            _collisionChecker.IsGrounded.Value ? _config.Deceleration : _config.AirDeceleration;

        public MoveHandler(Rigidbody2D rb, MovementConfig config, CollisionChecker collisionChecker)
        {
            _rb = rb;
            _config = config;
            _collisionChecker = collisionChecker;
        }

        public void UpdateMove(float horizontalInput)
        {
            if (!_isMovementEnabled)
            {
                horizontalInput = 0f;
            }

            var horizontalVelocity = GetHorizontalVelocity(horizontalInput);
            _rb.linearVelocity = new Vector2(horizontalVelocity, _rb.linearVelocity.y);
        }

        public void SetMovementEnabled(bool isEnabled)
        {
            _isMovementEnabled = isEnabled;
        }

        private float GetHorizontalVelocity(float horizontalInput)
        {
            var currentVelocity = _rb.linearVelocity.x;

            if (horizontalInput.ApproxZero())
            {
                IsMoving.Value = false;
                return Mathf.Lerp(currentVelocity, 0f, CurrentDeceleration * Time.fixedDeltaTime);
            }

            IsMoving.Value = true;
            var targetVelocity = horizontalInput * _config.MoveSpeed;
            return Mathf.Lerp(currentVelocity, targetVelocity, CurrentAcceleration * Time.fixedDeltaTime);
        }

        public void Dispose()
        {
            IsMoving.Dispose();
        }
    }
}
