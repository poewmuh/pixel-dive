using _Project.Application.GUI.Components;
using _Project.Application.GUI.WindowsSystem;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Windows.MainMenu
{
    public class SettingsWindow : WindowBase
    {
        [Header("Tabs")]
        [SerializeField] private TabGroup _tabGroup;
        [SerializeField] private SettingsTabBase[] _settingsTabs;
        
        [Header("Buttons")]
        [SerializeField] private Button _backButton;

        [Inject] private IObjectResolver _objectResolver;

        private void Awake()
        {
            foreach (var tab in _settingsTabs)
            {
                _objectResolver.Inject(tab);
            }
        }

        private void Start()
        {
            foreach (var tab in _settingsTabs)
            {
                tab.Initialize();
            }

            _tabGroup.OnTabChanged
                .Subscribe(OnTabChanged)
                .AddTo(this);

            _backButton.OnClickAsObservable()
                .Subscribe(_ => OnBackClicked())
                .AddTo(this);
        }

        private void OnTabChanged(int tabIndex)
        {
            if (tabIndex >= 0 && tabIndex < _settingsTabs.Length)
            {
                _settingsTabs[tabIndex].Refresh();
            }
        }

        private void OnBackClicked()
        {
            WindowSystem.Close(this);
            WindowSystem.SetHidden<MainMenuWindow>(false);
        }
    }
}
