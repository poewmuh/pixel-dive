using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.GUI
{
    public class OilOverlay : BaseOverlay
    {
        private static readonly int playerViewportPos = Shader.PropertyToID("_PlayerViewportPos");
        
        [SerializeField] private Image _oilRenderer;

        [Inject] private ISubscriber<OilOverlayEventMessage> _oilSubscriber;
        [Inject] private Camera _cam;

        private Canvas _canvas;
        private Transform _playerTransform;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            _oilSubscriber.Subscribe(OnOilActiveChange).AddTo(this);
        }

        private void LateUpdate()
        {
            UpdatePlayerPos();
        }

        private void UpdatePlayerPos()
        {
            var vp = _cam.WorldToViewportPoint(_playerTransform.position);
            _oilRenderer.material.SetVector(playerViewportPos, new Vector4(vp.x, vp.y, 0f, 0f));
        }

        private void OnOilActiveChange(OilOverlayEventMessage msg)
        {
            _canvas.enabled = msg.IsActive;
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