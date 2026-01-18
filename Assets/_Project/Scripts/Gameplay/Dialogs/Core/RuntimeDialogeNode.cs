using System;

namespace _Project.Gameplay.Dialogs.Core
{
    [Serializable]
    public class RuntimeDialogeNode : RuntimeBaseNode
    {
        public string ActorName;
        public string Message;
        public EmotionType Emotion;
    }
}