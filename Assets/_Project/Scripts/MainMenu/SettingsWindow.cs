using System;
using _Project.Application.GUI.WindowsSystem;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace _Project.MainMenu
{
    public class SettingsWindow : WindowBase
    {
        [SerializeField] private Button _languageButton;
        [SerializeField] private TextMeshProUGUI _languageText;
        
        [SerializeField] private Button _backButton;
        
        private int _languageIndex = 0;

        private void Awake()
        {
            _languageButton.OnClickAsObservable().Subscribe(OnLanguageClick).AddTo(this);
            _backButton.OnClickAsObservable().Subscribe(OnBackClick).AddTo(this);
        }

        private void OnLanguageClick(Unit _)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_languageIndex];
            if (_languageIndex == 0)
            {
                _languageText.text = "Russian";
                _languageIndex = 1;
            }
            else
            {
                _languageText.text = "English";
                _languageIndex = 0;
            }
        }

        private void OnBackClick(Unit _)
        {
            WindowSystem.Close(this);
            WindowSystem.SetHidden<MainMenuWindow>(false);
        }
    }
}
