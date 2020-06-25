using MongoDB.Driver;

namespace KhV.MongoDb.Context
{
    public interface IMongoDbContext
    {
        IMongoDatabase Database { get; }
    }

    public class MongoDbContext : IMongoDbContext
    {
        public static IMongoClient Client;
        public MongoDbContext(MongoDbSettings connectionSetting)
        {
            if (Client == null)
            {
                Client = new MongoClient(connectionSetting.ConnectionString);
            }
            Database = Client.GetDatabase(connectionSetting.DatabaseName);
        }

        public IMongoDatabase Database { get; }
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
