using Microsoft.Data.Sqlite;
using System;
using Microsoft.Extensions.Configuration;

namespace FileServer.Database
{
    public class SQLiteHandler : IDatabase
    {
        private readonly SqliteConnection _connection;

        public SQLiteHandler(IConfiguration configuration)
        {
            var sqlitePath = configuration["SQLite:DBFile"];
            _connection = new SqliteConnection($"Data Source={sqlitePath}");
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='fileInfo';";
            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    var createTableCommand = _connection.CreateCommand();
                    createTableCommand.CommandText = "create table fileInfo(fileId GUID PRIMARY KEY, fileName TEXT NOT NULL, key BLOB NOT NULL, iv BLOB NOT NULL)";
                    createTableCommand.ExecuteNonQuery();
                }
            }

            _connection.Close();

        }

    public DbFileInfo GetFileInfo(Guid fileId)
        {
            DbFileInfo fileInfo = null;
            using (var connection = _connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "select fileName, key, iv from fileInfo where fileID = @guid";
                command.Parameters.AddWithValue("@guid", fileId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        fileInfo = new DbFileInfo
                        {
                            FileName = (string)reader["fileName"],
                            Key = (byte[])reader["key"],
                            Iv = (byte[])reader["iv"],
                            FileId = fileId
                        };
                    }
                }

            }

            return fileInfo;

        }

        public void StoreFileDetails(DbFileInfo fileInfo)
        {
            using (var connection = _connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "insert into fileInfo values(@guid, @filename, @key, @iv)";
                command.Parameters.AddWithValue("@guid", fileInfo.FileId);
                command.Parameters.AddWithValue("@filename", fileInfo.FileName);
                command.Parameters.AddWithValue("@key", fileInfo.Key);
                command.Parameters.AddWithValue("@iv", fileInfo.Iv);
                command.ExecuteNonQuery();

            }
        }
    }
}
