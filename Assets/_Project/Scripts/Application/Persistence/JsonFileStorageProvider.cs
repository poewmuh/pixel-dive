using System;
using System.IO;
using UnityEngine;

namespace _Project.Application.Persistence
{
    public class JsonFileStorageProvider : IStorageProvider
    {
        private const string FolderName = "SaveData";
        private readonly string _basePath;

        public JsonFileStorageProvider()
        {
            _basePath = Path.Combine(UnityEngine.Application.persistentDataPath, FolderName);
            
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        void IStorageProvider.Save<T>(string key, T data)
        {
            try
            {
                var json = JsonUtility.ToJson(data, true);
                var filePath = GetFilePath(key);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonFileStorage] Failed to save '{key}': {e.Message}");
            }
        }

        T IStorageProvider.Load<T>(string key)
        {
            var filePath = GetFilePath(key);
            
            if (!File.Exists(filePath))
            {
                return new T();
            }

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonFileStorage] Failed to load '{key}': {e.Message}");
                return new T();
            }
        }

        bool IStorageProvider.Exists(string key)
        {
            return File.Exists(GetFilePath(key));
        }

        void IStorageProvider.Delete(string key)
        {
            var filePath = GetFilePath(key);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private string GetFilePath(string key) => Path.Combine(_basePath, $"{key}.json");
    }
}