using System;

namespace FileServer.Database
{
    public interface IDatabase
    {

        public void StoreFileDetails(DbFileInfo fileInfo);

        public DbFileInfo GetFileInfo(Guid fileId);
    }
}
