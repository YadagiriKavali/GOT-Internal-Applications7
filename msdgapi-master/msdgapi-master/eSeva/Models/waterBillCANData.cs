using System;
using System.ComponentModel.DataAnnotations;


namespace eseva.Models
{
    public class waterBillCANData
    {
        [Required(ErrorMessage = "CAN number required")]
        //[MaxLength(9, ErrorMessage = "Max length is 9 character")]
        //[MinLength(9, ErrorMessage = "Minimum length is 9 character")]
        [RegularExpression("([0-9]+)")]
        public string CANNo { get; set; }
    }
}