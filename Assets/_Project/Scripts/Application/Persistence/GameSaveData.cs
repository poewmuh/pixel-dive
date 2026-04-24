using System;
using _Project.Application.Persistence.Data;
using Newtonsoft.Json;

namespace _Project.Application.Persistence
{
    [Serializable]
    public class GameSaveData
    {
        [JsonProperty] public GameplaySaveData GameplaySaveData { get; set; }
    }
}