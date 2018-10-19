using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace ImageBot.Responses
{
    public class RootResponses
    {
        public static async Task ReplyWithGreetingAsync(ITurnContext context)
        {
            // Add a greeting
            await context.SendActivityAsync($"Hi, I'm PictureBot!");
        }

        public static async Task ReplyWithHelpAsync(ITurnContext context)
        {
            await context.SendActivityAsync($"I can search for pictures, share pictures and order prints of pictures.");
        }

        public static async Task ReplyWithResumeTopicAsync(ITurnContext context)
        {
            await context.SendActivityAsync($"What can I do for you?");
        }

        public static async Task ReplyWithConfusedAsync(ITurnContext context)
        {
            // Add a response for the user if Regex or LUIS doesn't know
            // What the user is trying to communicate
            await context.SendActivityAsync($"I'm sorry, I don't understand.");
        }

        public static async Task ReplyWithLuisScoreAsync(ITurnContext context, string key, double score)
        {
            await context.SendActivityAsync($"Intent: {key} ({score}).");
        }

        public static async Task ReplyWithShareConfirmationAsync(ITurnContext context)
        {
            await context.SendActivityAsync($"Posting your picture(s) on twitter...");
        }

        public static async Task ReplyWithOrderConfirmationAsync(ITurnContext context)
        {
            await context.SendActivityAsync($"Ordering standard prints of your picture(s)...");
        }
    }
}
