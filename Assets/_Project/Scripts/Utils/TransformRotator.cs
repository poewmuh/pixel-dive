using UnityEngine;

namespace Utils._Project.Utils
{
    public class TransformRotator : MonoBehaviour
    {
        [SerializeField] private Vector3 _rotationSpeed = new(0f, 0f, 90f);

        public Vector3 RotationSpeed
        {
            get => _rotationSpeed;
            set => _rotationSpeed = value;
        }

        private void Update()
        {
            transform.Rotate(_rotationSpeed * Time.deltaTime);
        }
    }
}
