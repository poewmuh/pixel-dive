using Newtonsoft.Json;
using UnityEngine;

namespace _Project.Application.Persistence.Data
{
    [CreateAssetMenu(fileName = nameof(InitialGameSaveData), menuName = "_Project/Data/" + nameof(InitialGameSaveData))]
    public class InitialGameSaveData : ScriptableObject
    {
        [field: SerializeField] public GameplaySaveData GameplaySaveData { get; private set; }
        
        [ContextMenu("Test Serialization")]
        public void TestSerialization()
        {
            Debug.Log(JsonConvert.SerializeObject(GameplaySaveData, Formatting.Indented));
        }
    }
}