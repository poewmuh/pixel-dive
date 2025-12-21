using System;
using R3;
using R3.Triggers;
using UnityEngine;

namespace _Project.Gameplay.Platforming.Movement
{
    public class CollisionChecker : IDisposable
    {
        public ReactiveProperty<bool> isGrounded { get; } = new();
        public ReactiveProperty<bool> isBumpedHead { get; } = new();
        
        private readonly Collider2D _groundCollider;
        private readonly Collider2D _headCollider;
        private readonly LayerMask _groundMask;
        private readonly CompositeDisposable _disposables = new();
        
        
        public CollisionChecker(LayerMask groundLayer, Collider2D groundCollider, Collider2D headCollider)
        {
            _groundMask = groundLayer;
            groundCollider.OnCollisionEnter2DAsObservable().Subscribe(EnterGrounded).AddTo(_disposables);
            headCollider.OnCollisionEnter2DAsObservable().Subscribe(EnterHeadBump).AddTo(_disposables);
            groundCollider.OnCollisionExit2DAsObservable().Subscribe(ExitGrounded).AddTo(_disposables);
            headCollider.OnCollisionExit2DAsObservable().Subscribe(ExitHeadBump).AddTo(_disposables);
        }
        
        private void EnterGrounded(Collision2D collision)
        {
            if (!isGrounded.Value)
            {
                isGrounded.Value = IsGroundCollision(collision);
            }
        }
        
        private void EnterHeadBump(Collision2D collision)
        {
            if (!isBumpedHead.Value)
            {
                isBumpedHead.Value = IsGroundCollision(collision);
            }
        }
        
        private void ExitGrounded(Collision2D collision)
        {
            if (IsGroundCollision(collision) && isGrounded.Value)
            {
                isGrounded.Value = false;
            }
        }
        
        private void ExitHeadBump(Collision2D collision)
        {
            if (IsGroundCollision(collision) && isBumpedHead.Value)
            {
                isBumpedHead.Value = false;
            }
        }

        private bool IsGroundCollision(Collision2D collision)
        {
            return (_groundMask.value & (1 << collision.gameObject.layer)) != 0;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}