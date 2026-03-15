using System;
using _Project.Application.Audio;
using UnityEngine;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.FishPuzzle
{
    public class FishHead : MonoBehaviour
    {
        [SerializeField] private int _index;
        [SerializeField] private Sprite _openMouthSprite;
        [SerializeField] private Sprite _closeMouthSprite;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private AudioSource _audioSource;

        public float MaxAudioLength => _audioSource.clip.length;
        
        public bool IsOpen { get; private set; }

        public void OnEnable()
        {
            SetOpen(0);
            _audioSource.Stop();
            _audioSource.Play();
        }

        public void SetOpen(float audioTime)
        {
            if (!IsOpen) Toggle(audioTime);
        }

        public void Toggle(float audioTime)
        {
            IsOpen = !IsOpen;
            UpdateVisual();

            _audioSource.time = audioTime;
            if (IsOpen) _audioSource.Play();
            else _audioSource.Stop();
        }
        
        private void UpdateVisual()
        {
            _spriteRenderer.sprite = IsOpen ? _openMouthSprite : _closeMouthSprite;
        }
    }
}
