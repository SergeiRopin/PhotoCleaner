using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Configuration;

namespace PhotoCleaner.Database.MongoDB
{
    public static class MongoDbConnector
    {
        public static IMongoDatabase GetConnection()
        {
            RegisterConventions();

            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient client = new MongoClient(connectionString);
            return client.GetDatabase(connection.DatabaseName);
        }

        private static void RegisterConventions()
        {
            var conventionPack = new ConventionPack();
            conventionPack.Add(new CamelCaseElementNameConvention());
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
        }
    }
}