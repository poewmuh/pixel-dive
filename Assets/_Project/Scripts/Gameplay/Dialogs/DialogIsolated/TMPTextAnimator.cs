using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace _Project.Gameplay.Dialogs
{
    /// <summary>
    /// Animates individual characters in TextMeshPro text.
    /// Attach to the same GameObject as TextMeshProUGUI.
    /// Call AnimateLastCharacter() after adding each new character for typewriter effect.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPTextAnimator : MonoBehaviour
    {
        [Header("Scale Animation")]
        [SerializeField] private float _startScale = 1.5f;
        [SerializeField] private float _endScale = 1f;
        [SerializeField] private float _animationDuration = 0.15f;
        [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        [Header("Optional Effects")]
        [SerializeField] private bool _enableBounce = true;
        [SerializeField] private float _bounceOvershoot = 1.1f;
        
        [Header("Fade Animation")]
        [SerializeField] private bool _enableFade = true;
        [SerializeField] private float _startAlpha = 0f;

        private TextMeshProUGUI _textComponent;
        private readonly Dictionary<int, CharacterAnimation> _activeAnimations = new();
        private readonly List<int> _completedAnimations = new();
        private readonly List<Vector3[]> _baseVertices = new();
        private readonly List<Color32[]> _baseColors = new();
        
        private struct CharacterAnimation
        {
            public float StartTime;
        }

        private void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
            
            if (_enableBounce)
            {
                _scaleCurve = CreateBounceCurve();
            }
        }

        private void Update()
        {
            if (_activeAnimations.Count == 0) return;
            
            var textInfo = _textComponent.textInfo;
            RestoreMeshState(textInfo);
            _completedAnimations.Clear();
            
            foreach (var kvp in _activeAnimations)
            {
                var charIndex = kvp.Key;
                var animation = kvp.Value;
                
                if (charIndex >= textInfo.characterCount) continue;
                
                var charInfo = textInfo.characterInfo[charIndex];
                if (!charInfo.isVisible) continue;
                
                var elapsed = Time.time - animation.StartTime;
                var t = Mathf.Clamp01(elapsed / _animationDuration);
                var curveValue = _scaleCurve.Evaluate(t);
                
                var scale = Mathf.LerpUnclamped(_startScale, _endScale, curveValue);
                var alpha = _enableFade ? Mathf.Lerp(_startAlpha, 1f, t) : 1f;
                
                ApplyCharacterTransform(textInfo, charIndex, scale, alpha);
                
                if (t >= 1f)
                {
                    _completedAnimations.Add(charIndex);
                }
            }
            
            foreach (var index in _completedAnimations)
            {
                _activeAnimations.Remove(index);
            }
            
            UpdateMeshes(textInfo);
        }

        /// <summary>
        /// Call this after adding a new character to animate it.
        /// </summary>
        public void AnimateLastCharacter()
        {
            _textComponent.ForceMeshUpdate();
            var textInfo = _textComponent.textInfo;
            
            var lastVisibleIndex = -1;
            for (var i = textInfo.characterCount - 1; i >= 0; i--)
            {
                if (textInfo.characterInfo[i].isVisible)
                {
                    lastVisibleIndex = i;
                    break;
                }
            }
            
            if (lastVisibleIndex >= 0)
            {
                AnimateCharacter(lastVisibleIndex);
            }
        }

        /// <summary>
        /// Animate a specific character by index.
        /// </summary>
        public void AnimateCharacter(int charIndex)
        {
            _textComponent.ForceMeshUpdate();
            var textInfo = _textComponent.textInfo;
            CaptureMeshState(textInfo);
            
            AddAnimation(textInfo, charIndex);
        }

        /// <summary>
        /// Animate all currently visible characters (for reveal-all effect).
        /// </summary>
        public void AnimateAllCharacters()
        {
            _textComponent.ForceMeshUpdate();
            var textInfo = _textComponent.textInfo;
            CaptureMeshState(textInfo);
            
            for (var i = 0; i < textInfo.characterCount; i++)
            {
                AddAnimation(textInfo, i);
            }
        }

        /// <summary>
        /// Clear all active animations and reset text to normal.
        /// </summary>
        public void ResetAnimations()
        {
            _activeAnimations.Clear();
            _baseVertices.Clear();
            _baseColors.Clear();
            _textComponent.ForceMeshUpdate();
        }

        private void ApplyCharacterTransform(TMP_TextInfo textInfo, int charIndex, float scale, float alpha)
        {
            var charInfo = textInfo.characterInfo[charIndex];
            var meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];
            var vertexIndex = charInfo.vertexIndex;
            
            // Calculate character center
            var vertices = meshInfo.vertices;
            var center = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f;
            
            // Apply scale from center
            for (var i = 0; i < 4; i++)
            {
                var offset = vertices[vertexIndex + i] - center;
                vertices[vertexIndex + i] = center + offset * scale;
            }
            
            // Apply alpha
            if (_enableFade)
            {
                var colors = meshInfo.colors32;
                var byteAlpha = (byte)(alpha * 255);
                for (var i = 0; i < 4; i++)
                {
                    var color = colors[vertexIndex + i];
                    color.a = byteAlpha;
                    colors[vertexIndex + i] = color;
                }
            }
        }

        private void AddAnimation(TMP_TextInfo textInfo, int charIndex)
        {
            if (charIndex < 0 || charIndex >= textInfo.characterCount) return;
            if (!textInfo.characterInfo[charIndex].isVisible) return;
            
            _activeAnimations[charIndex] = new CharacterAnimation
            {
                StartTime = Time.time
            };
        }

        private void CaptureMeshState(TMP_TextInfo textInfo)
        {
            EnsureSnapshotCapacity(textInfo.meshInfo.Length);

            for (var i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                _baseVertices[i] = CopyArray(meshInfo.vertices, _baseVertices[i]);
                
                if (_enableFade)
                {
                    _baseColors[i] = CopyArray(meshInfo.colors32, _baseColors[i]);
                }
            }
        }

        private void RestoreMeshState(TMP_TextInfo textInfo)
        {
            if (_baseVertices.Count < textInfo.meshInfo.Length) return;
            
            for (var i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                var vertices = _baseVertices[i];
                if (vertices == null || vertices.Length != meshInfo.vertices.Length) return;
                
                vertices.CopyTo(meshInfo.vertices, 0);
                
                if (!_enableFade) continue;
                
                var colors = _baseColors[i];
                if (colors == null || colors.Length != meshInfo.colors32.Length) return;
                
                colors.CopyTo(meshInfo.colors32, 0);
            }
        }

        private void EnsureSnapshotCapacity(int meshCount)
        {
            while (_baseVertices.Count < meshCount)
            {
                _baseVertices.Add(null);
            }
            
            while (_baseColors.Count < meshCount)
            {
                _baseColors.Add(null);
            }
        }

        private static T[] CopyArray<T>(T[] source, T[] target)
        {
            if (target == null || target.Length != source.Length)
            {
                target = new T[source.Length];
            }
            
            source.CopyTo(target, 0);
            return target;
        }

        private void UpdateMeshes(TMP_TextInfo textInfo)
        {
            for (var i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                meshInfo.mesh.colors32 = meshInfo.colors32;
                _textComponent.UpdateGeometry(meshInfo.mesh, i);
            }
        }

        private AnimationCurve CreateBounceCurve()
        {
            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 2f));
            curve.AddKey(new Keyframe(0.5f, _bounceOvershoot, 0f, 0f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }
    }
}
