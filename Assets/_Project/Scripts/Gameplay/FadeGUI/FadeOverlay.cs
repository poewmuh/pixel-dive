using _Project.GUI;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Gameplay.FadeGUI
{
    public class FadeOverlay : BaseOverlay
    {
        [SerializeField] private Fading _fading;
        [SerializeField] private Image _fadingImage;
        
        [Inject] private ISubscriber<FadeMessageEvent> _fadeMessageSub;

        private void Start()
        {
            _fadeMessageSub.Subscribe(OnFadeRequest).AddTo(this);
        }

        private async void OnFadeRequest(FadeMessageEvent msg)
        {
            _fadingImage.color = msg.FadeColor;
            if (msg.FadeType == FadeType.FadeIn) 
                await _fading.ShowAsync();
            else
                await _fading.HideAsync();
            
            msg.Completion?.TrySetResult();
        }
    }
}
