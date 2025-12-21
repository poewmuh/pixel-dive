using UnityEngine;

namespace _Project.Gameplay.Platforming.AI
{
    public class SimpleFishAI : MonoBehaviour
    {
        [SerializeField] private float _patrolDistance = 5f;
        [SerializeField] private float _patrolSpeed = 3f;
        [SerializeField] private AnimationCurve _patrolCurve;
        
        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private float _timer;

        private void Start()
        {
            _startPosition = transform.position;
            _endPosition = transform.position + Vector3.left * _patrolDistance;
            _timer = 0f;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            var speed = _patrolSpeed * Time.deltaTime * _patrolCurve.Evaluate(_timer);
            transform.position = Vector3.Lerp(transform.position, _endPosition, speed);

            if ((transform.position - _endPosition).sqrMagnitude < .2)
            {
                var transformLocalScale = transform.localScale;
                transformLocalScale.x *= -1;
                transform.localScale = transformLocalScale;
                _timer = 0;
                (_startPosition, _endPosition) = (_endPosition, _startPosition);
            }
        }
    }
}