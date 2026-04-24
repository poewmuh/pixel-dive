using System;
using _Project.Application.Scene;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.Application.Persistence.Data
{
    [Serializable]
    public class LevelSaveData
    {
        [JsonProperty]
        [field: SerializeField]
        public SceneType SceneType { get; private set; } = SceneType.PuzzlePlatforming;

        public void SetSceneType(SceneType sceneType)
        {
            SceneType = sceneType;
        }
    }
}