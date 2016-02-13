using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fahm_e_Islam.Models
{
    public class RegisterViewModel
    
    {
        [Display(Name="Name")]
        public string FullName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Contact No")]
        public string PhoneNo { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
        public string Message { get; set; }
    }
}