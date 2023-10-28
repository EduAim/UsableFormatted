using System;
using Realms;

namespace UsableFormatted.Model
{
    public class DocumentFileInfo : RealmObject
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Required]
        public string FullFileName { get; set; } = string.Empty;

        public long LastUseTime { get; set; } = 0;

        public long UserId { get; set; } = 0;
    }
}