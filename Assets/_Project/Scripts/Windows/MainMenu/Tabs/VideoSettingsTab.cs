using System.Collections.Generic;
using System.Linq;
using _Project.Application.GUI.Components;
using _Project.Application.Settings;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Windows.MainMenu.Tabs
{
    public class VideoSettingsTab : SettingsTabBase
    {
        [SerializeField] private TMP_Dropdown _resolutionDropdown;
        [SerializeField] private TMP_Dropdown _windowModeDropdown;

        [Inject] private ISettingsService _settingsService;

        private List<Resolution> _availableResolutions;
        private bool _isInitialized;

        public override void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            SetupResolutionDropdown();
            SetupWindowModeDropdown();

            _resolutionDropdown.OnValueChangedAsObservable()
                .Skip(1)
                .Subscribe(OnResolutionChanged)
                .AddTo(this);

            _windowModeDropdown.OnValueChangedAsObservable()
                .Skip(1)
                .Subscribe(OnWindowModeChanged)
                .AddTo(this);

            Refresh();
        }

        private void SetupResolutionDropdown()
        {
            _availableResolutions = Screen.resolutions
                .GroupBy(r => new { r.width, r.height })
                .Select(g => g.OrderByDescending(r => r.refreshRateRatio.value).First())
                .OrderByDescending(r => r.width * r.height)
                .ToList();

            _resolutionDropdown.ClearOptions();
            _resolutionDropdown.AddOptions(
                _availableResolutions.Select(r => $"{r.width} x {r.height}").ToList()
            );
        }

        private void SetupWindowModeDropdown()
        {
            _windowModeDropdown.ClearOptions();
            _windowModeDropdown.AddOptions(new List<string>
            {
                "Fullscreen",
                "Borderless Window",
                "Windowed"
            });
        }

        private void OnResolutionChanged(int index)
        {
            if (index < 0 || index >= _availableResolutions.Count) return;

            var resolution = _availableResolutions[index];
            _settingsService.SetResolution(
                resolution.width, 
                resolution.height, 
                (int)resolution.refreshRateRatio.value
            );
        }

        private void OnWindowModeChanged(int index)
        {
            var mode = index switch
            {
                0 => FullScreenMode.ExclusiveFullScreen,
                1 => FullScreenMode.FullScreenWindow,
                2 => FullScreenMode.Windowed,
                _ => FullScreenMode.FullScreenWindow
            };

            _settingsService.SetWindowMode(mode);
        }

        public override void Refresh()
        {
            var video = _settingsService.Video.Value;

            var resolutionIndex = _availableResolutions.FindIndex(r =>
                r.width == video.ResolutionWidth && r.height == video.ResolutionHeight);
            
            if (resolutionIndex >= 0)
            {
                _resolutionDropdown.SetValueWithoutNotify(resolutionIndex);
            }

            var modeIndex = video.WindowMode switch
            {
                FullScreenMode.ExclusiveFullScreen => 0,
                FullScreenMode.FullScreenWindow => 1,
                FullScreenMode.Windowed => 2,
                _ => 1
            };
            _windowModeDropdown.SetValueWithoutNotify(modeIndex);
        }
    }
}
