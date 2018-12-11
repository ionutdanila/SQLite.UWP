using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;

namespace TestApplicationStorage
{
    public static class SQLQuery
    {
        private static string BaseDatabasePath => Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Test.Data");

        private static SQLiteConnectionWithLock _connection;
        private static SQLiteConnectionWithLock Connection => _connection ?? (_connection = new SQLiteConnectionWithLock
        (
            new SQLiteConnectionString(BaseDatabasePath, false),
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache)
        );

        static SQLQuery()
        {
            InitializeDatabase();
        }

        internal static void InitializeDatabase()
        {
            try
            {
                using (Connection.Lock())
                {
                    Connection.CreateTable(typeof(KeyValueBase));
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        internal static void DropDatabase()
        {
            using (Connection.Lock())
            {
                Connection.DropTable(typeof(KeyValueBase));
            }
        }

        internal static List<T> SelectEntityProperties<T>() where T : KeyValueBase, new()
        {
            using (Connection.Lock())
                return Connection.Table<T>().ToList();
        }

        internal static void ClearTable<T>() where T : KeyValueBase, new()
        {
            using (Connection.Lock())
            {
                while (Connection.GetTableInfo(typeof(T).Name).Any())
                    Connection.DropTable<T>();

                Connection.CreateTable<T>();
            }
        }

        internal static int InsertOrUpdateEntityProperty<T>(T keyValueObject) where T : KeyValueBase, new()
        {
            using (Connection.Lock())
            {
                T existingEtity = GetEntityPropertyByKey<T>(keyValueObject.Key);
                if (existingEtity == null)
                    return Connection.Insert(keyValueObject, typeof(T));

                keyValueObject.Id = existingEtity.Id;
                return Connection.Update(keyValueObject, typeof(T));
            }
        }

        internal static int DeleteEntityByKey<T>(string key) where T : KeyValueBase, new()
        {
            using (Connection.Lock())
                return Connection.Table<T>().Delete(x => x.Key == key);
        }

        internal static T GetEntityPropertyByKey<T>(string key) where T : KeyValueBase, new()
        {
            using (Connection.Lock())
                return Connection.Table<T>().FirstOrDefault(x => x.Key == key);
        }

        internal static bool IsEntityAvailable<T>(string key) where T : KeyValueBase, new()
        {
            using (Connection.Lock())
                return Connection.Table<T>().Any(x => key.Equals(x.Key) && !string.IsNullOrWhiteSpace(x.Value));
        }

        internal static bool TableExists<T>()
        {
            using (Connection.Lock())
                return Connection.GetTableInfo(typeof(T).Name).Any();
        }
    }
}
