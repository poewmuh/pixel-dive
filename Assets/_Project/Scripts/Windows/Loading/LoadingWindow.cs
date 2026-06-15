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
            KillCurrentTween();
            _background.color = fadeColor;
            var transparentColor = _background.color;
            transparentColor.a = 0f;
            _background.color = transparentColor;
            _currentTween = _background
                .DOFade(1f, _animationDuration)
                .SetUpdate(isIndependentUpdate: true);
            await _currentTween.AsyncWaitForCompletion();
        }

        public async UniTask StartHideAnimation()
        {
            KillCurrentTween();
            _currentTween = _background
                .DOFade(0f, _animationDuration)
                .SetUpdate(isIndependentUpdate: true);
            await _currentTween.AsyncWaitForCompletion();
        }

        protected override void OnDestroy()
        {
            KillCurrentTween();
            base.OnDestroy();
        }

        private void KillCurrentTween()
        {
            if (_currentTween is { active: true })
            {
                _currentTween.Kill();
            }

            _currentTween = null;
        }
    }
}
