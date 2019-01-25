using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBot.Logic
{
    public class SimpleBotUser
    {
        public string Reply(SimpleMessage message)
        {
            //string userId = message.Id;

            //UserProfile userProfile = GetProfile(userId);

            //userProfile.AccessCount++;

            //SetProfile(userId, userProfile);

            return $"{message.User} disse '{message.Text}";
        }
        private UserProfile GetProfile(string userId)
        {
            IMongoCollection<BsonDocument> collection = Mongoloide.GetMongoCollection<BsonDocument>("BotDB", "tbl_user");

            //Adicionar UPSERT

            return new UserProfile();
        }

        private void SetProfile(string userId, UserProfile userProfile)
        {
            throw new NotImplementedException();
        }
    }
}