using Mono.TextTemplating;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BlazorAppIdentityDotNet.Data.Models
{
    public class Cerere
    {        
        public int CerereId { get; set; }
        [Required(ErrorMessage = "Introduceți prenumele")]
        [MaxLength(64)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Introduceți numele")]
        [MaxLength(64)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Introduceți data nașterii")]
        [AgeRangeValidation(ErrorMessage = "Nu întrați în categoria de vârstă potrivită")]
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "Introduceți IDNP")]
        [MaxLength(13, ErrorMessage ="IDNP trebuie să conțină 13 cifre")]
        [RegularExpression("^[02][0-9]{12}$", ErrorMessage = "IDNP trebuie să înceapă cu 0 sau 2 și să conțină exact 13 cifre.")]

        public string IDNP { get; set; }
        [Required(ErrorMessage = "Nu ați ales categoria")]
        public string? Category { get; set; }

        public string State { get; set; }
        public string Message { get; set; }

        public DateTime CreateTime { get; set; }


    }

}

