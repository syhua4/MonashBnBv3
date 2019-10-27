using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonashBnBv3.Models
{
    public class MailReceipientList
    {
        public List<SelectedReceipientVM> Selected { get; set; }
        public MailReceipientList()
        {
            this.Selected = new List<SelectedReceipientVM>();
        }
    }
}