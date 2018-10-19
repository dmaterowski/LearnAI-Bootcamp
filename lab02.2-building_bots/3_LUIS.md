## 3_LUIS:
Estimated Time: 10-15 minutes

Our bot is now capable of taking in a user's input, calling Azure Search, and returning the results in a carousel of Hero cards. Unfortunately, our bot's communication skills are brittle. One typo, or a rephrasing of words, and the bot will not understand. This can cause frustration for the user. We can greatly increase the bot's conversation abilities by enabling it to understand natural language with the LUIS model we built yesterday in "lab01.5-luis."  

We will have to update our bot in order to use LUIS.

### Lab 3.1: Adding LUIS to Startup.cs

First, install Microsoft.Bot.Builder.AI.Luis 4.0.6 package

We are going to simplify the setup a bit and create a recognizer as a regular registered service. You can do that in `Startup.cs`
with
```csharp
            var luisApp = new LuisApplication("appid", "key", "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/");
            var recognizer = new LuisRecognizer(luisApp);
            services.AddSingleton(recognizer);

```
If you want to use the whole framework for supporting multiple models check out the [guide](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=cs)

Use the app ID, subscription ID, and base URI for your LUIS model. The base URI will be "https://region.api.cognitive.microsoft.com" , where region is the region associated with the key you are using. Some examples of regions are, `westus`, `westcentralus`, `eastus2`, and `southeastasia`.  

You can find your base URL by logging into www.luis.ai, going to the **Publish** tab, and looking at the **Endpoint** column under **Resources and Keys**. The base URL is the portion of the **Endpoint URL** before the subscription ID and other parameters.  

**Hint**: The LUIS App ID will have hyphens in it, and the LUIS subscription key will not.  

### Lab 3.2: Adding LUIS to bot

Now we can inject `LuisRecognizer` in the bot constructor. Then we can call it in the `OnTurnAsync`:
```csharp
var result = await _luisRecognizer.RecognizeAsync(context, cancellationToken);
var topIntnet = result.GetTopScoringIntent();
await RootResponses.ReplyWithLuisScoreAsync(context, topIntnet.intent, topIntnet.score);
```

Run your bot and see if it works!

### Lab 3.3 Luis in a dialog
WaterfallStep is just a function, we can create it, e.g. 
```csharp
return async (stepContext, token) =>
{
    var result = await recognizer.RecognizeAsync(stepContext.Context, token);
    //...
```

Pass the recognizer to RootDialog. 
Modify the RootDialog code to switch action based on LUIS intent. 
Run SearchDialog on SearchPics, send a dummy message on "OrderPic"
If None intent was found send a prompt with choice of possible functionalities
```csharp
await stepContext.PromptAsync(
            ChoicePrompt,
            new PromptOptions
            {
                Prompt = MessageFactory.Text("I can:"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Order pictures", "Search for pictures" }),
            },
            token);
```
If you run it, result of the Prompt will carry over to next step (which you need to create) as Result.



If you have extra time, see if there are things LUIS isn't picking up on that you expected it to. Maybe now is a good time to go to luis.ai, [review your endpoint utterances](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/label-suggested-utterances), and retrain/republish your model. 


> Fun Aside: Reviewing the endpoint utterances can be extremely powerful.  LUIS makes smart decisions about which utterances to surface.  It chooses the ones that will help it improve the most to have manually labeled by a human-in-the-loop.  For example, if the LUIS model predicted that a given utterance mapped to Intent1 with 47% confidence and predicted that it mapped to Intent2 with 48% confidence, that is a strong candidate to surface to a human to manually map, since the model is very close between two intents.  


**Extra credit (to complete later)**: Create a process for ordering prints with the bot using dialogs, responses, and models.  Your bot will need to collect the following information: Photo size (8x10, 5x7, wallet, etc.), number of prints, glossy or matte finish, user's phone number, and user's email. The bot will then want to send you a confirmation before submitting the request.




### Continue to [4_Publish_and_Register](./4_Publish_and_Register.md)  
Back to [README](./0_README.md)
