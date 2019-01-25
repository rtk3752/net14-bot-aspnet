using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBot.Logic
{
    public static class Mongoloide
    {
        public static IMongoCollection<T> GetMongoCollection<T>(string dbName, string table)
        {
            var client = new MongoClient();

            var db = client.GetDatabase(dbName);
            var col = db.GetCollection<T>(table);

            return col;
        }
    }
}