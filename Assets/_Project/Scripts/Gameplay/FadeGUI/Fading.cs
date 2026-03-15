using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Project.Gameplay.FadeGUI
{
    public class Fading : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _appearDuration;
        [SerializeField] private float _disappearDuration;

        public async UniTask ShowAsync(bool isForce = false)
        {
            if (isForce)
            {
                _canvasGroup.alpha = 1f;
                return;
            }
            await _canvasGroup.DOFade(1, _appearDuration).SetUpdate(isIndependentUpdate: true).AsyncWaitForCompletion();
        }

        public async UniTask HideAsync(bool isForce = false)
        {
            if (isForce)
            {
                _canvasGroup.alpha = 0f;
                return;
            }
            await _canvasGroup.DOFade(0, _appearDuration).SetUpdate(isIndependentUpdate: true).AsyncWaitForCompletion();
        }
    }
}