using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using System.Linq;
using System.Configuration;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    [QnAMaker("7d26f05ae72842478df8fdca921de66d", "025fd52b-e8d7-43aa-a10f-e8f9bde3e369", "Beklager, men jeg kan ikke svare på det akkurat nå.", 0.50, 3)]
    public class QnADialog : QnAMakerDialog
    {
        private static string subKey = "7d26f05ae72842478df8fdca921de66d";
        private static string kID = "025fd52b-e8d7-43aa-a10f-e8f9bde3e369";
        private static string endpoint = "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/" + kID + "/generateAnswer";

    }
}