using System;
using UnityEngine;

namespace _Project.Application.Audio
{
    public class UnityAudioSystem : MonoBehaviour, IAudioSystem
    {
        void IAudioSystem.PlayOneShot(string key)
        {
            throw new NotImplementedException();
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
