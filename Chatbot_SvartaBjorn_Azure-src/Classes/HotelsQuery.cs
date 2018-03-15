using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Classes
{
    [Serializable]
    public class HotelsQuery
    {
        [Prompt("Vennligst legg inn din {&}")]
        public string Destinasjon { get; set; }

        [Prompt("Når vil du {&}?")]
        public DateTime SjekkeInn { get; set; }

        [Numeric(1, int.MaxValue)]
        [Prompt("Hvor mange {&} vil du leie rommet?")]
        public int Netter { get; set; }
    }
}