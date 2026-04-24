using System;
using _Project.Application.Audio;
using _Project.Application.Input;
using _Project.Gameplay.TruePuzzle.Cursor;
using VContainer;

namespace _Project.Gameplay.Initializations
{
    public class TruePuzzleInitilizer : LevelInitilizer
    {
        [Inject] private IInputSwitcher _inputSwitcher;
        [Inject] private IAudioSystem _audioSystem;
        public override void Initilize(IContainerBuilder builder) { }

        protected override void Start()
        {
            base.Start();
            _inputSwitcher.SetLevelMode(InputMapMode.Puzzle);
            _audioSystem.StopMusic();
        }
    }
}