using System.Collections.Generic;
using System.Net;
using AdaptiveCards.Rendering;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using AdaptiveCards;

namespace WeightLess
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessageAsync(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                IConversationUpdateActivity iConversationUpdated = message as IConversationUpdateActivity;
                if (iConversationUpdated != null)
                {
                    ConnectorClient connector = new ConnectorClient(new System.Uri(message.ServiceUrl));

                    foreach (var member in iConversationUpdated.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                    {
                        // if the bot is added, then
                        if (member.Id == iConversationUpdated.Recipient.Id)
                        {
                            HeroCard card = new HeroCard
                            {
                                Title = "Вас приветствует WeightLess бот.",
                                Text = "Данный бот помогает следить за вашим здоровьем и балансом калорий в организме. Для начала работы с ботом отправьте любую\nпоследовательность символов",
                            };
                            //AdaptiveTextBlock adaptiveTextBlock = message.Text("Привет");
                            
                            Activity replyToConversation = message.CreateReply("");
                            // replyToConversation.Attachments = new List<Attachment>();
                            /*
                            AdaptiveCard card = new AdaptiveCard();

                            // Specify speech for the card.
                            // card.Speak = "<s>Your  meeting about \"Adaptive Card design session\"<break strength='weak'/> is starting at 12:30pm</s><s>Do you want to snooze <break strength='weak'/> or do you want to send a late notification to the attendees?</s>";

                            // Add text to the card.
                            card.Body.Add(new AdaptiveTextBlock()
                            {
                                Text = "Вас приветствует WeightLess бот",
                                Size = AdaptiveTextSize.Large,
                                Weight = AdaptiveTextWeight.Bolder
                            });
                            // Add text to the card.
                            card.Body.Add(new AdaptiveTextBlock()
                            {
                                Text = "Данный бот помогает следить за вашим здоровьем и"
                            });
                            // Add text to the card.
                            card.Body.Add(new AdaptiveTextBlock()
                            {
                                Text = "балансом калорий в организме"
                            });
                            // Add text to the card.
                            card.Body.Add(new AdaptiveTextBlock()
                            {
                                Text = "Для начала работы с ботом отправьте любую"
                            });

                            card.Body.Add(new AdaptiveTextBlock()
                            {
                                Text = "последовательность символов "
                            });

                            // Create the attachment.
                            
                            Attachment attachment = new Attachment()
                            {
                                ContentType = AdaptiveCard.ContentType,
                                Content = card
                            };*/
                            replyToConversation.Attachments.Add(card.ToAttachment());

                            await connector.Conversations.SendToConversationAsync(replyToConversation);
                            
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}