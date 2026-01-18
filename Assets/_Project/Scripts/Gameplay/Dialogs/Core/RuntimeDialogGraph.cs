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
}
