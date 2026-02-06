using System;
using _Project.Application.GUI.WindowsSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Windows.Loading
{
    public class LoadingWindow : WindowBase
    {
        [SerializeField] private Image _background;

        [SerializeField] private float _animationDuration;
        
        private Tween _currentTween;

        public async UniTask StartShowAnimation(Color fadeColor)
        {
            _currentTween?.Kill();
            _background.color = fadeColor;
            _currentTween = _background.DOFade(1, _animationDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(_animationDuration));
        }

        public async UniTask StartHideAnimation()
        {
            _currentTween?.Kill();
            _currentTween = _background.DOFade(0, _animationDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(_animationDuration));
        }

        protected override void OnDestroy()
        {
            _currentTween?.Kill();
        }
    }
}
