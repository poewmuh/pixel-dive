using System;
using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Application.Audio
{
    public class UnityAudioSystem : MonoBehaviour, IAudioSystem
    {
        private const string MasterVolumeParam = "MasterVolume";
        private const string MusicVolumeParam = "MusicVolume";
        private const string SfxVolumeParam = "SFXVolume";
        private const string LowPassParam = "Lowpass";

        [SerializeField] private AudioConfig _audioConfig;
        [SerializeField] private AudioSource _effectSource;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioMixer _mixer;

        private float _lowpassOrigin;

        private void Awake()
        {
            _mixer.GetFloat(LowPassParam, out _lowpassOrigin);
        }

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

        void IAudioSystem.SetMasterVolume(float volume)
        {
            SetMixerVolume(MasterVolumeParam, volume);
        }

        void IAudioSystem.SetMusicVolume(float volume)
        {
            SetMixerVolume(MusicVolumeParam, volume);
        }

        void IAudioSystem.SetSfxVolume(float volume)
        {
            SetMixerVolume(SfxVolumeParam, volume);
        }
        
        void IAudioSystem.SetLowpassValue(float lowpass)
        {
            _mixer.SetFloat(LowPassParam, _lowpassOrigin * lowpass);
        }

        private void SetMixerVolume(string parameterName, float normalizedVolume)
        {
            var volumeDb = normalizedVolume > 0.0001f 
                ? Mathf.Log10(normalizedVolume) * 20f 
                : -80f;
            
            _mixer.SetFloat(parameterName, volumeDb);
        }
    }
}
