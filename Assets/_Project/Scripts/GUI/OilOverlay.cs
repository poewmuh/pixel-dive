using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.GUI
{
    public class OilOverlay : BaseOverlay
    {
        private static readonly int PlayerViewportPos = Shader.PropertyToID("_PlayerViewportPos");
        
        [SerializeField] private Image _oilRenderer;

        [Inject] private ISubscriber<OilOverlayEventMessage> _oilSubscriber;
        [Inject] private Camera _cam;

        private Canvas _canvas;
        private Material _oilMaterial;
        private Transform _playerTransform;
        private bool _isActive;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _oilMaterial = _oilRenderer.material;
            
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning($"{nameof(OilOverlay)} could not find object with Player tag.", this);
            }
            
            _isActive = _canvas.enabled;
            _oilSubscriber.Subscribe(OnOilActiveChange).AddTo(this);
        }

        private void LateUpdate()
        {
            if (!_isActive || _playerTransform == null || _cam == null) return;
            
            UpdatePlayerPos();
        }

        private void UpdatePlayerPos()
        {
            var vp = _cam.WorldToViewportPoint(_playerTransform.position);
            _oilMaterial.SetVector(PlayerViewportPos, new Vector4(vp.x, vp.y, 0f, 0f));
        }

        private void OnOilActiveChange(OilOverlayEventMessage msg)
        {
            _isActive = msg.IsActive;
            _canvas.enabled = _isActive;
        }
    }

    public class OilOverlayEventMessage
    {
        public bool IsActive { get; }

        public OilOverlayEventMessage(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
