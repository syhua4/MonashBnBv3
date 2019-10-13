using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonashBnBv3.Models
{
    public class SelectedReceipient
    {
        public bool Selected { get; set; }

        public SelectedReceipient() { }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}