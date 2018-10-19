using Microsoft.Bot.Builder;
using System.Threading.Tasks;

namespace ImageBot.Responses
{
    public class RootResponses
    {
        public static async Task ReplyWithGreetingAsync(ITurnContext context)
        {
            // Add a greeting
            await context.SendActivityAsync($"Hi, I'm PictureBot!");
        }
        public static async Task ReplyWithSecondGreeting(ITurnContext context)
        {
            await context.SendActivityAsync($"Hi again!");
        }
    }
}
