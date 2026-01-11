using UnityEditor;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BoxGizmoDrawer2d : MonoBehaviour
    {
        public enum DrawMode
        {
            Always,
            OnSelected,
            Never
        }
        
        [SerializeField] private DrawMode _drawMode = DrawMode.Always;
        [SerializeField] private string _drawText = "";
        [SerializeField] private Color _gizmoColor = new (Color.softGreen.r, Color.softGreen.g, Color.softGreen.b, 0.6f);
        [SerializeField] private Color _textColor = Color.white;
        [SerializeField] private int _fontSize = 12;
        [SerializeField] private Vector3 _textOffset = Vector3.zero;
        
        private BoxCollider2D _boxCollider;
        
        private void OnValidate()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_drawMode is DrawMode.Always)
            {
                DrawColliderGizmo();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_drawMode is DrawMode.OnSelected)
            {
                DrawColliderGizmo();
            }
        }
        
        private void DrawColliderGizmo()
        {
            if (_boxCollider == null)
                _boxCollider = GetComponent<BoxCollider2D>();

            if (_boxCollider == null || _drawMode == DrawMode.Never)
                return;

            var prevColor = Gizmos.color;
            var prevMatrix = Gizmos.matrix;
            Gizmos.color = _gizmoColor;

            var position = transform.position + (Vector3)_boxCollider.offset;
            var rotation = transform.rotation;
            var scale = transform.lossyScale;
            var size = new Vector3(
                _boxCollider.size.x * Mathf.Abs(scale.x),
                _boxCollider.size.y * Mathf.Abs(scale.y),
                0.01f
            );

            Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);

            Gizmos.DrawCube(Vector3.zero, size);

            Gizmos.matrix = prevMatrix;

            DrawText(position + _textOffset, _drawText, _textColor);

            Gizmos.color = prevColor;
        }
        
        private void DrawText(Vector3 worldPosition, string text, Color color)
        {
            var style = new GUIStyle();
            style.normal.textColor = color;
            style.fontSize = _fontSize;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;

            var screenPos = Camera.current.WorldToScreenPoint(worldPosition);
        
            if (screenPos.z > 0)
            {
                Handles.Label(worldPosition, text, style);
            }
        }
        
        #endif
    }
}