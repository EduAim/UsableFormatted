using Realms;

namespace UsableFormatted.Model
{
    internal class Authorization : RealmObject
    {
        [PrimaryKey]
        public long Id { get; set; }

        public long UserId { get; set; } = 0;
        public string Email { get; set; } = string.Empty;
    }
}