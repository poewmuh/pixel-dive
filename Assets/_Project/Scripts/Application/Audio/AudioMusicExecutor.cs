using UnityEngine;
using VContainer;

namespace _Project.Application.Audio
{
    public class AudioMusicExecutor : MonoBehaviour
    {
        [SerializeField] private bool _executeOnStart;
        [SerializeField] private string _key;
        
        [Inject] private IAudioSystem _audioSystem;

        private void Start()
        {
            if (_executeOnStart)
            {
                ExecuteAudioFile();
            }
        }

        private void ExecuteAudioFile()
        {
            _audioSystem.PlayMusic(_key);
        }
    }
}