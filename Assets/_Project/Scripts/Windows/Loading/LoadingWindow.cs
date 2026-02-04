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

        public async UniTask StartShowAnimation(Color fadeColor)
        {
            _background.color = fadeColor;
            _background.DOFade(1, _animationDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(_animationDuration));
        }

        public async UniTask StartHideAnimation()
        {
            _background.DOFade(0, _animationDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(_animationDuration));
        }
    }
}
