using DG.Tweening;
using UnityEngine;

namespace _Project.Gameplay.Platforming.AI
{
    public class HintFishAI : MonoBehaviour
    {
        [SerializeField] private Transform _viewTransform;
        private Sequence _hintSequence;
        
        private void Start()
        {
            _hintSequence = DOTween.Sequence();

            _hintSequence
                .Append(_viewTransform.DOLocalMoveX(-1, .2f).SetEase(Ease.Linear))
                .Append(_viewTransform.DOLocalMoveX(0, .1f).SetEase(Ease.Linear))
                .AppendInterval(.5f)
                .Append(_viewTransform.DOLocalMoveX(-1, .1f).SetEase(Ease.Linear))
                .Append(_viewTransform.DOLocalMoveX(0, .1f).SetEase(Ease.Linear))
                .AppendInterval(.5f)
                .Append(_viewTransform.DOLocalMoveY(1, .1f).SetEase(Ease.Linear))
                .Append(_viewTransform.DOLocalMoveY(0, .1f).SetEase(Ease.Linear))
                .AppendInterval(.5f)
                .SetLoops(-1, LoopType.Restart);
        }
    }
}