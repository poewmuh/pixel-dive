using System;
using _Project.Application.Scene;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace _Project.Gameplay.Platforming.Level
{
    [RequireComponent(typeof(Collider2D))]
    public class NextLevelTrigger : MonoBehaviour
    {
        [SerializeField] private SceneType _sceneType;
        [SerializeField] private Color _fadeColor;
        
        [Inject] private ISceneSystem _sceneSystem;
        
        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.OnTriggerEnter2DAsObservable().Subscribe(OnEnter).AddTo(this);
        }

        private void OnEnter(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                _sceneSystem.LoadScene(_sceneType, LoadSceneMode.Single, _fadeColor);
            }
        }
    }
}
