using System;

namespace _Project.Application.Persistence
{
    [Serializable]
    public class SaveReferenceData
    {
        public string Filename { get; set; }
        public string DisplayName { get; set; }
        public DateTime LastSaveDateTime { get; set; }
    }
}