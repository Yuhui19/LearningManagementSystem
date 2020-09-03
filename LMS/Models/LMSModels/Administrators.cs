using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Administrators
    {
        public string UId { get; set; }

        public virtual Users U { get; set; }
    }
}
