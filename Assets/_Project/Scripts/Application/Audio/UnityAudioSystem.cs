using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Application.Audio
{
    public class UnityAudioSystem : MonoBehaviour, IAudioSystem
    {
        [SerializeField] private AudioConfig _audioConfig;
        [SerializeField] private AudioSource _effectSource;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioMixer _mixer;
        
        void IAudioSystem.PlayOneShot(string key)
        {
            _effectSource.Stop();
            var audioClip = _audioConfig.GetAudioClip(key);
            _effectSource.clip = audioClip;
            _effectSource.Play();
        }

        void IAudioSystem.PlayOneShot(string key, Vector3 worldPosition)
        {
            throw new NotImplementedException();
        }

        void IAudioSystem.PlayMusic(string key)
        {
            ((IAudioSystem)this).StopMusic();
            var audioClip = _audioConfig.GetAudioClip(key);
            _musicSource.clip = audioClip;
            _musicSource.Play();
        }

        void IAudioSystem.StopMusic()
        {
            _musicSource.Stop();
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
