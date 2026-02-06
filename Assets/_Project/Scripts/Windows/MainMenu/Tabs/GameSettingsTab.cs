using _Project.Application.GUI.Components;
using _Project.Application.Localization;
using _Project.Application.Settings;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Windows.MainMenu.Tabs
{
    public class GameSettingsTab : SettingsTabBase
    {
        [SerializeField] private Button _languageButton;
        [SerializeField] private TextMeshProUGUI _languageText;

        [Inject] private ISettingsService _settingsService;

        private bool _isInitialized;

        public override void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _languageButton.OnClickAsObservable()
                .Subscribe(_ => OnLanguageClicked())
                .AddTo(this);

            Refresh();
        }

        private void OnLanguageClicked()
        {
            var currentIndex = _settingsService.Game.Value.LanguageIndex;
            var newIndex = (currentIndex + 1) % LocalizationHelper.GetMaxLocales();
            
            _settingsService.SetLanguage(newIndex);
            UpdateLanguageDisplay();
        }

        private void UpdateLanguageDisplay()
        {
            var languageIndex = _settingsService.Game.Value.LanguageIndex;
            _languageText.text = LocalizationHelper.GetLanguageByIndex(languageIndex);
        }

        public override void Refresh()
        {
            UpdateLanguageDisplay();
        }
    }
}
