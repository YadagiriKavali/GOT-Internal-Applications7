using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eseva.Models
{
    public class mobileBillData
    {
        [Required(ErrorMessage = "Mobile number required")]
        [RegularExpression("([0-9]+)")]
        public string msisdn { get; set; }
    }
}
