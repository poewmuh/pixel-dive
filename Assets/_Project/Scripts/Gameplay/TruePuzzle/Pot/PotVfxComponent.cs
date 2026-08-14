using DG.Tweening;
using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Pot
{
    public class PotVfxComponent : MonoBehaviour
    {
        [Header("Glow")]
        [SerializeField] private Transform _light;
        [SerializeField] private float _maxLightScale = 1f;
        [SerializeField] private float _growDuration = 0.35f;
        [SerializeField] private float _pulseScale = 0.15f;
        [SerializeField] private float _pulseDuration = 0.2f;
        [SerializeField] private int _pulseCount = 2;
        [SerializeField] private float _fadeOutDuration = 0.5f;

        [Header("Complete Flash")]
        [SerializeField] private SpriteRenderer[] _potSprites;
        [SerializeField] private Color _flashColor = new(1f, 0.96f, 0.65f, 1f);
        [SerializeField] private float _flashDuration = 0.15f;
        [SerializeField] private int _flashCount = 3;

        private Color[] _defaultColors;
        private Sequence _glowSequence;
        private Sequence _flashSequence;

        private void Awake()
        {
            CacheDefaultColors();

            if (_light)
            {
                _light.localScale = Vector3.zero;
            }
        }

        private void OnDestroy()
        {
            _glowSequence?.Kill();
            _flashSequence?.Kill();
        }

        /// <summary>Grows the glow proportionally to how full the pot is and pulses it.</summary>
        public void PlayIngredientAdded(int filledCount, int requiredCount)
        {
            var progress = requiredCount <= 0
                ? 1f
                : Mathf.Clamp01((float)filledCount / requiredCount);

            PlayGlow(_maxLightScale * progress, false);
        }

        /// <summary>Pushes the glow to its maximum, blinks the pot sprites and fades everything back.</summary>
        public void PlayCombined()
        {
            PlayGlow(_maxLightScale, true);
            PlaySpritesFlash();
        }

        /// <summary>Stops every running effect and restores the default look immediately.</summary>
        public void ResetVfx()
        {
            _glowSequence?.Kill();
            _flashSequence?.Kill();

            if (_light)
            {
                _light.localScale = Vector3.zero;
            }

            RestoreDefaultColors();
        }

        private void PlayGlow(float targetScale, bool fadeOutAtEnd)
        {
            if (!_light) return;

            _glowSequence?.Kill();

            var targetVector = Vector3.one * targetScale;
            _glowSequence = DOTween.Sequence();
            _glowSequence.Append(_light.DOScale(targetVector, _growDuration).SetEase(Ease.OutBack));

            if (_pulseCount > 0 && _pulseScale > 0f)
            {
                _glowSequence.Append(_light
                    .DOScale(targetVector * (1f + _pulseScale), _pulseDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(_pulseCount * 2, LoopType.Yoyo));
            }

            if (fadeOutAtEnd)
            {
                _glowSequence.Append(_light.DOScale(Vector3.zero, _fadeOutDuration).SetEase(Ease.InQuad));
            }
        }

        private void PlaySpritesFlash()
        {
            if (_potSprites == null || _potSprites.Length == 0) return;

            _flashSequence?.Kill();
            RestoreDefaultColors();

            _flashSequence = DOTween.Sequence();

            foreach (var potSprite in _potSprites)
            {
                if (!potSprite) continue;

                _flashSequence.Join(potSprite
                    .DOColor(_flashColor, _flashDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(_flashCount * 2, LoopType.Yoyo));
            }

            _flashSequence.OnKill(RestoreDefaultColors);
        }

        private void CacheDefaultColors()
        {
            if (_potSprites == null)
            {
                _defaultColors = System.Array.Empty<Color>();
                return;
            }

            _defaultColors = new Color[_potSprites.Length];
            for (var i = 0; i < _potSprites.Length; i++)
            {
                _defaultColors[i] = _potSprites[i] ? _potSprites[i].color : Color.white;
            }
        }

        private void RestoreDefaultColors()
        {
            if (_potSprites == null || _defaultColors == null) return;

            for (var i = 0; i < _potSprites.Length && i < _defaultColors.Length; i++)
            {
                if (_potSprites[i])
                {
                    _potSprites[i].color = _defaultColors[i];
                }
            }
        }
    }
}
