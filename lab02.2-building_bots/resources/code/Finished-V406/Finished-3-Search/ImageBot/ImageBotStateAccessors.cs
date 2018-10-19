using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace ImageBot
{
    public class PictureBotStateAccessors
    {
        public PictureBotStateAccessors(ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
        }

        public static string GreetingStateName { get; } = $"{nameof(PictureBotStateAccessors)}.GreetingState";
        public static string DialogStateAccessorName { get; } = $"{nameof(PictureBotStateAccessors)}.DialogState";

        public IStatePropertyAccessor<GreetingState> GreetingState { get; set; }
        public IStatePropertyAccessor<DialogState> DialogState { get; set; }

        public ConversationState ConversationState { get; }
    }
}
