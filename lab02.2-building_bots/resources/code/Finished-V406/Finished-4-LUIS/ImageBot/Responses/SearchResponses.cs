using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace ImageBot.Responses
{
    public class SearchResponses
    {
        public static async Task ConfirmSearchAsync(ITurnContext context)
        {
            await context.SendActivityAsync($"Searching..");
        }

        public static async Task ReplyWithNoResults(ITurnContext context, string query)
        {
            await context.SendActivityAsync($"Sorry, no pictures found for ${query}");
        }


    }
}
