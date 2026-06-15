using _Project.Application.Persistence.Data;
using VContainer;

namespace _Project.Application.Persistence
{
    public class GameSaveMediator : IGameSaveMediator
    {
        [Inject] private IGameSaveSystem _gameSaveSystem;

        GameSaveData IGameSaveMediator.GameSaveData => _gameSaveSystem.CurrentSaveData;

        PlayerSaveData IGameSaveMediator.GetPlayerSaveData()
        {
            return _gameSaveSystem.CurrentSaveData.GameplaySaveData.PlayerSaveData;
        }

        LevelSaveData IGameSaveMediator.GetLevelSaveData()
        {
            return _gameSaveSystem.CurrentSaveData.GameplaySaveData.LevelSaveData;
        }

        void IGameSaveMediator.Save()
        {
            _gameSaveSystem.Save();
        }
    }
}
