using _Project.Application.Persistence.Data;

namespace _Project.Application.Persistence
{
    public interface IGameSaveMediator
    {
        GameSaveData GameSaveData { get; }
        
        PlayerSaveData GetPlayerSaveData();
        LevelSaveData GetLevelSaveData();

        void Save();
    }
}