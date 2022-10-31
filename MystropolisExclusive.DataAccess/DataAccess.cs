using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace MystropolisExclusive.DataAccess
{
    public static class DataAccess
    {

        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(MystiConnection.DatbaseFileName, CreationCollisionOption.OpenIfExists);

            using (var conn = MystiConnection.Connect())
            {
                conn.Db.Execute(@"CREATE TABLE IF NOT EXISTS MysticlusiveCodes 
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                        Code NVARCHAR(200) NOT NULL,
                        OneTimeUse BOOL NOT NULL,
                        Video NVARCHAR(2048) NOT NULL,
                        Remarks NVARCHAR(2048) NULL,
                        MinimumDuration INTEGER NULL, 
                        Used BOOL NOT NULL,
                        UsedDateTime DATETIME NULL
                    )");
            }
        }

        public static IReadOnlyCollection<MysticlusiveCode> GetAllCodes()
        {
            using (var conn = MystiConnection.Connect())
            {
                return conn.Db.Query<MysticlusiveCode>("SELECT * FROM MysticlusiveCodes").ToArray();
            }
        }

        public static void SaveCode(MysticlusiveCode code)
        {
            using (var conn = MystiConnection.Connect())
            {
                conn.Db.Execute(@"UPDATE MysticlusiveCodes 
                    SET Code = @Code,
                        Video = @Video,
                        OneTimeUse = @OneTimeUse,
                        Used = @Used,
                        MinimumDuration = @MinimumDuration,
                        Remarks = @Remarks
                    WHERE Id = @Id", code);
            }
        }

        public static void InsertCode(MysticlusiveCode code)
        {
            using (var conn = MystiConnection.Connect())
            {
                conn.Db.Execute(@"INSERT INTO MysticlusiveCodes 
                    (Code, Video, OneTimeUse, Used, MinimumDuration, Remarks)
                    VALUES(@Code, @Video, @OneTimeUse, @Used, @MinimumDuration, @Remarks)", code);
            }
        }

        public static void DeleteCode(int id)
        {
            using (var conn = MystiConnection.Connect())
            {
                conn.Db.Execute(@"DELETE FROM MysticlusiveCodes WHERE Id = @Id", new { Id = id });
            }
        }

        public static MysticlusiveCode CheckCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return null;
            }

            using (var conn = MystiConnection.Connect())
            {
                return conn.Db.QueryFirstOrDefault<MysticlusiveCode>("SELECT * FROM MysticlusiveCodes WHERE Code = @Code", new { Code = code });
            }
        }

        public static void MarkAsUsed(string code)
        {
            using (var conn = MystiConnection.Connect())
            {
                conn.Db.Execute(@"UPDATE MysticlusiveCodes 
                    SET Used = true
                    WHERE Code = @Code", new { Code = code });
            }
        }
    }
}