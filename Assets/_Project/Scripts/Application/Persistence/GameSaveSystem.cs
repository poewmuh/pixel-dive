using System;
using _Project.Application.Persistence.Data;
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

        private SaveReferenceData _currentSaveReference;
        private GameSaveData _currentGameSaveData;

        private FileSystem _fileSystem;
        private SavesMetaData _metadata;
        private ISerializeContract _serializeContract;

        GameSaveData IGameSaveSystem.CurrentSaveData => EnsureCurrentSaveData();
        SavesMetaData IGameSaveSystem.MetaData => EnsureMetaData();

        void IInitializable.Initialize()
        {
            InitializeStorage();
            LoadMetaData();
        }

        GameSaveData IGameSaveSystem.CreateNewSave()
        {
            return CreateNewSave(CreateSaveReference());
        }

        GameSaveData IGameSaveSystem.LoadSave(SaveReferenceData saveReference)
        {
            if (CanLoad(saveReference))
            {
                return Load(saveReference);
            }
            
            Debug.LogWarning("[SaveLoadSystem] Requested save is missing. Creating a new save instead.");
            return CreateNewSave(CreateSaveReference());
        }

        GameSaveData IGameSaveSystem.CreateNewOrLoad(SaveReferenceData saveReference)
        {
            return CanLoad(saveReference)
                ? Load(saveReference)
                : CreateNewSave(CreateSaveReference());
        }

        void IGameSaveSystem.Save()
        {
            EnsureCurrentSaveData();
            _currentSaveReference.LastSaveDateTime = DateTime.Now;
            _fileSystem.SaveToFile(MetadataFileName, _serializeContract.Serialize(_metadata));
            _fileSystem.SaveToFile(
                _currentSaveReference.Filename,
                _serializeContract.Serialize(_currentGameSaveData));

            Debug.Log($"[SaveLoadSystem] Saving >> {_currentSaveReference.Filename}");
        }

        private GameSaveData CreateNewSave(SaveReferenceData saveReference)
        {
            EnsureMetaData();
            
            _currentSaveReference = saveReference;
            AddSaveReference(saveReference);
            _currentGameSaveData = CreateSaveDataFromTemplate();

            Debug.Log($"[SaveLoadSystem] Creating new save >> {_currentSaveReference.Filename}");
            return _currentGameSaveData;
        }

        private GameSaveData Load(SaveReferenceData saveReference)
        {
            _currentSaveReference = saveReference;
            var loadedSave = _fileSystem.LoadTextFromFile(saveReference.Filename);
            _currentGameSaveData = _serializeContract.Deserialize<GameSaveData>(loadedSave);
            _currentGameSaveData ??= new GameSaveData();
            _currentGameSaveData.EnsureInitialized();

            Debug.Log($"[SaveLoadSystem] Loading save >> {_currentSaveReference.Filename}");
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
            EnsureMetaData();
            
            var saveIndex = GetNextSaveIndex();
            return new SaveReferenceData
            {
                Filename = $"Save_{saveIndex}.dat",
                DisplayName = $"Save_{saveIndex}",
                LastSaveDateTime = DateTime.Now,
            };
        }

        private void InitializeStorage()
        {
            var isDevelopment = UnityEngine.Application.isEditor || Debug.isDebugBuild;
            var subFolder = isDevelopment ? "Development" : "Production";
            _fileSystem = new FileSystem(subFolder);
            _serializeContract = new JSONSerialize();
        }

        private void LoadMetaData()
        {
            if (_fileSystem.FileExists(MetadataFileName))
            {
                var loadedMetaData = _fileSystem.LoadTextFromFile(MetadataFileName);
                _metadata = _serializeContract.Deserialize<SavesMetaData>(loadedMetaData);
            }

            EnsureMetaData();
        }

        private SavesMetaData EnsureMetaData()
        {
            _metadata ??= new SavesMetaData();
            _metadata.EnsureInitialized();
            return _metadata;
        }

        private GameSaveData CreateSaveDataFromTemplate()
        {
            var saveData = new GameSaveData
            {
                GameplaySaveData = _initialGameSaveData != null
                    ? Object.Instantiate(_initialGameSaveData).GameplaySaveData
                    : new GameplaySaveData()
            };
            
            saveData.EnsureInitialized();
            return saveData;
        }

        private bool CanLoad(SaveReferenceData saveReference)
        {
            return saveReference != null &&
                   !string.IsNullOrWhiteSpace(saveReference.Filename) &&
                   _fileSystem.FileExists(saveReference.Filename);
        }

        private void AddSaveReference(SaveReferenceData saveReference)
        {
            foreach (var save in _metadata.Saves)
            {
                if (save.Filename == saveReference.Filename) return;
            }
            
            _metadata.Saves.Add(saveReference);
        }

        private int GetNextSaveIndex()
        {
            var saveIndex = _metadata.Saves.Count;
            while (SaveReferenceExists($"Save_{saveIndex}.dat"))
            {
                saveIndex++;
            }

            return saveIndex;
        }

        private bool SaveReferenceExists(string filename)
        {
            foreach (var save in _metadata.Saves)
            {
                if (save.Filename == filename)
                {
                    return true;
                }
            }

            return _fileSystem.FileExists(filename);
        }
    }
}
