using System;
using Microsoft.Bot.Builder;

namespace ImageBot
{
    public class PictureBotStateAccessors
    {
        public PictureBotStateAccessors(ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
        }

        public static string GreetingStateName { get; } = $"{nameof(PictureBotStateAccessors)}.GreetingState";

        public IStatePropertyAccessor<GreetingState> GreetingState { get; set; }

        public ConversationState ConversationState { get; }
    }
}
