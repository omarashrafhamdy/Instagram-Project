using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Instagram.Models
{
    public class ChangePassword
    {
        [Required,DataType(DataType.Password)]
        public string Oldpassword { get; set; }

        [Required, DataType(DataType.Password)]
        public string Newpassword { get; set; }

        [Required, DataType(DataType.Password)]
        
        public string Confirmpassword { get; set; }


    }
}