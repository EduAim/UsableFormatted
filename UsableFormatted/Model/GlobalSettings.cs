using Realms;

namespace UsableFormatted.Model
{
    public class GlobalSettings : RealmObject
    {
        [PrimaryKey]
        public long Id { get; set; }

        public string Language { get; set; } = string.Empty;
    }
}