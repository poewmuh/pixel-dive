using R3;
using UnityEngine;

namespace _Project.Gameplay.Platforming.Damage
{
    public class HealthComponent : MonoBehaviour, IDamagable, IKillable, IComponent
    {
        [field: SerializeField]
        public SerializableReactiveProperty<int> Health { get; private set; } = new(1);
        public ReactiveProperty<bool> IsDead { get; } = new(false);

        void IComponent.Init()
        {
            
        }

        void IDamagable.TakeDamage(int damage)
        {
            Health.Value -= damage;
            if (Health.Value <= 0)
            {
                IsDead.Value = true;
                Health.Value = 1;
                IsDead.Value = false;
            }
        }
    }
}