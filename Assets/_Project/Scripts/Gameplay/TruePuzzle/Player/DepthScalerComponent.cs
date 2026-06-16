using System;
using _Project.Gameplay.TruePuzzle.Level;
using AYellowpaper.SerializedCollections;
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
        private const float PositionThreshold = 0.001f;

        [SerializeField] private SerializedDictionary<CurrentLevelType, DepthSettings> _levelDepthSettings;
        
        
        [SerializeField] private Transform _view;

        private IAstarAI _astarAI;
        private CurrentLevelType _currentLevelType;
        private DepthSettings _depthSettings;
        private float _lastY = float.NaN;
        
        void IComponent.Init()
        {
            TryGetComponent(out _astarAI);
            SetLevel(_currentLevelType);
            ApplyDepth();
        }

        private void Update()
        {
            if (_depthSettings == null) return;
            if (Mathf.Abs(transform.position.y - _lastY) <= PositionThreshold) return;
            ApplyDepth();
        }

        private void ApplyDepth()
        {
            _lastY = transform.position.y;
            var t = Mathf.InverseLerp(_depthSettings.NearY, _depthSettings.FayY, _lastY);
            var scale = Mathf.Lerp(_depthSettings.NearScale, _depthSettings.FarScale, t);
            var speed = Mathf.Lerp(_depthSettings.NearSpeed, _depthSettings.FarSpeed, t);
        
            _view.localScale = Vector3.one * scale;
            _astarAI.maxSpeed = speed;
        }

        public void SetLevel(CurrentLevelType levelType)
        {
            _currentLevelType = levelType;
            if (!_levelDepthSettings.TryGetValue(_currentLevelType, out _depthSettings))
            {
                Debug.LogWarning($"[DepthScalerComponent] Missing depth settings for level {_currentLevelType}.");
            }

            _lastY = float.NaN;
        }
    }

    [Serializable]
    public class DepthSettings
    {
        [SerializeField, Group("Y Settings")] public float NearY = -10f;
        [SerializeField, Group("Y Settings")] public float FayY = 1f;
        [SerializeField, Group("Scale Settings")] public float NearScale = 2f;
        [SerializeField, Group("Scale Settings")] public float FarScale = 0.5f;
        [SerializeField, Group("Speed Settings")] public float NearSpeed = 10f;
        [SerializeField, Group("Speed Settings")] public float FarSpeed = 1f;
    }
}
