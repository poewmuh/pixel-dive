using R3;

namespace _Project.Gameplay.Platforming.Damage
{
    public interface IKillable
    {
        ReactiveProperty<bool> IsDead { get; }
    }
}