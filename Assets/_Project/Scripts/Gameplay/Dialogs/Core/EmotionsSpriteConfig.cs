using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace _Project.Gameplay.Dialogs.Core
{
    [CreateAssetMenu(fileName = nameof(EmotionsSpriteConfig), menuName = "_Project/Gameplay/" + nameof(EmotionsSpriteConfig))]
    public class EmotionsSpriteConfig : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<EmotionType, Sprite> _emotionSprites = new();
        
        public Sprite GetSpriteForEmotion(EmotionType emotion)
        {
            if (_emotionSprites.TryGetValue(emotion, out var sprite))
            {
                return sprite;
            }

            Debug.LogWarning($"[EmotionsSpriteConfig] No sprite found for emotion {emotion}.");
            return null;
        }
    }
}
