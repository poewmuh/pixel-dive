using _Project.Application.GUI.Components;
using _Project.Application.Settings;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Windows.MainMenu.Tabs
{
    public class AudioSettingsTab : SettingsTabBase
    {
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        [Inject] private ISettingsService _settingsService;

        private bool _isInitialized;

        public override void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _masterVolumeSlider.OnValueChangedAsObservable()
                .Skip(1)
                .Subscribe(value => _settingsService.SetMasterVolume(value))
                .AddTo(this);

            _musicVolumeSlider.OnValueChangedAsObservable()
                .Skip(1)
                .Subscribe(value => _settingsService.SetMusicVolume(value))
                .AddTo(this);

            _sfxVolumeSlider.OnValueChangedAsObservable()
                .Skip(1)
                .Subscribe(value => _settingsService.SetSfxVolume(value))
                .AddTo(this);

            Refresh();
        }

        public override void Refresh()
        {
            var audio = _settingsService.Audio.Value;
            
            _masterVolumeSlider.SetValueWithoutNotify(audio.MasterVolume);
            _musicVolumeSlider.SetValueWithoutNotify(audio.MusicVolume);
            _sfxVolumeSlider.SetValueWithoutNotify(audio.SfxVolume);
        }
    }
}
