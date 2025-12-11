using UnityEngine;

namespace _Project.Application.Audio
{
    public interface IAudioSystem
    {
        void PlayOneShot(string key);
        void PlayOneShot(string key, Vector3 worldPosition);

        void PlayMusic(string key);
        void StopMusic();
		
        void PlayAmbient(string key);
        void StopAmbient(string key);
        void StopAllAmbient();
    }
}
