using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongV3
{
    public static class MongoLogger
    {
        private static IMongoCollection<BsonDocument> _collection;

        private static IMongoCollection<BsonDocument> GetCollection()
        {
            if (_collection == null)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = config["MongoDB:ConnectionString"];
                var databaseName = config["MongoDB:DatabaseName"] ?? "SongGuesserLogs";
                var collectionName = config["MongoDB:CollectionName"] ?? "GameLogs";

                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _collection = database.GetCollection<BsonDocument>(collectionName);
            }
            return _collection;
        }

        // CREATE: Insertar Log
        public static void LogAction(string user, string action, string details)
        {
            try
            {
                var col = GetCollection();
                var doc = new BsonDocument
                {
                    { "User", user },
                    { "Action", action },
                    { "Details", details },
                    { "Timestamp", DateTime.Now }
                };
                col.InsertOne(doc);
            }
            catch { }
        }

        public static async Task<List<string>> GetRecentLogsAsync(string userId, int limit = 5)
        {
            try
            {
                var col = GetCollection();
                var filter = Builders<BsonDocument>.Filter.Eq("User", userId);
                var sort = Builders<BsonDocument>.Sort.Descending("Timestamp");

                var docs = await col.Find(filter).Sort(sort).Limit(limit).ToListAsync();
                var result = new List<string>();

                foreach (var d in docs)
                {
                    // Formato: [Fecha] Acción: Detalle
                    var ts = d.Contains("Timestamp") ? d["Timestamp"].ToLocalTime().ToString("HH:mm") : "--:--";
                    var act = d.Contains("Action") ? d["Action"].ToString() : "N/A";
                    var det = d.Contains("Details") ? d["Details"].ToString() : "";
                    result.Add($"[{ts}] {act}: {det}");
                }
                return result;
            }
            catch
            {
                return new List<string> { "No se pudo conectar a MongoDB." };
            }
        }
    }
}