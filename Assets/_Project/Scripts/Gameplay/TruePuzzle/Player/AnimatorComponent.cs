using _Project.Gameplay.Platforming.PlayerView;
using Pathfinding;
using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Player
{
    public class AnimatorComponent : MonoBehaviour, IComponent
    {
        [SerializeField] private Animator _animator;
        private IAstarAI _astarAI;
        private bool _isMoving;
        
        void IComponent.Init()
        {
            TryGetComponent(out _astarAI);
        }

        private void Update()
        {
            var isMoving = _astarAI.velocity.sqrMagnitude > .01f;
            if (_isMoving == isMoving) return;

            _isMoving = isMoving;
            _animator.SetBool(AnimatorHelper.IsMoving, _isMoving);
        }
    }
}
