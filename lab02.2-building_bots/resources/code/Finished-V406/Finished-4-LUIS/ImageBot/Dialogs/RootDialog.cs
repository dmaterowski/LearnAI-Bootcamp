using ImageBot.Responses;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ImageBot.Dialogs
{

    public class RootDialog : DialogSet
    {
        public const string Id = "rootDialog";
        private const string TextPrompt = "textPrompt";
        private const string ChoicePrompt = "choicePrompt";
        private readonly LuisRecognizer luisRecognizer;

        public RootDialog(IStatePropertyAccessor<DialogState> dialogStateAccessor, LuisRecognizer luisRecognizer)
            : base(dialogStateAccessor)
        {
            Add(new WaterfallDialog(
                Id,
                new WaterfallStep[]
                {
                    RootDialogSteps.MakeRunDialogStep(luisRecognizer),
                    RootDialogSteps.HandlePromptResultAsync,
                }));
            Add(new SearchDialog());
            Add(new TextPrompt(TextPrompt));
            Add(new ChoicePrompt(ChoicePrompt));
            this.luisRecognizer = luisRecognizer;
        }

        private static class RootDialogSteps
        {
            public static WaterfallStep MakeRunDialogStep(LuisRecognizer recognizer)
            {
                return async (stepContext, token) =>
                {
                    var result = await recognizer.RecognizeAsync(stepContext.Context, token);
                    var topIntnet = result.GetTopScoringIntent();
                    await RootResponses.ReplyWithLuisScoreAsync(stepContext.Context, topIntnet.intent, topIntnet.score);

                    switch (topIntnet.intent)
                    {
                        case "SearchPics":
                            return await stepContext.BeginDialogAsync(SearchDialog.DialogId);
                        case "OrderPic":
                            await RootResponses.ReplyWithOrderConfirmationAsync(stepContext.Context);
                            return await stepContext.EndDialogAsync();
                        default:
                            await RootResponses.ReplyWithConfusedAsync(stepContext.Context);
                            return await stepContext.PromptAsync(
                                ChoicePrompt,
                                new PromptOptions
                                {
                                    Prompt = MessageFactory.Text("I can:"),
                                    Choices = ChoiceFactory.ToChoices(new List<string> { "Order pictures", "Search for pictures" }),
                                },
                                token);
                    }
                };
            }

            public static async Task<DialogTurnResult> HandlePromptResultAsync(
                       WaterfallStepContext stepContext,
                       CancellationToken cancellationToken)
            {
                switch ((stepContext.Result as FoundChoice)?.Value)
                {
                    case "Order pictures":
                        return await stepContext.BeginDialogAsync(SearchDialog.DialogId);
                    case "Search for pictures":
                        await RootResponses.ReplyWithOrderConfirmationAsync(stepContext.Context);
                        return await stepContext.EndDialogAsync();
                    default:
                        await RootResponses.ReplyWithConfusedAsync(stepContext.Context);
                        return await stepContext.EndDialogAsync();
                }
            }
        }
    }
}
