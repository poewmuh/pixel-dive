using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Gameplay.FadeGUI
{
    public class FadeMessageEvent
    {
        public FadeType FadeType { get; }
        public Color FadeColor { get; }
        public readonly UniTaskCompletionSource Completion;

        public FadeMessageEvent(FadeType fadeType, Color fadeColor, UniTaskCompletionSource completion )
        {
            FadeType = fadeType;
            FadeColor = fadeColor;
            Completion = completion;
        }
    }

    public enum FadeType
    {
        FadeIn,
        FadeOut
    }
}
