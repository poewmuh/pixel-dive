using System;
using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Player
{
    public class DynamicSpriteFlipper : MonoBehaviour, IComponent
    {
        [SerializeField] private SpriteRenderer _view;
        
        private Vector3 _lastPosition;
        
        void IComponent.Init()
        {
            _lastPosition = transform.position;
        }
        
        private void Update()
        {
            var delta = transform.position - _lastPosition;
        
            if (Mathf.Abs(delta.x) > 0.001f)
            {
                _view.flipX = delta.x < 0;
            }
            
            _lastPosition = transform.position;
        }
    }
}