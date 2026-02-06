using System;
using UnityEngine;

namespace _Project.Application.Settings
{
    [Serializable]
    public class SettingsData
    {
        public AudioSettingsData Audio = new();
        public VideoSettingsData Video = new();
        public GameSettingsData Game = new();
    }

    [Serializable]
    public class AudioSettingsData
    {
        [Range(0f, 1f)] public float MasterVolume = .8f;
        [Range(0f, 1f)] public float MusicVolume = .4f;
        [Range(0f, 1f)] public float SfxVolume = 1f;
    }

    [Serializable]
    public class VideoSettingsData
    {
        public int ResolutionWidth = 1920;
        public int ResolutionHeight = 1080;
        public int RefreshRate = 60;
        public FullScreenMode WindowMode = FullScreenMode.FullScreenWindow;
    }

    [Serializable]
    public class GameSettingsData
    {
        public int LanguageIndex = 0;
    }
}
