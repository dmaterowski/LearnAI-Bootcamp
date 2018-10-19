using ImageBot.Responses;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace ImageBot
{
    public class PictureBot : IBot
    {
        private readonly PictureBotStateAccessors _accessors;

        public PictureBot(PictureBotStateAccessors accessors)
        {
            _accessors = accessors;
        }

        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Activity.Type is ActivityTypes.Message)
            {

                var state = await _accessors.GreetingState.GetAsync(context, () => new GreetingState(), cancellationToken);

                // Greet them if we haven't already
                if (state.Greeted == "not greeted")
                {
                    await RootResponses.ReplyWithGreetingAsync(context);
                    state.Greeted = "greeted";
                    await _accessors.GreetingState.SetAsync(context, state, cancellationToken);
                }
                else
                {
                    await RootResponses.ReplyWithSecondGreeting(context);
                }

                await _accessors.ConversationState.SaveChangesAsync(context, false, cancellationToken);
            }
        }
    }

}
