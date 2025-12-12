using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Project.Application.Boot
{
    public class LogoView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup[] _slides;

        private float _showTime = 1f;
        private float _hideTime = 1f;
        private float _stayTime = 1f;
            
        public async UniTask StartShowLogo()
        {
            foreach (var slide in _slides)
            {
                slide.DOFade(1, _showTime);
                await UniTask.Delay(TimeSpan.FromSeconds(_showTime + _stayTime));
                slide.DOFade(0, _hideTime);
                await UniTask.Delay(TimeSpan.FromSeconds(_hideTime));
            }
        }
    }
}
