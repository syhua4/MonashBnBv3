using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MonashBnBv3.Models
{
    public class SelectedReceipientVM
    {
        public bool isSelected { get; set; }
        public SelectedReceipientVM() {}
        //public String UserId { get; set; }
        public String Username { get; set; }
        public String Email { get; set; }
    }
}