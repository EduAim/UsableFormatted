using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsableFormatted.Controller;
using UsableFormatted.Model;
using UsfoModels;
using DocumentFormat.OpenXml.InkML;

namespace UsableFormatted.Repos
{
    internal static class RecentFilesRepo
    {
        internal static List<DocumentFileInfo> GetRecentFiles(long userId)
        {
            try
            {
                if (userId <= 0)
                    return new List<DocumentFileInfo>();

                var realm = RealmController.Instance;
                var recentFiles = realm.All<DocumentFileInfo>();
                return recentFiles.Where(x => x.UserId == userId).OrderByDescending(x => x.LastUseTime).ToList();
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return new List<DocumentFileInfo>();
            }
        }

        internal static bool AddUpdateRecentFile(string fullFileName, long userId)
        {
            try
            {
                if (userId <= 0)
                    return true;
                var realm = RealmController.Instance;
                var existing = realm.All<DocumentFileInfo>().Where(x => x.FullFileName == fullFileName && x.UserId == userId).FirstOrDefault();
                if (existing == null)
                {
                    var recentFile = new DocumentFileInfo
                    {
                        Id = DateTime.UtcNow.Ticks,
                        FullFileName = fullFileName,
                        LastUseTime = DateTime.UtcNow.Ticks,
                        UserId = userId,
                    };
                    realm.Write(() =>
                    {
                        realm.Add(recentFile);
                    });
                    return true;
                }

                realm.Write(() =>
                {
                    existing.LastUseTime = DateTime.UtcNow.Ticks;
                });
                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }

        }

        internal static bool RemoveRecentFile(string fullFileName, long userId)
        {
            try
            {
                if (userId <= 0)
                    return true;

                var realm = RealmController.Instance;
                var existing = realm.All<DocumentFileInfo>().Where(x => x.FullFileName == fullFileName && x.UserId == userId).FirstOrDefault();
                if (existing == null)
                    return true;

                realm.Write(() =>
                {
                    realm.Remove(existing);
                });

                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

    }
}
