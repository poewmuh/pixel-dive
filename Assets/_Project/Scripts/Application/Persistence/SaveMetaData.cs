using System;
using System.Collections.Generic;

namespace _Project.Application.Persistence
{
    [Serializable]
    public class SavesMetaData
    {
        public List<SaveReferenceData> Saves { get; set; } = new();

        public void EnsureInitialized()
        {
            Saves ??= new List<SaveReferenceData>();
            Saves.RemoveAll(save => save == null || string.IsNullOrWhiteSpace(save.Filename));
        }
    }
}
