using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace ImageBot.Dialogs
{
    public class SearchDialog : ComponentDialog
    {
        public const string DialogId = "searchPicture";

        public SearchDialog()
            : base(DialogId)
        {
            InitialDialogId = Id;

            AddDialog(new WaterfallDialog(
                Id,
                new WaterfallStep[]
                {
                    SearchDialogSteps.PresentMenuAsync,
                }));
        }

        private static class SearchDialogSteps
        {
            public static async Task<DialogTurnResult> PresentMenuAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                await stepContext.Context.SendActivityAsync(
                    "Ready to search.",
                    cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }

        }
    }
}
