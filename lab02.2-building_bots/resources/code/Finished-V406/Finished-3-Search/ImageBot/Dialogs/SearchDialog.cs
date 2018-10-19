using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageBot.Responses;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using PictureBot.Models;

namespace ImageBot.Dialogs
{
    public class SearchDialog : ComponentDialog
    {
        public const string DialogId = "searchPicture";
        private const string SearchQueryPrompt = "searchQuery";

        public SearchDialog()
            : base(DialogId)
        {
            InitialDialogId = Id;

            AddDialog(new WaterfallDialog(
                Id,
                new WaterfallStep[]
                {
                    SearchDialogSteps.AskForSearchQueryAsync,
                    SearchDialogSteps.SearchAsync,
                }));
            AddDialog(new TextPrompt(SearchQueryPrompt));
        }

        private static class SearchDialogSteps
        {
            public static async Task<DialogTurnResult> AskForSearchQueryAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                await stepContext.PromptAsync(SearchQueryPrompt, new PromptOptions { Prompt = MessageFactory.Text("What would you like to search for?") }, cancellationToken);
                return EndOfTurn;
            }

            public static async Task<DialogTurnResult> SearchAsync(
               WaterfallStepContext stepContext,
               CancellationToken cancellationToken)
            {
                await SearchResponses.ConfirmSearchAsync(stepContext.Context);
                await SearchDialog.SearchAsync(stepContext.Context, stepContext.Result as string);
                return await stepContext.EndDialogAsync();
            }

        }

        public static async Task SearchAsync(ITurnContext context, string searchText)
        {
            ISearchIndexClient indexClientForQueries = CreateSearchIndexClient();
            // For more examples of calling search with SearchParameters, see
            // https://github.com/Azure-Samples/search-dotnet-getting-started/blob/master/DotNetHowTo/DotNetHowTo/Program.cs.  
            // Call the search service and store the results
            DocumentSearchResult results = await indexClientForQueries.Documents.SearchAsync(searchText);
            await SendResultsAsync(context, searchText, results);
        }

        public static async Task SendResultsAsync(ITurnContext context, string searchText, DocumentSearchResult results)
        {
            IMessageActivity activity = context.Activity.CreateReply();
            // if the search returns no results
            if (results.Results.Count == 0)
            {
                await SearchResponses.ReplyWithNoResults(context, searchText);
            }
            else // this means there was at least one hit for the search
            {
                // create the response with the result(s) and send to the user
                SearchHitStyler searchHitStyler = new SearchHitStyler();
                searchHitStyler.Apply(
                    ref activity,
                    "Here are the results that I found:",
                    results.Results.Select(r => ImageMapper.ToSearchHit(r)).ToList().AsReadOnly());

                await context.SendActivityAsync(activity);
            }
        }

        public static ISearchIndexClient CreateSearchIndexClient()
        {
            // Configure the search service and establish a connection, call it in StartAsync()
            // replace "YourSearchServiceName" and "YourSearchServiceKey" with your search service values
            string searchServiceName = "[]";
            string queryApiKey = "[]";
            string indexName = "images";
            // if you named your index "images" as instructed, you do not need to change this value

            SearchIndexClient indexClient = new SearchIndexClient(searchServiceName, indexName, new SearchCredentials(queryApiKey));
            return indexClient;
        }
    }
}
