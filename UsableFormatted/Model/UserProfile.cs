using MongoDB.Bson;
using Realms;

namespace UsableFormatted.Model
{
    public class UserProfile : RealmObject
    {
        [PrimaryKey]
        public long Id { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[0];
        public short BirthYear { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string LanguageIcList { get; set; } = string.Empty;

        public string FontName { get; set; } = "Ariel";
        public decimal FontSize { get; set; } = 14;
        public decimal HeadingFontSize { get; set; } = 16;
        public decimal LineSpace { get; set; } = 1;

        public UserProfile Detached()
        {
            return new UserProfile
            {
                Id = Id,
                Email = Email,
                FullName = FullName,
                PasswordHash = PasswordHash,
                BirthYear = BirthYear,
                Gender = Gender,
                LanguageIcList = LanguageIcList,
                FontName = FontName,
                FontSize = FontSize,
                HeadingFontSize = HeadingFontSize,
                LineSpace = LineSpace,
            };
        }
    }
}