using System;
using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Player
{
    public class DynamicSpriteFlipper : MonoBehaviour, IComponent
    {
        private const float PositionThreshold = 0.001f;

        [SerializeField] private SpriteRenderer _view;
        
        private Vector3 _lastPosition;
        
        void IComponent.Init()
        {
            _lastPosition = transform.position;
        }
        
        private void Update()
        {
            var delta = transform.position - _lastPosition;
        
            if (Mathf.Abs(delta.x) > PositionThreshold)
            {
                _view.flipX = delta.x < 0;
            }
            
            _lastPosition = transform.position;
        }
    }
}
