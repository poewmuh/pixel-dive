using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils._Project.Utils
{
    public class FileSystem
    {
        public string PersistentPath { get; private set; }
        
        public FileSystem()
        {
            PersistentPath = UnityEngine.Application.persistentDataPath;
        }

        public FileSystem(string subFolder)
        {
            PersistentPath = UnityEngine.Application.persistentDataPath + '/' + subFolder + '/';

            if (!Directory.Exists(PersistentPath))
                Directory.CreateDirectory(PersistentPath);
        }

        public void SaveToFile(string relativePath, object obj)
        {
            switch (obj)
            {
                case string text:
                    SaveToFile(relativePath, text);
                    break;
                case byte[] bytes:
                    SaveToFile(relativePath, bytes);
                    break;
            }
        }
        
        public void SaveToFile(string relativePath, byte[] bytes)
        {
            var fullPath = GetFullPath(relativePath);
            File.WriteAllBytes(fullPath, bytes);
            
            Debug.Log($"[FileSystem] Saved to {fullPath}");
        }
        
        public void SaveToFile(string relativePath, string text)
        {
            var fullPath = GetFullPath(relativePath);
            File.WriteAllText(fullPath, text);
            
            Debug.Log($"[FileSystem] Saved to {fullPath}");
        }
        
        public Task SaveToFileAsync(string relativePath, byte[] bytes)
        {
            var fullPath = GetFullPath(relativePath);
            Debug.Log($"[FileSystem] SavedAsync to {fullPath}");
            return File.WriteAllBytesAsync(fullPath, bytes);
        }

        public Task SaveToFileAsync(string relativePath, string text)
        {
            var fullPath = GetFullPath(relativePath);
            Debug.Log($"[FileSystem] SavedAsync to {fullPath}");
            return File.WriteAllTextAsync(fullPath, text);
        }
        
        public string LoadTextFromFile(string relativePath)
        {
            var fullPath = GetFullPath(relativePath);
            Debug.Log($"[FileSystem] Load from {fullPath}");
            return File.ReadAllText(fullPath);
        }

        public byte[] LoadBytesFromFile(string relativePath)
        {
            var fullPath = GetFullPath(relativePath);
            Debug.Log($"[FileSystem] Load from {fullPath}");
            return File.ReadAllBytes(fullPath);
        }
        
        public Task<byte[]> LoadBytesFromFileAsync(string relativePath)
        {
            var fullPath = GetFullPath(relativePath);
            Debug.Log($"[FileSystem] LoadAsync from {fullPath}");
            return File.ReadAllBytesAsync(fullPath);
        }

        public Task<string> LoadTextFromFileAsync(string relativePath)
        {
            var fullPath = GetFullPath(relativePath);
            Debug.Log($"[FileSystem] LoadAsync from {fullPath}");
            return File.ReadAllTextAsync(fullPath);
        }
        
        public bool FileExists(string relativePath)
        {
            var fullPath = GetFullPath(relativePath);
            return File.Exists(fullPath);
        }
        
        private string GetFullPath(string relativePath)
        {
            return PersistentPath + relativePath;
        }
    }
}