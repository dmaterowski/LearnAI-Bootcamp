using System.Threading;
using System.Threading.Tasks;
using ImageBot;
using ImageBot.Dialogs;
using ImageBot.Responses;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace PictureBot
{
    public class PictureBot : IBot
    {
        private readonly PictureBotStateAccessors _accessors;
        private readonly LuisRecognizer _luisRecognizer;

        // Define the dialog set for the bot.
        private readonly DialogSet _dialogs;

        public PictureBot(PictureBotStateAccessors accessors, LuisRecognizer luisRecognizer)
        {
            _accessors = accessors;
            this._luisRecognizer = luisRecognizer;
            _dialogs = new RootDialog(_accessors.DialogState, luisRecognizer);
        }

        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Activity.Type is ActivityTypes.Message)
            {
                DialogContext dc = await _dialogs.CreateContextAsync(context, cancellationToken);

                await dc.ContinueDialogAsync();

                // Every turn sends a response, so if no response was sent,
                // then there no dialog is currently active.
                if (!context.Responded)
                {
                    var state = await _accessors.GreetingState.GetAsync(context, () => new GreetingState());

                    // Greet them if we haven't already
                    if (state.Greeted == "not greeted")
                    {
                        await RootResponses.ReplyWithGreetingAsync(context);
                        state.Greeted = "greeted";
                        await _accessors.GreetingState.SetAsync(context, state);
                    }
                  
                    await dc.BeginDialogAsync(RootDialog.Id, context.Activity.Text);
                }

                await _accessors.ConversationState.SaveChangesAsync(context, false, cancellationToken);
            }
        }
    }

}
