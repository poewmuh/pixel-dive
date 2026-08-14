using DG.Tweening;
using UnityEngine;

namespace _Project.Gameplay.TruePuzzle.Vfx
{
    /// <summary>
    /// Sprite based dirt burst. Spawned as a standalone instance right before the picked up
    /// object disappears, plays once and destroys itself.
    /// </summary>
    public sealed class PickupVfx : MonoBehaviour
    {
        private const float DestroyPadding = 0.1f;

        [Header("Pieces")]
        [SerializeField] private Sprite[] _pieceSprites;
        [SerializeField] private int _piecesCount = 8;
        [SerializeField] private Vector2 _pieceScaleRange = new(0.15f, 0.35f);
        [SerializeField] private Color _tint = new(0.36f, 0.31f, 0.22f, 1f);

        [Header("Movement")]
        [SerializeField] private Vector2 _spreadRadiusRange = new(0.3f, 0.8f);
        [SerializeField] private float _riseHeight = 0.5f;
        [SerializeField] private float _fallDepth = 0.6f;
        [SerializeField] private Vector2 _durationRange = new(0.4f, 0.8f);
        [SerializeField] private float _spinDegrees = 240f;

        [Header("Rendering")]
        [SerializeField] private string _sortingLayer = "Default";
        [SerializeField] private int _sortingOrder = 100;

        private Sequence _sequence;

        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        /// <param name="source">
        /// Renderer of the object that was just taken. Its sprite is used when no
        /// <see cref="_pieceSprites"/> are assigned, and its world scale keeps the pieces
        /// proportional to the object they came from.
        /// </param>
        public void Play(SpriteRenderer source = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            var fallbackSprite = source ? source.sprite : null;
            var sourceScale = source ? source.transform.lossyScale : Vector3.one;
            var longestDuration = 0f;
            var origin = transform.position;

            for (var i = 0; i < _piecesCount; i++)
            {
                var sprite = PickSprite(fallbackSprite);
                if (!sprite) continue;

                var duration = Random.Range(_durationRange.x, _durationRange.y);
                longestDuration = Mathf.Max(longestDuration, duration);

                AnimatePiece(CreatePiece(sprite, sourceScale), origin, duration);
            }

            if (longestDuration <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            Destroy(gameObject, longestDuration + DestroyPadding);
        }

        private Sprite PickSprite(Sprite fallbackSprite)
        {
            if (_pieceSprites == null || _pieceSprites.Length == 0) return fallbackSprite;

            var sprite = _pieceSprites[Random.Range(0, _pieceSprites.Length)];
            return sprite ? sprite : fallbackSprite;
        }

        private SpriteRenderer CreatePiece(Sprite sprite, Vector3 sourceScale)
        {
            var pieceObject = new GameObject("PickupVfxPiece");
            var pieceTransform = pieceObject.transform;
            pieceTransform.SetParent(transform, false);
            pieceTransform.localPosition = Vector3.zero;
            pieceTransform.localScale = sourceScale * Random.Range(_pieceScaleRange.x, _pieceScaleRange.y);

            var pieceRenderer = pieceObject.AddComponent<SpriteRenderer>();
            pieceRenderer.sprite = sprite;
            pieceRenderer.color = _tint;
            pieceRenderer.sortingLayerName = _sortingLayer;
            pieceRenderer.sortingOrder = _sortingOrder;

            return pieceRenderer;
        }

        private void AnimatePiece(SpriteRenderer piece, Vector3 origin, float duration)
        {
            var pieceTransform = piece.transform;
            var direction = Random.insideUnitCircle.normalized;
            var distance = Random.Range(_spreadRadiusRange.x, _spreadRadiusRange.y);

            var peakPosition = origin
                               + (Vector3)(direction * (distance * 0.6f))
                               + Vector3.up * _riseHeight;
            var endPosition = origin
                              + (Vector3)(direction * distance)
                              + Vector3.down * _fallDepth;

            var movement = DOTween.Sequence()
                .Append(pieceTransform.DOMove(peakPosition, duration * 0.4f).SetEase(Ease.OutQuad))
                .Append(pieceTransform.DOMove(endPosition, duration * 0.6f).SetEase(Ease.InQuad));

            _sequence.Join(movement);
            _sequence.Join(pieceTransform
                .DORotate(new Vector3(0f, 0f, Random.Range(-_spinDegrees, _spinDegrees)), duration,
                    RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad));
            _sequence.Join(pieceTransform
                .DOScale(pieceTransform.localScale * 0.35f, duration)
                .SetEase(Ease.InQuad));
            _sequence.Join(piece.DOFade(0f, duration).SetEase(Ease.InQuad));
        }
    }
}
