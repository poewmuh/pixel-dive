using System;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.Application.Persistence.Data
{
    [Serializable]
    public class GameplaySaveData
    {
        [JsonProperty]
        [field: SerializeField]
        public PlayerSaveData PlayerSaveData { get; private set; }
        
        [JsonProperty]
        [field: SerializeField]
        public LevelSaveData LevelSaveData { get; private set; }
    }
}