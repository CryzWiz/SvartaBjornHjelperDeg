using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Bachelor_Gr4_Chatbot_MVC.Services;

namespace Bachelor_Gr4_Chatbot_MVC.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            string subject = String.Format("Fullf�r registrering");
            string url = HtmlEncoder.Default.Encode(link);
            string message = String.Format("En konto er blitt opprettet for deg i ChatBot. " +
                "Vennligst fullf�r registreringen ved � bekrefte e-post adressen din: <a href={0}>Bekreft e-post adresse</a>", url);
            return emailSender.SendEmailAsync(email, subject, message);
        }
    }
}
