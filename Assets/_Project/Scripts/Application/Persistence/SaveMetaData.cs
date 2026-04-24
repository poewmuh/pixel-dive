using System;
using System.Collections.Generic;

namespace _Project.Application.Persistence
{
    [Serializable]
    public class SavesMetaData
    {
        public List<SaveReferenceData> Saves { get; set; } = new();
    }
}