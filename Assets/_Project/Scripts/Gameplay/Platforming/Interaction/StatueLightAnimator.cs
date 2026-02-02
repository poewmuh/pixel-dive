using DG.Tweening;
using UnityEngine;

namespace _Project.Gameplay.Platforming.Interaction
{
    public class StatueLightAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Sequence _sequence;
        private bool _isAppeared;

        public void AppearAnimation()
        {
            _sequence = DOTween.Sequence();
            _sequence
                .Append(_spriteRenderer.DOFade(1, 0.3f))
                .Append(_spriteRenderer.DOFade(0.3f, 0.2f))
                .Append(_spriteRenderer.DOFade(0.6f, 0.2f));

            _isAppeared = true;
        }

        public void DisappearAnimation()
        {
            if (!_isAppeared) return;
            _sequence.Kill(true);
            _sequence = DOTween.Sequence();
            _sequence
                .Append(_spriteRenderer.DOFade(0, 0.1f))
                .Append(_spriteRenderer.DOFade(1f, 0.1f))
                .Join(_spriteRenderer.DOColor(Color.pink, .1f))
                .Append(_spriteRenderer.DOFade(0.2f, 0.1f))
                .Append(_spriteRenderer.DOFade(1f, 0.1f))
                .Append(_spriteRenderer.DOColor(Color.white, .01f))
                .Append(_spriteRenderer.DOFade(0f, 0.2f));
            _isAppeared = false;
        }
    }
}
