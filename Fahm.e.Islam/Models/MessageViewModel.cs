using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fahm_e_Islam.Models
{
    public class MessageViewModel
    
    {

        [Display(Name = "Subject")]
        [Required(ErrorMessage = "Please describe your query in short.")]
        public string Subject { get; set; }


        [Display(Name="Name")]
        [Required(ErrorMessage="Your Name Is Required.")]
        public string FullName { get; set; }
        [Required(ErrorMessage="Email is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Display(Name = "Contact No")]
        public string PhoneNo { get; set; }
        [Required(ErrorMessage="Your city is required.")]
        public string City { get; set; }
        [Required(ErrorMessage = "Your country is required.")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Please give us some detail about the course(s) you are intrested in.")]
        [StringLength(1000,MinimumLength=50,ErrorMessage="Please give us some valid detail of your message (text of min 50 characters required).")]
        public string Message { get; set; }
    }
}