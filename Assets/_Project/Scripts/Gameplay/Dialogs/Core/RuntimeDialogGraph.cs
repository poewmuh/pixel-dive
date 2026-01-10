using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace _Project.Gameplay.Dialogs.Core
{
    public class RuntimeDialogGraph : ScriptableObject
    {
        public string EntryNodeId;
        
        [SerializeReference]
        public List<RuntimeBaseNode> Nodes = new();
    }
    
    [Serializable]
    public class RuntimeDialogeNode : RuntimeBaseNode
    {
        public string ActorName;
        public string Message;
        public EmotionType Emotion;
    }

    [Serializable]
    public class RuntimeBaseNode
    {
        public string NodeId;
        public string NextNodeId;
    }
}
