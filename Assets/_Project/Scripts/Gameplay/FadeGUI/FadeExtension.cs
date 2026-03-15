using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;

namespace _Project.Gameplay.FadeGUI
{
    public static class FadeExtension
    {
        public static UniTask Fade(this IPublisher<FadeMessageEvent> pub, FadeType type, Color color, CancellationToken token)
        {
            var completion = new UniTaskCompletionSource();
            pub.Publish(new FadeMessageEvent(type, color, completion));
            return completion.Task.AttachExternalCancellation(token);
        }
    }
}