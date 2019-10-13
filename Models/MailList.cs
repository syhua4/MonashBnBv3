using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonashBnBv3.Models
{
    public class MailTemplate
    {
        public string To { get; set; }
        public string Subject { get; set; }

        [AllowHtml]
        public string Body { get; set; }
    }
}