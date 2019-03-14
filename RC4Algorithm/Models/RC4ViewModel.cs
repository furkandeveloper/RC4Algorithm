using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RC4Algorithm.Models
{
    public class RC4ViewModel
    {
        public Guid Key { get; set; }

        [Required(ErrorMessage ="Plain Text alanı zorunludur.")]
        public string PlainText { get; set; }

        public string ChiperText { get; set; }
    }
}
