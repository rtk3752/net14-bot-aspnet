using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using MongoDB.Bson;
using MongoDB.Driver;
using SimpleBot.Logic;

namespace SimpleBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        static SimpleBotUser g_bot = null;

        public MessagesController()
        {
            // Pattern: singletonn
            if (g_bot == null)
            {
                g_bot = new SimpleBotUser();
            }
        }

        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if ( activity != null && activity.Type == ActivityTypes.Message)
            {
                InsertActivityLog(activity.Text, activity.From.Name);
                await HandleActivityAsync(activity);
            }

            // HTTP 202
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        private static void InsertActivityLog(string mensagem, string userName)
        {
            var client = new MongoClient();
            var db = client.GetDatabase("BotDB");
            var col = db.GetCollection<BsonDocument>("tbl_historico");
            var doc = new BsonDocument() {
                { "Criado", DateTime.Now },
                { "Mensagem", mensagem },
                { "UserName", userName }
            };

            col.InsertOne(doc);
        }
        
        // Estabelece comunicacao entre o usuario e o SimpleBotUser
        async Task HandleActivityAsync(Activity activity)
        {
            string text = activity.Text;
            string userFromId = activity.From.Id;
            string userFromName = activity.From.Name;

            var message = new SimpleMessage(userFromId, userFromName, text);

            string response = g_bot.Reply(message);

            await ReplyUserAsync(activity, response);
        }

        // Responde mensagens usando o Bot Framework Connector
        async Task ReplyUserAsync(Activity message, string text)
        {
            var connector = new ConnectorClient(new Uri(message.ServiceUrl));
            var reply = message.CreateReply(text);

            await connector.Conversations.ReplyToActivityAsync(reply);

            InsertActivityLog(text, "BOT");
        }
    }
}