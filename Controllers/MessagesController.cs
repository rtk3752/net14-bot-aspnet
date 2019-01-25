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
                InsertActivityLog(activity);

                await HandleActivityAsync(activity);
            }

            // HTTP 202
            return new HttpResponseMessage(HttpStatusCode.Accepted);
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

            message.Text = text; // Gambiarrinha rapida
            InsertActivityLog(message, isBot:true);
        }

        private static void InsertActivityLog(Activity activity, bool isBot = false)
        {
            string id = activity.From.Id;
            string name = activity.From.Name;
            string message = activity.Text;

            if (isBot)
            {
                id = "0";
                name = "BOT";
            }

            InsertActivityLog(id, name, message);
        }

        private static void InsertActivityLog(string userId, string userName, string mensagem)
        {
            var client = new MongoClient();
            
            var col = Mongoloide.GetMongoCollection<BsonDocument>("BotDB", "tbl_historico");

            var doc = new BsonDocument() {
                { "Criado", DateTime.Now },
                { "Mensagem", mensagem },
                { "UserId", userId },
                { "UserName", userName }
            };

            col.InsertOne(doc);
        }
    }
}