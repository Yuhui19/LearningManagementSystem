using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Users
    {
        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }

        public virtual Administrators Administrators { get; set; }
        public virtual Professors Professors { get; set; }
        public virtual Students Students { get; set; }
    }
}
