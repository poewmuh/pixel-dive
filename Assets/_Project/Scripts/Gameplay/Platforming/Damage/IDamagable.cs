using R3;

namespace _Project.Gameplay.Platforming.Damage
{
    public interface IDamagable
    {
        SerializableReactiveProperty<int> Health { get; }
        
        void TakeDamage(int damage);
    }
}
