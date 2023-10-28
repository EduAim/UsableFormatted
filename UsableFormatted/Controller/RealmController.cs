using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UsableFormatted.Model;
using Realms;

namespace UsableFormatted.Controller
{
    internal static class RealmController
    {
        public static Realm Instance => Realm.GetInstance(new RealmConfiguration { 
            SchemaVersion = 3,
            MigrationCallback = RealmMigration,
        });

        private static void RealmMigration(Migration migration, ulong oldSchemaVersion)
        {
            var oldVersion = oldSchemaVersion;
            if(oldVersion == 0)
            {
                oldVersion++;
            }

            //if (oldVersion == 1)
            //{
            //    //var oldUsers = migration.OldRealm.All<UserProfile>();
            //    var newUsers = migration.NewRealm.All<UserProfile>();

            //    for (var i = 0; i < newUsers.Count(); i++)
            //    {
            //        //var oldUser = oldUsers.ElementAt(i);
            //        var newUser = newUsers.ElementAt(i);
            //    }
            //    oldVersion++;
            //}

            //if (oldSchemaVersion == 7)
            //{
            //    var newDocFiles = migration.NewRealm.All<DocumentFileInfo>();
            //    oldVersion++;
            //}
        }
    }
}
