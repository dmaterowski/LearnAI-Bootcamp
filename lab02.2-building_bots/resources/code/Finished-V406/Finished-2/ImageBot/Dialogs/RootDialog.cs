using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace ImageBot.Dialogs
{

    public class RootDialog : DialogSet
    {
        public const string Id = "rootDialog";
        private const string TextPrompt = "textPrompt";

        public RootDialog(IStatePropertyAccessor<DialogState> dialogStateAccessor)
            : base(dialogStateAccessor)
        {
            Add(new WaterfallDialog(
                Id,
                new WaterfallStep[]
                {
                    RootDialogSteps.PresentMenuAsync,
                    RootDialogSteps.RunDialogsAsync,
                }));
            Add(new SearchDialog());
            Add(new TextPrompt(TextPrompt));
        }

        private static class RootDialogSteps
        {
            public static async Task<DialogTurnResult> PresentMenuAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                await stepContext.PromptAsync(TextPrompt, new PromptOptions { Prompt = MessageFactory.Text("How can I help?") }, cancellationToken);
                return Dialog.EndOfTurn;
            }

            public static async Task<DialogTurnResult> RunDialogsAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                switch (stepContext.Result as string)
                {
                    case "search":
                        return await stepContext.BeginDialogAsync(SearchDialog.DialogId);
                    default:
                        return Dialog.EndOfTurn;
                }
            }
        }
    }
}
