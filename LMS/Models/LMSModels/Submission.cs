using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public string UId { get; set; }
        public uint AId { get; set; }
        public DateTime? Time { get; set; }
        public string Solution { get; set; }
        public int? Score { get; set; }

        public virtual Assignments A { get; set; }
        public virtual Students U { get; set; }
    }
}
