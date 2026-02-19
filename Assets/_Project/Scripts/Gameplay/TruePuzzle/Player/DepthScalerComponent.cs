using System;
using Pathfinding;
using TriInspector;
using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Player
{
    [DeclareBoxGroup("Y Settings")]
    [DeclareBoxGroup("Scale Settings")]
    [DeclareBoxGroup("Speed Settings")]
    public class DepthScalerComponent : MonoBehaviour, IComponent
    {
        [SerializeField, Group("Y Settings")] private float _nearY = -3f;
        [SerializeField, Group("Y Settings")] private float _farY = 3f;
        [SerializeField, Group("Scale Settings")] private float _nearScale = 1f;
        [SerializeField, Group("Scale Settings")] private float _farScale = 0.4f;
        [SerializeField, Group("Speed Settings")] private float _nearSpeed = 3f;
        [SerializeField, Group("Speed Settings")] private float _farSpeed = 1f;
        
        [SerializeField] private Transform _view;

        private IAstarAI _astarAI;
        
        void IComponent.Init()
        {
            TryGetComponent(out _astarAI);
        }

        private void Update()
        {
            var t = Mathf.InverseLerp(_nearY, _farY, transform.position.y);
            var scale = Mathf.Lerp(_nearScale, _farScale, t);
            var speed = Mathf.Lerp(_nearSpeed, _farSpeed, t);
        
            _view.localScale = Vector3.one * scale;
            _astarAI.maxSpeed = speed;
        }
    }
}
