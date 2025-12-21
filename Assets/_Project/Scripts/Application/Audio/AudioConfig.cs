using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace _Project.Application.Audio
{
    [CreateAssetMenu(fileName = nameof(AudioConfig), menuName = "_Project/Audio/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        [SerializedDictionary("Element Type", "Description"), SerializeField] private SerializedDictionary<string, AudioClip> _audioClips;

        public AudioClip GetAudioClip(string key)
        {
            return _audioClips[key];
        }
    }
}
