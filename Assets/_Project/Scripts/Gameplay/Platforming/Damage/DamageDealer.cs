using R3;
using R3.Triggers;
using UnityEngine;

namespace _Project.Gameplay.Platforming.Damage
{
    [RequireComponent(typeof(Collider2D))]
    public class DamageDealer : MonoBehaviour
    {
        [SerializeField] private int _damage;
        
        private Collider2D _collider;

        public void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.OnTriggerEnter2DAsObservable().Subscribe(OnEnter).AddTo(this);
        }

        private void OnEnter(Collider2D colldier)
        {
            var damagable = colldier.GetComponentInParent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(_damage);
            }
        }
    }
}