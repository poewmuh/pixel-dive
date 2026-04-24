using System;
using _Project.Application.Audio;
using _Project.Application.Localization;
using _Project.Application.Persistence;
using R3;
using UnityEngine;
using Utils._Project.Utils;
using Utils._Project.Utils.Serialization;
using VContainer;
using VContainer.Unity;

namespace _Project.Application.Settings
{
    public class SettingsService : ISettingsService, IInitializable, IDisposable
    {
        private const string SettingsSaveName = "Settings.dat";
        
        [Inject] private IAudioSystem _audioSystem;

        public ReactiveProperty<AudioSettingsData> Audio { get; } = new();
        public ReactiveProperty<VideoSettingsData> Video { get; } = new();
        public ReactiveProperty<GameSettingsData> Game { get; } = new();

        private readonly CompositeDisposable _disposables = new();
        private SettingsData _currentSettings;
        private FileSystem _fileSystem;
        private ISerializeContract _serializeContract;

        void IInitializable.Initialize()
        {
            _fileSystem = new FileSystem();
            _serializeContract = new JSONSerialize();
            Load();
            SubscribeToChanges();
            
            Observable.NextFrame()
                .Subscribe(_ => ApplyAllSettings())
                .AddTo(_disposables);
        }

        private void Load()
        {
            if (!_fileSystem.FileExists(SettingsSaveName))
            {
                _currentSettings = new SettingsData();
                Audio.Value = _currentSettings.Audio;
                Video.Value = _currentSettings.Video;
                Game.Value = _currentSettings.Game;
                return;
            }
            
            var settingsText = _fileSystem.LoadTextFromFile(SettingsSaveName);
            _currentSettings = _serializeContract.Deserialize<SettingsData>(settingsText);
            
            Audio.Value = _currentSettings.Audio;
            Video.Value = _currentSettings.Video;
            Game.Value = _currentSettings.Game;
        }

        private void Save()
        {
            _currentSettings.Audio = Audio.Value;
            _currentSettings.Video = Video.Value;
            _currentSettings.Game = Game.Value;
            
            _fileSystem.SaveToFile(SettingsSaveName, _serializeContract.Serialize(_currentSettings));
        }

        private void SubscribeToChanges()
        {
            Audio.Subscribe(_ => Save()).AddTo(_disposables);
            Video.Subscribe(_ => Save()).AddTo(_disposables);
            Game.Subscribe(_ => Save()).AddTo(_disposables);
        }

        private void ApplyAllSettings()
        {
            ApplyAudioSettings();
            ApplyVideoSettings();
            ApplyGameSettings();
        }

        private void ApplyAudioSettings()
        {
            var audio = Audio.Value;
            _audioSystem.SetMasterVolume(audio.MasterVolume);
            _audioSystem.SetMusicVolume(audio.MusicVolume);
            _audioSystem.SetSfxVolume(audio.SfxVolume);
        }

        private void ApplyVideoSettings()
        {
            var video = Video.Value;
            Screen.SetResolution(video.ResolutionWidth, video.ResolutionHeight, video.WindowMode, new RefreshRate
            {
                numerator = (uint)video.RefreshRate,
                denominator = 1
            });
        }

        private void ApplyGameSettings()
        {
            var game = Game.Value;
            LocalizationHelper.SetupLocalization(game.LanguageIndex);
        }

        void ISettingsService.SetMasterVolume(float value)
        {
            var audio = Audio.Value;
            audio.MasterVolume = Mathf.Clamp01(value);
            Audio.ForceNotify();
            _audioSystem.SetMasterVolume(audio.MasterVolume);
        }

        void ISettingsService.SetMusicVolume(float value)
        {
            var audio = Audio.Value;
            audio.MusicVolume = Mathf.Clamp01(value);
            Audio.ForceNotify();
            _audioSystem.SetMusicVolume(audio.MusicVolume);
        }

        void ISettingsService.SetSfxVolume(float value)
        {
            var audio = Audio.Value;
            audio.SfxVolume = Mathf.Clamp01(value);
            Audio.ForceNotify();
            _audioSystem.SetSfxVolume(audio.SfxVolume);
        }

        void ISettingsService.SetResolution(int width, int height, int refreshRate)
        {
            var video = Video.Value;
            video.ResolutionWidth = width;
            video.ResolutionHeight = height;
            video.RefreshRate = refreshRate;
            Video.ForceNotify();
            ApplyVideoSettings();
        }

        void ISettingsService.SetWindowMode(FullScreenMode mode)
        {
            var video = Video.Value;
            video.WindowMode = mode;
            Video.ForceNotify();
            ApplyVideoSettings();
        }

        void ISettingsService.SetLanguage(int languageIndex)
        {
            var game = Game.Value;
            game.LanguageIndex = languageIndex;
            Game.ForceNotify();
            ApplyGameSettings();
        }

        void IDisposable.Dispose()
        {
            _disposables.Dispose();
        }
    }
}
