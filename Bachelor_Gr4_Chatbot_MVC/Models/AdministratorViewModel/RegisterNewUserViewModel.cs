using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.AdministratorViewModels
{

    public class RegisterNewUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Epost")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Fornavn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Etternavn")]
        public string LastName { get; set; }
    }
}
