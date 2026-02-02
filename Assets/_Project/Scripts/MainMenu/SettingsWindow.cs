using System;
using _Project.Application.GUI.WindowsSystem;
using _Project.Application.Localization;
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
            _languageIndex = PlayerPrefs.GetInt("LocalizationIndex", -1);
            _languageText.text = LocalizationHelper.GetLanguageByIndex(_languageIndex);
            
            _languageButton.OnClickAsObservable().Subscribe(OnLanguageClick).AddTo(this);
            _backButton.OnClickAsObservable().Subscribe(OnBackClick).AddTo(this);
        }

        private void OnLanguageClick(Unit _)
        {
            _languageIndex = GetNewLanguageIndex();
            LocalizationHelper.SetupLocalization(_languageIndex);
            _languageText.text = LocalizationHelper.GetLanguageByIndex(_languageIndex);
        }

        private int GetNewLanguageIndex()
        {
            if (_languageIndex + 1 >= LocalizationHelper.GetMaxLocales())
            {
                return 0;
            }

            return _languageIndex + 1;
        }

        private void OnBackClick(Unit _)
        {
            WindowSystem.Close(this);
            WindowSystem.SetHidden<MainMenuWindow>(false);
        }
    }
}
