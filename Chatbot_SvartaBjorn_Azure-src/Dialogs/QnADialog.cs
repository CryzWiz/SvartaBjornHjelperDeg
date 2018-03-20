using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    [QnAMaker("7d26f05ae72842478df8fdca921de66d", "025fd52b-e8d7-43aa-a10f-e8f9bde3e369")]
    public class QnADialog : QnAMakerDialog
    {
    }
}