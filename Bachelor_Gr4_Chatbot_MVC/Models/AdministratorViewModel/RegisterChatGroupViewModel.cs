﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.AdministratorViewModel
{
    public class RegisterChatGroupViewModel
    {
        [Required]
        [Display(Name = "Gruppe navn")]
        public string ChatGroupName { get; set; }
    }
}
