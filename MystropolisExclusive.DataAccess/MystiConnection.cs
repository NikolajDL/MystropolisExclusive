using Microsoft.Data.Sqlite;
using System;
using System.IO;
using Windows.Storage;

namespace MystropolisExclusive.DataAccess
{
    public class MystiConnection : IDisposable
    {
        public const string DatbaseFileName = "mystiqlite.db";

        public SqliteConnection Db { get; private set; }

        private MystiConnection()
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DatbaseFileName);

            Db = new SqliteConnection($"Filename={dbPath}");
            SQLitePCL.Batteries.Init();
            Db.Open();
        }

        public static MystiConnection Connect()
        {
            return new MystiConnection();
        }

        public void Dispose()
        {
            Db.Close();
            Db.Dispose();
        }
    }
}