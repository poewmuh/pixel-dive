using R3;
using UnityEngine;

namespace _Project.Application.Settings
{
    public interface ISettingsService
    {
        ReactiveProperty<AudioSettingsData> Audio { get; }
        ReactiveProperty<VideoSettingsData> Video { get; }
        ReactiveProperty<GameSettingsData> Game { get; }

        void SetMasterVolume(float value);
        void SetMusicVolume(float value);
        void SetSfxVolume(float value);
        
        void SetResolution(int width, int height, int refreshRate);
        void SetWindowMode(FullScreenMode mode);
        
        void SetLanguage(int languageIndex);
    }
}
