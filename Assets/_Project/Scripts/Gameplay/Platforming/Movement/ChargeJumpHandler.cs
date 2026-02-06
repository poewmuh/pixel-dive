using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace _Project.Gameplay.Platforming.Movement
{
    public class ChargeJumpHandler : IJumpHandler
    {
        public ReactiveProperty<JumpState> State { get; } = new(JumpState.None);
        
        private readonly Rigidbody2D _rb;
        private readonly CollisionChecker _collisionChecker;
        private readonly MovementConfig _config;

        private bool _isCharging;
        private float _chargeTime;
        
        private CancellationTokenSource _cts;
        
        public ChargeJumpHandler(Rigidbody2D rb, MovementConfig config, CollisionChecker collisionChecker)
        {
            _rb = rb;
            _collisionChecker = collisionChecker;
            _config = config;
        }

        void IJumpHandler.JumpPressed()
        {
            if (_collisionChecker.isGrounded.Value)
            {
                _isCharging = true;
                _chargeTime = 0f;
                SetJumpState(JumpState.Charging);

                _cts = new CancellationTokenSource();
                ChargeJump(_cts.Token).Forget();
            }
        }

        private async UniTaskVoid ChargeJump(CancellationToken cancellationToken)
        {
            while (_isCharging && !cancellationToken.IsCancellationRequested)
            {
                _chargeTime += Time.deltaTime;
                await UniTask.NextFrame(cancellationToken);
            }
        }

        void IJumpHandler.JumpReleased()
        {
            if (_isCharging)
            {
                float jumpForce = Mathf.Lerp(_config.MinJumpForce, _config.MaxJumpForce, 
                    _chargeTime / _config.ChargeTime);
                
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
                _isCharging = false;
                _chargeTime = 0f;
                SetJumpState(JumpState.Jumping);
            }
        }

        void IJumpHandler.Tick()
        {
            if (_rb.linearVelocity.y < -0.1f)
            {
                SetJumpState(JumpState.Falling);
            }
        }

        private void SetJumpState(JumpState state)
        {
            State.Value = state;
        }

        void IDisposable.Dispose()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }
    }
}