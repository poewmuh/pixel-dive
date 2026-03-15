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
        [SerializeField] private SerializedDictionary<CurrentLevelType, DepthSettings> _levelDepthSettings;
        
        
        [SerializeField] private Transform _view;

        private IAstarAI _astarAI;
        private CurrentLevelType _currentLevelType;
        
        void IComponent.Init()
        {
            TryGetComponent(out _astarAI);
        }

        private void Update()
        {
            var depthSettings = _levelDepthSettings[_currentLevelType];
            var t = Mathf.InverseLerp(depthSettings.NearY, depthSettings.FayY, transform.position.y);
            var scale = Mathf.Lerp(depthSettings.NearScale, depthSettings.FarScale, t);
            var speed = Mathf.Lerp(depthSettings.NearSpeed, depthSettings.FarSpeed, t);
        
            _view.localScale = Vector3.one * scale;
            _astarAI.maxSpeed = speed;
        }

        public void SetLevel(CurrentLevelType levelType)
        {
            _currentLevelType = levelType;
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
