using _Project.Application.Input;
using UnityEngine;
using VContainer;

namespace _Project.Gameplay.Initializations
{
    public class PuzzlePlatformingInitilizer : LevelInitilizer
    {
        [Inject] private IInputSwitcher _inputSwitcher;
        
        public override void Initilize(IContainerBuilder builder)
        {
            
        }

        protected override void Start()
        {
            base.Start();
            _inputSwitcher.SetLevelMode(InputMapMode.Platforming);
        }
    }
}