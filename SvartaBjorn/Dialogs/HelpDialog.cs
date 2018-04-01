using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class HelpDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Dette er hjelpe dialogen. Her kan vi returnere en link, forklaring, tilbud om ekte mennesker osv." +
                "Du vil komme hit uansett hvor vi måtte være i en annen dialog. Vi kan legge til så mange globals som vi ønsker. Denne reagerer" +
                " kun på hjelp når det er alene i tekstfeltet. Skriv inn hva som helst for å komme tilbake og fortsette chatten.");

            context.Wait(this.MessageReceived);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if ((message.Text != null) && (message.Text.Trim().Length > 0))
            {
                context.Done<object>(null);
            }
            else
            {
                context.Fail(new Exception("Du skrev ikke inn et ord/setning."));
            }
        }
    }
}