using _Project.Application.Scene;
using Cysharp.Threading.Tasks;
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
        private bool _isTriggered;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.OnTriggerEnter2DAsObservable().Subscribe(OnEnter).AddTo(this);
        }

        private void OnEnter(Collider2D collider)
        {
            if (_isTriggered || !collider.CompareTag("Player")) return;

            _isTriggered = true;
            _collider.enabled = false;
            LoadNextLevel().Forget();
        }

        private async UniTaskVoid LoadNextLevel()
        {
            await _sceneSystem.LoadScene(_sceneType, LoadSceneMode.Single, _fadeColor);
        }
    }
}
