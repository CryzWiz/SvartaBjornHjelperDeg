using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Bot.Sample.SimpleEchoBot;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string FlightsOption = "Fly";

        private const string HotelsOption = "Hotell";

        private const string EchoOption = "EchoTest";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.ToLower().Contains("#hjelp") || message.Text.ToLower().Contains("#support") || message.Text.ToLower().Contains("#problem"))
            {
                await context.Forward(new SupportDialog(), this.ResumeAfterSupportDialog, message, CancellationToken.None);
            }
            else
            {
                this.ShowOptions(context);
            }
        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { FlightsOption, HotelsOption, EchoOption }, "Hva kan jeg hjelpe deg med?", "Ikke gyldig valg", 4);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case FlightsOption:
                        context.Call(new FlightsDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case HotelsOption:
                        context.Call(new HotelsDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case EchoOption:
                        context.Call(new EchoDialog(), this.ResumeAfterOptionDialog);
                        break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! For mange forsøk :(. Men eg tar hånd om exception, så du kan prøve igjen!");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<object> result)
        {
            var ticketNumber = await result;

            await context.PostAsync($"Takk for at du sa ifra om problemet. Ticket er {ticketNumber}.");
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"ERROR. Feilet med beskjed: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}