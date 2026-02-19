using _Project.Gameplay.Platforming.PlayerView;
using Pathfinding;
using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Player
{
    public class AnimatorComponent : MonoBehaviour, IComponent
    {
        [SerializeField] private Animator _animator;
        private IAstarAI _astarAI;
        
        void IComponent.Init()
        {
            TryGetComponent(out _astarAI);
        }

        private void Update()
        {
            _animator.SetBool(AnimatorHelper.IsMoving, _astarAI.velocity.sqrMagnitude > .01f);
        }
    }
}