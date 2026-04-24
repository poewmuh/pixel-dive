using R3;
using UnityEngine;
using Utils._Project.Utils.Extensions;

namespace _Project.Gameplay.Platforming.Movement
{
    public class MoveHandler
    {
        public ReactiveProperty<bool> IsMoving { get; private set; } = new ();
        
        private readonly Rigidbody2D _rb;
        private readonly CollisionChecker _collisionChecker;
        
        private Vector2 _currentVelocity;
        private MovementConfig _config;
        private bool _isCanMove = true;
        private float currentAcceleration => _collisionChecker.isGrounded.Value ? _config.Acceleration : _config.AirAcceleration;
        private float currentDeceleration => _collisionChecker.isGrounded.Value ? _config.Acceleration : _config.AirDeceleration;
        
        public MoveHandler(Rigidbody2D rb, MovementConfig config, CollisionChecker collisionChecker)
        {
            _config = config;
            _rb = rb;
            _collisionChecker = collisionChecker;
        }

        public void UpdateMove(float horizontalInput)
        {
            if (!_isCanMove) horizontalInput = 0;
            
            _currentVelocity = GetNewVelocity(horizontalInput);
            _rb.linearVelocity = new Vector2(_currentVelocity.x, _rb.linearVelocity.y);
        }

        public void SetEnableMovement(bool enable)
        {
            _isCanMove = enable;
        }
        
        private Vector2 GetNewVelocity(float horizontalInput)
        {
            if (horizontalInput.ApproxZero())
            {
                IsMoving.Value = false;
                return Vector2.Lerp(_currentVelocity, Vector2.zero, currentDeceleration * Time.fixedDeltaTime);
                
            }
            else
            {
                IsMoving.Value = true;
                var targetVelocity = new Vector2(horizontalInput, 0) * _config.MoveSpeed;
                return Vector2.Lerp(_currentVelocity, targetVelocity, currentAcceleration * Time.fixedDeltaTime);
            }
        }
    }
}