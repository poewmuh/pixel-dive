using _Project.Application.Persistence.Data;
using _Project.Application.Scene;

namespace _Project.Application.Persistence
{
    public interface IGameSaveMediator
    {
        PlayerSaveData GetPlayerSaveData();
        SceneType GetSavedScene();

        void SaveScene(SceneType sceneType);
        void Save();
    }
}
