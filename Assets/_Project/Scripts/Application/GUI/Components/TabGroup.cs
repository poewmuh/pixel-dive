using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Application.GUI.Components
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private Button[] _tabButtons;
        [SerializeField] private GameObject[] _tabContents;
        [SerializeField] private Color _activeTabColor = Color.white;
        [SerializeField] private Color _inactiveTabColor = Color.gray;

        private readonly Subject<int> _onTabChanged = new();
        public Observable<int> OnTabChanged => _onTabChanged;
        
        public int CurrentTabIndex { get; private set; }

        private void Awake()
        {
            if (_tabButtons.Length != _tabContents.Length)
            {
                Debug.LogError("[TabGroup] Buttons and Contents count mismatch!");
                return;
            }

            for (var i = 0; i < _tabButtons.Length; i++)
            {
                var index = i;
                _tabButtons[i].OnClickAsObservable()
                    .Subscribe(_ => SelectTab(index))
                    .AddTo(this);
            }
        }

        private void Start()
        {
            SelectTab(0);
        }

        public void SelectTab(int index)
        {
            if (index < 0 || index >= _tabContents.Length) return;

            CurrentTabIndex = index;

            for (var i = 0; i < _tabContents.Length; i++)
            {
                var isActive = i == index;
                _tabContents[i].SetActive(isActive);
                
                var buttonImage = _tabButtons[i].GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = isActive ? _activeTabColor : _inactiveTabColor;
                }
            }

            _onTabChanged.OnNext(index);
        }

        private void OnDestroy()
        {
            _onTabChanged.Dispose();
        }
    }
}
