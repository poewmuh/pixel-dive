using System;
using _Project.Application.Persistence.Data;
using R3;
using UnityEngine;
using Utils._Project.Utils;
using Utils._Project.Utils.Serialization;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace _Project.Application.Persistence
{
    public class GameSaveSystem : IGameSaveSystem, IInitializable
    {
        private const string MetadataFileName = "SaveMetadata.dat";
        
        [Inject] private InitialGameSaveData _initialGameSaveData;
        
        private readonly ReactiveProperty<SaveReferenceData> _currentSaveReference = new ();
        private GameSaveData _currentGameSaveData;
        
        private FileSystem _fileSystem;
        private SavesMetaData _metadata;
        private ISerializeContract _serializeContract;

        GameSaveData IGameSaveSystem.CurrentSaveData => EnsureCurrentSaveData();
        SavesMetaData IGameSaveSystem.MetaData => _metadata;
        
        void IInitializable.Initialize()
        {
            var isDevelopment = UnityEngine.Application.isEditor || Debug.isDebugBuild;
            var subFolder = isDevelopment ? "Development" : "Production";
            _fileSystem = new FileSystem(subFolder);
            _serializeContract = new JSONSerialize();

            if (_fileSystem.FileExists(MetadataFileName))
            {
                var loadedMetaData = _fileSystem.LoadTextFromFile(MetadataFileName);
                _metadata = _serializeContract.Deserialize<SavesMetaData>(loadedMetaData);
            }

            _metadata ??= new SavesMetaData();
            _metadata.Saves ??= new();
        }
        
        GameSaveData IGameSaveSystem.CreateNewSave()
        {
            return CreateNewSave(CreateSaveReference());
        }

        GameSaveData IGameSaveSystem.LoadSave(SaveReferenceData saveReference)
        {
            return Load(saveReference);
        }

        GameSaveData IGameSaveSystem.CreateNewOrLoad(SaveReferenceData saveReference)
        {
            return saveReference != null && _fileSystem.FileExists(saveReference.Filename)
                ? Load(saveReference)
                : CreateNewSave(CreateSaveReference());
        }

        void IGameSaveSystem.Save()
        {
            EnsureCurrentSaveData();
            _currentSaveReference.Value.LastSaveDateTime = DateTime.Now;
            _fileSystem.SaveToFile(MetadataFileName, _serializeContract.Serialize(_metadata));
            _fileSystem.SaveToFile(_currentSaveReference.CurrentValue.Filename, _serializeContract.Serialize(_currentGameSaveData));
            
            Debug.Log($"[SaveLoadSystem] Saving >> {_currentSaveReference.Value.Filename}");
        }

        private GameSaveData CreateNewSave(SaveReferenceData saveReference)
        {
            _currentSaveReference.Value = saveReference;
            _metadata.Saves.Add(saveReference);
            _currentGameSaveData = new GameSaveData()
            {
                GameplaySaveData = Object.Instantiate(_initialGameSaveData).GameplaySaveData,
            };
            _currentGameSaveData.EnsureInitialized();
            
            Debug.Log($"[SaveLoadSystem] Creating new save >> {_currentSaveReference.Value.Filename}");
            return _currentGameSaveData;
        }
        
        private GameSaveData Load(SaveReferenceData saveReference)
        {
            _currentSaveReference.Value = saveReference;
            var loadedSave = _fileSystem.LoadTextFromFile(saveReference.Filename);
            _currentGameSaveData = _serializeContract.Deserialize<GameSaveData>(loadedSave);
            _currentGameSaveData ??= new GameSaveData();
            _currentGameSaveData.EnsureInitialized();
            
            Debug.Log($"[SaveLoadSystem] Loading save >> {_currentSaveReference.Value.Filename}");
            return _currentGameSaveData;
        }

        private GameSaveData EnsureCurrentSaveData()
        {
            if (_currentGameSaveData == null)
            {
                CreateNewSave(CreateSaveReference());
            }

            _currentGameSaveData.EnsureInitialized();
            return _currentGameSaveData;
        }

        private SaveReferenceData CreateSaveReference()
        {
            var saveIndex = _metadata.Saves.Count;
            return new SaveReferenceData
            {
                Filename = $"Save_{saveIndex}.dat",
                DisplayName = $"Save_{saveIndex}",
                LastSaveDateTime = DateTime.Now,
            };
        }
    }
}
