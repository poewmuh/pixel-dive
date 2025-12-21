using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Project.Application.Audio
{
    public class UnityAudioSystem : MonoBehaviour, IAudioSystem
    {
        [SerializeField] private AudioConfig _audioConfig;
        [SerializeField] private AudioSource _audioSource;
        
        void IAudioSystem.PlayOneShot(string key)
        {
            _audioSource.Stop();
            var audioClip = _audioConfig.GetAudioClip(key);
            _audioSource.clip = audioClip;
            _audioSource.Play();
            Stop().Forget();
        }

        private async UniTaskVoid Stop()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(.09));
            _audioSource.Stop();
        }

        void IAudioSystem.PlayOneShot(string key, Vector3 worldPosition)
        {
            throw new NotImplementedException();
        }

        void IAudioSystem.PlayMusic(string key)
        {
            throw new NotImplementedException();
        }

        void IAudioSystem.StopMusic()
        {
            throw new NotImplementedException();
        }

        void IAudioSystem.PlayAmbient(string key)
        {
            throw new NotImplementedException();
        }

        void IAudioSystem.StopAmbient(string key)
        {
            throw new NotImplementedException();
        }

        void IAudioSystem.StopAllAmbient()
        {
            throw new NotImplementedException();
        }
    }
}
