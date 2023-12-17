using UsfoModels.Model;
using UsableFormatted.Controller;
using DocumentFormat.OpenXml.Spreadsheet;
using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UsableFormatted.Model;
using UsfoModels;

namespace UsableFormatted.Repos
{
    delegate void UserChangedHandler(object sender, EventArgs e);

    internal static class UserProfileRepo
    {
        public static event UserChangedHandler? OnUserChanged = null;

        private const string SALT = "ŠķīvītisСлаваУкраїніPlauktā";

        private static UserProfile? _userProfile = null;
        internal static UserProfile? LoggedInUser { get { 
                var loggedInUser = GetLoggedInUser();
                if (loggedInUser.userId != (_userProfile?.Id ?? 0))
                {
                    _userProfile = GetUserProfile(loggedInUser.userId);
                }
                return _userProfile; 
            } }
        internal static long LoggedInUserId { get { return LoggedInUser?.Id ?? 0; } }
        internal static bool IsLoggedIn => LoggedInUserId > 0;

        internal static short AnonymousBirthYear { get; set; } = 0;

        internal static List<UserProfile> GetUsers()
        {
            try
            {
                var realm = RealmController.Instance;
                var users = realm.All<UserProfile>();
                return users.ToList();
            }
            catch(Exception ex)
            {
                ex.TraceEx();
                return new List<UserProfile>();
            }
        }

        internal static bool LoginUser(string email, string password = "", bool skipPassword = false)
        {
            try
            {
                var existing = GetUserProfileByEmail(email);
                if (existing == null)
                    return false;

                if (!skipPassword)
                {
                    var hash = GetPasswordHash(password);

                    if (!hash.SequenceEqual(existing.PasswordHash))
                        return false;
                }

                _userProfile = existing;
                RegisterLoggedInUser(_userProfile.Id, _userProfile.Email);

                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        internal static bool LogoutUser()
        {
            try
            {
                if (_userProfile == null)
                    return true;

                _userProfile = null;
                RegisterLoggedInUser(0, string.Empty);
                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        internal static bool CreateUser(UserProfile userProfile)
        {
            try
            {
                var realm = RealmController.Instance;
                if (userProfile.Id <= 0)
                    userProfile.Id = DateTime.UtcNow.Ticks;

                var existing = realm.All<UserProfile>().Where(x => x.Email == userProfile.Email).FirstOrDefault();
                if (existing != null)
                {
                    return false;
                }


                realm.Write(() =>
                {
                    realm.Add(userProfile);
                });

                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        internal static bool CreateUser(string email, string password, short birthYear)
        {
            try
            {
                FormatSet formatSet = Recommendations.GetByAge(DateTime.Now.Year - birthYear);

                var userProfile = new UserProfile
                {
                    Email = email,
                    PasswordHash = GetPasswordHash(password),
                    BirthYear = birthYear,
                    FontName = formatSet.Font,
                    FontSize = formatSet.FontSize,
                    HeadingFontSize = formatSet.HeadingFontSize,
                };

                return CreateUser(userProfile);
            }
            catch(Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        internal static bool UpdateUser(UserProfile userProfile)
        {
            try
            {
                var realm = RealmController.Instance;
                var existing = realm.All<UserProfile>().Where(x => x.Id == userProfile.Id).FirstOrDefault();
                if (existing == null)
                    return false;
                realm.Write(() =>
                {
                    if (!string.IsNullOrEmpty(userProfile.Email))
                        existing.Email = userProfile.Email;
                    if (!string.IsNullOrEmpty(userProfile.FullName))
                        existing.FullName = userProfile.FullName;
                    if (userProfile.BirthYear > 0)
                        existing.BirthYear = userProfile.BirthYear;
                    existing.LanguageIcList = userProfile.LanguageIcList;
                    if (!string.IsNullOrEmpty(userProfile.FontName))
                        existing.FontName = userProfile.FontName;
                    if (userProfile.FontSize > 0)
                        existing.FontSize = userProfile.FontSize;
                    if (userProfile.HeadingFontSize > 0)
                        existing.HeadingFontSize = userProfile.HeadingFontSize;
                    if (userProfile.LineSpace > 0)
                        existing.LineSpace = userProfile.LineSpace;
                    if (!string.IsNullOrEmpty(userProfile.Gender))
                        existing.Gender = userProfile.Gender;

                });
                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        internal static bool UpdatePassword(long userId, string password)
        {
            try
            {
                var realm = RealmController.Instance;
                var existing = realm.All<UserProfile>().Where(x => x.Id == userId).FirstOrDefault();
                if (existing == null)
                    return false;

                var hash = GetPasswordHash(password);
                realm.Write(() =>
                {
                    existing.PasswordHash = hash;
                });
                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        internal static bool UpdateUserFormat(long userId, FormatSet formatSet)
        {
            try
            {
                var realm = RealmController.Instance;
                var existing = realm.All<UserProfile>().Where(x => x.Id == userId).FirstOrDefault();
                if (existing == null)
                    return false;
                realm.Write(() =>
                {
                    existing.FontName = formatSet.Font;
                    existing.FontSize = formatSet.FontSize;
                    existing.HeadingFontSize = formatSet.HeadingFontSize;
                });
                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        /// <summary>
        /// Validates an email address
        /// https://stackoverflow.com/a/1374644/4120738
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        internal static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }


        private static byte[] GetPasswordHash(string password)
        {
            var salted = Encoding.UTF8.GetBytes(password + SALT);
            byte[] hash = SHA256.Create().ComputeHash(salted);
            return hash;
        }

        internal static FormatSet GetUserFormatSet()
        {
            if (_userProfile == null)
                return new FormatSet();

            var formatSet = new FormatSet
            {
                Font = _userProfile.FontName,
                FontSize = _userProfile.FontSize,
                HeadingFontSize = _userProfile.HeadingFontSize,
            };

            return formatSet;
        }

        internal static bool IsBirthYearValid(short birthYear)
        {
            var currentYear = DateTime.Now.Year;
            return birthYear < currentYear - 2 && birthYear > currentYear - 150;
        }

        private static bool RegisterLoggedInUser(long userId, string email)
        {
            try
            {
                var realm = RealmController.Instance;
                realm.Write(() =>
                {
                    var authorization = new Authorization
                    {
                        Id = 1,
                        UserId = userId,
                        Email = email
                    };
                    realm.Add(authorization, update: true);
                });
                if (OnUserChanged != null)
                    OnUserChanged(userId, new EventArgs());

                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        private static UserProfile? GetUserProfile(long userId)
        {
            try
            {
                var realm = RealmController.Instance;
                var existing = realm.All<UserProfile>();
                var list = existing.ToList();
                var existingUser = existing.Where(x => x.Id == userId).FirstOrDefault()?.Detached();
                return existingUser;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return null;
            }
        }

        private static UserProfile? GetUserProfileByEmail(string email)
        {
            try
            {
                var realm = RealmController.Instance;
                var existing = realm.All<UserProfile>().Where(x => x.Email == email).FirstOrDefault();

                return existing;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return null;
            }
        }

        private static (long userId, string email) GetLoggedInUser()
        {
            var realm = RealmController.Instance;
            var existing = realm.All<Authorization>().Where(x => x.Id == 1).FirstOrDefault();
            if (existing != null)
            {
                return (existing.UserId, existing.Email);
            }
            return (0, string.Empty);
        }


    }
}
