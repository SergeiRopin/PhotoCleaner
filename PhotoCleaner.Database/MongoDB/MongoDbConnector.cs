using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
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
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            // Fail fast when MongoDB is not running instead of waiting the default 30 seconds.
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(3);
            MongoClient client = new MongoClient(settings);
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