using Azure.AI.OpenAI;
using Azure;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly OpenAIClient _openAIClient;
        private readonly string _deploymentId;

        public EchoBot(IConfiguration configuration)
        {
            // var endpoint = new Uri(configuration["Endpoint"]);
            string nonAzureOpenAIApiKey = configuration["OpenAIApiKey"];
            // _deploymentId = configuration["DeploymentId"];

            _openAIClient = new OpenAIClient(nonAzureOpenAIApiKey,new OpenAIClientOptions());
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var userMessage = turnContext.Activity.Text;

            try
            {
                Response<ChatCompletions> response = await _openAIClient.GetChatCompletionsAsync(new ChatCompletionsOptions()
                {
                    DeploymentName = "gpt-3.5-turbo", // assumes a matching model deployment or model name
                    Messages =
                    {
                        new ChatRequestSystemMessage("あなたはアシスタントです。ユーザーからの質問に答えてください。"),
                        new ChatRequestUserMessage( userMessage ),
                    }
                });
                
                // var replyText = response.Value.ToString().ToUpperInvariant();
                var replyText = response.Value.Choices[0].Message.Content;
                ChatResponseMessage responseMessage = response.Value.Choices[0].Message;
                Console.WriteLine($"[{responseMessage.Role.ToString().ToUpperInvariant()}]: {responseMessage.Content}");
                
                Console.WriteLine(replyText);
                Console.WriteLine(response);

                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
            catch (Exception)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, something went wrong."), cancellationToken);
            }


            // // Optional: Log the conversation
            // await LogConversation(turnContext, userMessage, replyText);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "私はAIアシスタントです。";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        // private async Task LogConversation(ITurnContext<IMessageActivity> turnContext, string userMessage, string botReply)
        // {
        //     // Implement your logging logic here
        //     // Example: Log to a database or a file
        //     var logMessage = $"User: {userMessage}\nBot: {botReply}\nTimestamp: {DateTime.UtcNow}\n";
        //     Console.WriteLine(logMessage);
        // }
    }
}
