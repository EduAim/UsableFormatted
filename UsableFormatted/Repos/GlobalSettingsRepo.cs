using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UsableFormatted.Controller;
using UsableFormatted.Model;
using UsfoModels;

namespace UsableFormatted.Repos
{
    internal static class GlobalSettingsRepo
    {
        internal static GlobalSettings GetSettings()
        {
            try
            {
                var realm = RealmController.Instance;
                var settings = realm.All<GlobalSettings>().FirstOrDefault(x => x.Id == 1);
                if (string.IsNullOrEmpty(settings?.Language))
                {
                    var newSettings = new GlobalSettings {
                        Id = 1,
                        Language = GetDefaultLanguage(),
                    };
                    SaveSettings(newSettings);
                    return newSettings;
                }
                return new GlobalSettings()
                {
                    Id = 1,
                    Language = settings.Language,
                };
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return new GlobalSettings();
            }
        }

        internal static bool SaveSettings(GlobalSettings settings)
        {
            try
            {
                var realm = RealmController.Instance;
                var existing = realm.All<GlobalSettings>().FirstOrDefault(x => x.Id == 1);
                if (existing == null)
                {
                    settings.Id = 1;
                    realm.Write(() =>
                    {
                        realm.Add(settings);
                    });
                    return true;
                }

                realm.Write(() =>
                {
                    existing.Language = settings.Language;
                });
                return true;

            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        private static string GetDefaultLanguage()
        {
            try
            {
                uint size = GetKeyboardLayoutList(0, null);
                IntPtr[] ids = new IntPtr[size];
                GetKeyboardLayoutList(ids.Length, ids);
                var isLv = ids.Any(x => new CultureInfo((int)x & 0xFFFF).Name.ToLower().StartsWith("lv"));
                return isLv ? "lv" : "en";
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return "en";
            }
        }

        [DllImport("user32.dll")]
        static extern uint GetKeyboardLayoutList(int nBuff, [Out] IntPtr[] lpList);
    }
}
