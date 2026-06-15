using _Project.Application.Persistence.Data;
using _Project.Application.Scene;
using VContainer;

namespace _Project.Application.Persistence
{
    public class GameSaveMediator : IGameSaveMediator
    {
        [Inject] private IGameSaveSystem _gameSaveSystem;

        PlayerSaveData IGameSaveMediator.GetPlayerSaveData()
        {
            return _gameSaveSystem.CurrentSaveData.GameplaySaveData.PlayerSaveData;
        }

        SceneType IGameSaveMediator.GetSavedScene()
        {
            return GetLevelSaveData().SceneType;
        }

        void IGameSaveMediator.SaveScene(SceneType sceneType)
        {
            GetLevelSaveData().SetSceneType(sceneType);
            _gameSaveSystem.Save();
        }

        void IGameSaveMediator.Save()
        {
            _gameSaveSystem.Save();
        }

        private LevelSaveData GetLevelSaveData()
        {
            return _gameSaveSystem.CurrentSaveData.GameplaySaveData.LevelSaveData;
        }
    }
}
