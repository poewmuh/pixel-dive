using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Gameplay.Platforming.Level
{
    [RequireComponent(typeof(Collider2D))]
    public class DoSomethingTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onPlayerEnter;
        [SerializeField] private bool _disableOnExecute = true;
        
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
                _onPlayerEnter?.Invoke();
                if (_disableOnExecute)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}