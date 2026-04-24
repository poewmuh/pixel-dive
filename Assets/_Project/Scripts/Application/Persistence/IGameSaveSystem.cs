namespace _Project.Application.Persistence
{
    public interface IGameSaveSystem
    {
        GameSaveData CurrentSaveData { get; }
        SavesMetaData MetaData { get; }

        GameSaveData CreateNewSave();
        GameSaveData LoadSave(SaveReferenceData saveReference);
        GameSaveData CreateNewOrLoad(SaveReferenceData saveReference);
        void Save();
    }
}