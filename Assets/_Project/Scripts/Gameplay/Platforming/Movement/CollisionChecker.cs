using System;
using System.Collections.Generic;
using R3;
using R3.Triggers;
using UnityEngine;

namespace _Project.Gameplay.Platforming.Movement
{
    public sealed class CollisionChecker : IDisposable
    {
        public ReactiveProperty<bool> IsGrounded { get; } = new();
        public ReactiveProperty<bool> IsHeadBlocked { get; } = new();

        private readonly LayerMask _groundMask;
        private readonly CompositeDisposable _disposables = new();
        private readonly HashSet<int> _groundContacts = new();
        private readonly HashSet<int> _headContacts = new();

        public CollisionChecker(LayerMask groundLayer, Collider2D groundCollider, Collider2D headCollider)
        {
            _groundMask = groundLayer;

            groundCollider.OnCollisionEnter2DAsObservable()
                .Subscribe(collision => AddContact(collision, _groundContacts, IsGrounded))
                .AddTo(_disposables);
            groundCollider.OnCollisionExit2DAsObservable()
                .Subscribe(collision => RemoveContact(collision, _groundContacts, IsGrounded))
                .AddTo(_disposables);

            headCollider.OnCollisionEnter2DAsObservable()
                .Subscribe(collision => AddContact(collision, _headContacts, IsHeadBlocked))
                .AddTo(_disposables);
            headCollider.OnCollisionExit2DAsObservable()
                .Subscribe(collision => RemoveContact(collision, _headContacts, IsHeadBlocked))
                .AddTo(_disposables);
        }

        private void AddContact(
            Collision2D collision,
            HashSet<int> contacts,
            ReactiveProperty<bool> contactState)
        {
            if (!IsGroundCollision(collision)) return;

            contacts.Add(collision.collider.GetInstanceID());
            contactState.Value = contacts.Count > 0;
        }

        private void RemoveContact(
            Collision2D collision,
            HashSet<int> contacts,
            ReactiveProperty<bool> contactState)
        {
            if (!IsGroundCollision(collision)) return;

            contacts.Remove(collision.collider.GetInstanceID());
            contactState.Value = contacts.Count > 0;
        }

        private bool IsGroundCollision(Collision2D collision)
        {
            return (_groundMask.value & (1 << collision.gameObject.layer)) != 0;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            IsGrounded.Dispose();
            IsHeadBlocked.Dispose();
        }
    }
}
